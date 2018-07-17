///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Flash"
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

  float _FlashPhase;
  float _FlashIntensity;
  float _FlashZoom;
  float _FlashVelocity;
  float4 _FlashColor;

  float4 frag(v2f_img i) : COLOR
  {
    float3 fromPixel = tex2D(_MainTex, i.uv).rgb;
    float3 toPixel = tex2D(_SecondTex, RenderTextureUV(i.uv)).rgb;

    float intensity = lerp(1.0, 2.0 * distance(i.uv, float2(0.5, 0.5)), _FlashZoom) * _FlashIntensity * pow(smoothstep(_FlashPhase, 0.0, distance(0.5, _T)), _FlashVelocity);

    float3 pixel = lerp(fromPixel, toPixel, smoothstep(0.5 * (1.0 - _FlashPhase), 0.5 * (1.0 + _FlashPhase), _T));
    pixel += intensity * _FlashColor.rgb;

    return float4(pixel, 1.0);
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
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}