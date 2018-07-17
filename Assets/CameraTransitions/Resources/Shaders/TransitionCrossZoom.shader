﻿///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Cross Zoom"
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

  float _Strength;
  float _Quality;
  
  inline float LinearEase(float begin, float change, float duration, float time)
  {
    return change * time / duration + begin;
  }

  inline float ExponentialEaseInOut(float begin, float change, float duration, float time)
  {
    if (time == 0.0)
      return begin;
    else if (time == duration)
      return begin + change;

    time = time / (duration * 0.5);

    if (time < 1.0)
      return change / 2.0 * pow(2.0, 10.0 * (time - 1.0)) + begin;

    return change / 2.0 * (-pow(2.0, -10.0 * (time - 1.0)) + 2.0) + begin;
  }

  inline float SinusoidalEaseInOut(float begin, float change, float duration, float time)
  {
    return -change / 2.0 * (cos(_PI * time / duration) - 1.0) + begin;
  }
  
  inline float3 CrossFade(float2 uv, float dissolve)
  {
#if MODE_REVERSE
    return lerp(tex2D(_SecondTex, uv).rgb, tex2D(_MainTex, RenderTextureUV(uv)).rgb, dissolve);
#else
    return lerp(tex2D(_MainTex, uv).rgb, tex2D(_SecondTex, RenderTextureUV(uv)).rgb, dissolve);
#endif
  }
  
  float4 frag(v2f_img i) : COLOR
  {
    float2 center = float2(LinearEase(0.25, 0.5, 1.0, _T), 0.5);

    float dissolve = ExponentialEaseInOut(0.0, 1.0, 1.0, _T);
    float strength = SinusoidalEaseInOut(0.0, _Strength, 0.5, _T);

    float3 color = float3(0.0, 0.0, 0.0);
    float total = 0.0;
    float2 toCenter = center - i.uv;
    float offset = Rand01(i.uv * _ScreenParams.xy);

    for (float t = 0.0; t <= _Quality; ++t)
    {
      float percent = (t + offset) / _Quality;
      float weight = 4.0 * (percent - percent * percent);
      color += CrossFade(i.uv + toCenter * percent * strength, dissolve) * weight;
      total += weight;
    }

    return float4(color / total, 1.0);
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