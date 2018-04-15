///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Warp Wave"
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

  float _WarpWaveCurvature;

  float4 frag(v2f_img i) : COLOR
  {
    float p = smoothstep(0.0, 1.0,
#ifdef MODE_HORIZONTAL
      (_T * 2.0 + i.uv.x - 1.0));
#else
      (_T * 2.0 + i.uv.y - 1.0));
#endif

#if MODE_REVERSE
    float3 from = tex2D(_SecondTex, (i.uv - _WarpWaveCurvature) * (1.0 - p) + _WarpWaveCurvature);
    float3 to = tex2D(_MainTex, RenderTextureUV((i.uv - _WarpWaveCurvature) * p + _WarpWaveCurvature));
#else
    float3 from = tex2D(_MainTex, (i.uv - _WarpWaveCurvature) * (1.0 - p) + _WarpWaveCurvature);
    float3 to = tex2D(_SecondTex, RenderTextureUV((i.uv - _WarpWaveCurvature) * p + _WarpWaveCurvature));
#endif

    return float4(lerp(from, to, _T), 1.0);
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
      #pragma multi_compile ___ MODE_HORIZONTAL
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}