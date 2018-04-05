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
Shader "Hidden/Camera Transitions/Swap"
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
  half _SwapPerspective;
  half _SwapDepth;
  half _SwapReflection;

  inline bool InBounds(half2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline half3 BackgroundColor(half2 p, half2 pfr, half2 pto, sampler2D from, sampler2D to)
  {
    half3 pixel = half3(0.0, 0.0, 0.0); // Black.

    pfr.y *= -1.0;

    if (InBounds(pfr))
      pixel += lerp(0.0, tex2D(from, pfr), _SwapReflection * lerp(1.0, 0.0, pfr.y));

    pto.y *= -1.0;

    if (InBounds(pto))
      pixel += lerp(0.0, tex2D(to, pto), _SwapReflection * lerp(1.0, 0.0, pto.y));

    return pixel;
  }

  half4 frag(v2f_img i) : COLOR
  {
    //i.uv = FixUV(i.uv);

    half2 pfr = -1.0;
    half2 pto = -1.0;
    half size = lerp(1.0, _SwapDepth, _T);
    half persp = _SwapPerspective * _T;

    pfr = (i.uv + half2(0.0, -0.5)) * half2(size / (1.0 - _SwapPerspective * _T), size / (1.0 - size * persp * i.uv.x)) + half2(0.0, 0.5);

    size = lerp(1.0, _SwapDepth, 1.0 - _T);
    persp = _SwapPerspective * (1.0 - _T);

    pto = (i.uv + half2(-1.0, -0.5)) * half2(size / (1.0 - _SwapPerspective * (1.0 - _T)), size / (1.0 - size * persp * (0.5 - i.uv.x))) + half2(1.0, 0.5);

    half4 final = half4(0.0, 0.0, 0.0, 0.0);

    if (_T < 0.5)
    {
      if (InBounds(pfr))
        final = tex2D(_MainTex, pfr);
      else if (InBounds(pto))
        final = tex2D(_SecondTex, pto);
      else
        final = half4(BackgroundColor(i.uv, pfr, pto, _MainTex, _SecondTex), 1.0);
    }
    else
    {
      if (InBounds(pto))
        final = tex2D(_SecondTex, FixUV(pto));
      else if (InBounds(pfr))
        final = tex2D(_MainTex, pfr);
      else
        final = half4(BackgroundColor(i.uv, pfr, pto, _MainTex, _SecondTex), 1.0);
    }

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