///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Camera Transitions.
//
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// http://unity3d.com/support/documentation/Components/SL-Shader.html
Shader "Hidden/Camera Transitions/Cube"
{
  // http://unity3d.com/support/documentation/Components/SL-Properties.html
  Properties
  {
    _MainTex("Base (RGB)", 2D) = "white" {}

    _SecondTex("Second (RGB)", 2D) = "white" {}

	  // Transition.
    _T("Amount", Range(0.0, 1.0)) = 1.0
  }

  CGINCLUDE
  #include "UnityCG.cginc"
  #include "CameraTransitionsCG.cginc"

  sampler2D _MainTex;
  sampler2D _SecondTex;

  half _T;
  half _CubePerspective;
  half _CubeZoom;
  half _CubeReflection;
  half _CubeElevantion;

  inline bool InBounds(half2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline half3 BackgroundColor(half2 p, half2 pfr, half2 pto, sampler2D from, sampler2D to)
  {
    half3 pixel = half3(0.0, 0.0, 0.0);

    pfr.y *= -1.2;
    pfr.y += _CubeElevantion;

    if (all(0.0 < pfr) && all(pfr < 1.0))
      pixel += lerp(half3(0.0, 0.0, 0.0), tex2D(from, pfr), _CubeReflection * (1.0 - pfr.y));
      
    pto.y *= -1.2;
    pto.y += _CubeElevantion;

    if (all(0.0 < pto) && all(pto < 1.0))
      pixel += lerp(half3(0.0, 0.0, 0.0), tex2D(to, pto), _CubeReflection * (1.0 - pto.y));

    return pixel;
  }

  inline half2 XSkew(half2 p, half persp, half center)
  {
    half x = lerp(p.x, 1.0 - p.x, center);

    half2 coord = half2(x, (p.y - 0.5 * (1.0 - persp) * x) / (1.0 + (persp - 1.0) * x)) - half2(0.5 - distance(center, 0.5), 0.0);

    half d = 0.5 / distance(center, 0.5);

    if (center > 0.5)
      d *= -1.0;

    coord.x *= d;

    if (center > 0.5)
      coord.x += 1.0;

    return coord;
  }

  half4 frag(v2f_img i) : COLOR
  {
    half uz = _CubeZoom * 2.0 * (0.5 - distance(0.5, _T));
    half2 p = -uz * 0.5 + (1.0 + uz) * FixUV(i.uv);
    
    half2 fromP = XSkew((p - half2(_T, 0.0)) / half2(1.0 - _T, 1.0), 1.0 - lerp(_T, 0.0, _CubePerspective), 0.0);
    half2 toP = XSkew(p / half2(_T, 1.0), lerp(pow(_T, 2.0), 1.0, _CubePerspective), 1.0);

    half4 final = 0.0;

    if (InBounds(fromP))
      final = tex2D(_MainTex, FixUV(fromP));
    else if (InBounds(toP))
      final = tex2D(_SecondTex, toP);
    else
      final = half4(BackgroundColor(i.uv, fromP, toP, _MainTex, _SecondTex), 1.0);
  
    return final;
  }
  ENDCG

  // Techniques (http://unity3d.com/support/documentation/Components/SL-SubShader.html).
  SubShader
  {
    // Tags (http://docs.unity3d.com/Manual/SL-CullAndDepth.html).
    ZTest Always
    Cull Off
    ZWrite Off
    Fog { Mode off }

    Pass
    {
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}