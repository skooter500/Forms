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
Shader "Hidden/Camera Transitions/Gate"
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
  half _GatePerspective;
  half _GateDepth;
  half _GateReflection;

  inline bool InBounds(half2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline half3 BackgroundColor(half2 p, half2 pto, sampler2D to)
  {
    half3 pixel = half3(0.0, 0.0, 0.0); // Black.

    pto *= half2(1.0, -1.2);
    pto.y -= 0.02;

    if (InBounds(pto))
      pixel += lerp(0.0, tex2D(to, pto).rgb, _GateReflection * lerp(1.0, 0.0, pto.y));

    return pixel;
  }

  half4 frag(v2f_img i) : COLOR
  {
    i.uv = FixUV(i.uv);

    half2 pfr = -1.0;
    half2 pto = -1.0;

    half middleSlit = 2.0 * abs(i.uv.x - 0.5) - _T;
    if (middleSlit > 0.0)
    {
      pfr = i.uv + (i.uv.x > 0.5 ? -1.0 : 1.0) * half2(0.5 * _T, 0.0);
      half d = 1.0 / (1.0 + _GatePerspective * _T * (1.0 - middleSlit));
      pfr.y -= d / 2.0;
      pfr.y *= d;
      pfr.y += d / 2.0;
    }

    half size = lerp(1.0, _GateDepth, 1.0 - _T);
    pto = (i.uv - 0.5) * size + 0.5;

    if (InBounds(pfr))
      return tex2D(_MainTex, FixUV(pfr));
    else if (InBounds(pto))
      return tex2D(_SecondTex, pto);

    return half4(BackgroundColor(i.uv, pto, _SecondTex), 1.0);
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