///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Smooth Circle"
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

  float _Smoothness;
  float _Invert;
  float2 _Center;

  float4 frag(v2f_img i) : COLOR
  {
    float m = smoothstep(-_Smoothness, 0.0, 1.414213562373 * distance(_Center, i.uv) - (_Invert == 1 ? 1.0 - _T : _T) * (1.0 + _Smoothness));

#if MODE_REVERSE
    return float4(lerp(tex2D(_SecondTex, i.uv).rgb, tex2D(_MainTex, RenderTextureUV(i.uv)).rgb, (_Invert == 1 ? m : 1.0 - m)), 1.0);
#else
    return float4(lerp(tex2D(_MainTex, i.uv).rgb, tex2D(_SecondTex, RenderTextureUV(i.uv)).rgb, (_Invert == 1 ? m : 1.0 - m)), 1.0);
#endif
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