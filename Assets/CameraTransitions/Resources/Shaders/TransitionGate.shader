///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Gate"
{
  Properties
  {
    _MainTex("Base (RGB)", 2D) = "white" {}
    _SecondTex("Second (RGB)", 2D) = "white" {}

    _T("Amount", Range(0.0, 1.0)) = 1.0
  }

  CGINCLUDE
  #include "UnityCG.cginc"
  #include "CameraTransitionsCG.cginc"

  float _GatePerspective;
  float _GateDepth;
  float _GateReflection;

  inline bool InBounds(float2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline float3 BackgroundColor(float2 p, float2 pto, sampler2D to)
  {
    float3 pixel = 0.0; // Black.

    pto *= float2(1.0, -1.2);
    pto.y -= 0.02;

    if (InBounds(pto))
      pixel += lerp(0.0, tex2D(to, RenderTextureUV(pto)).rgb, _GateReflection * lerp(1.0, 0.0, pto.y));

    return pixel;
  }

  float4 frag(v2f_img i) : COLOR
  {
    float2 pfr = -1.0;
    float2 pto = -1.0;

    float middleSlit = 2.0 * abs(i.uv.x - 0.5) - _T;
    if (middleSlit > 0.0)
    {
      pfr = i.uv + (i.uv.x > 0.5 ? -1.0 : 1.0) * float2(0.5 * _T, 0.0);
      float d = 1.0 / (1.0 + _GatePerspective * _T * (1.0 - middleSlit));
      pfr.y -= d / 2.0;
      pfr.y *= d;
      pfr.y += d / 2.0;
    }

    float size = lerp(1.0, _GateDepth, 1.0 - _T);
    pto = (i.uv - 0.5) * size + 0.5;

    if (InBounds(pfr))
#if MODE_REVERSE
      return tex2D(_SecondTex, pfr);
#else
      return tex2D(_MainTex, pfr);
#endif
    else if (InBounds(pto))
#if MODE_REVERSE
      return tex2D(_MainTex, RenderTextureUV(pto));
#else
      return tex2D(_SecondTex, RenderTextureUV(pto));
#endif

    return float4(BackgroundColor(i.uv, pto,
#if MODE_REVERSE
      _MainTex),
#else
      _SecondTex),
#endif
    1.0);
  }
  ENDCG

  SubShader
  {
    ZTest Always
    Cull Off
    ZWrite Off
    Fog { Mode off }

    Pass
    {
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma multi_compile ___ MODE_REVERSE
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}