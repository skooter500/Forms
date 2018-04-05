﻿///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
Shader "Hidden/Camera Transitions/Cross Zoom"
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
  half _Strength;
  half _Quality;
  
  inline half ExponentialEaseInOut(half time)
  {
    if (time == 0.0 || time == 1.0)
      return time;

    time *= 2.0;

    if (time < 1.0)
      return 1.0 / 2.0 * pow(2.0, 10.0 * (time - 1.0));

    return 1.0 / 2.0 * (-pow(2.0, -10.0 * (time - 1.0)) + 2.0);
  }
  
  half4 frag(v2f_img i) : COLOR
  {
    half2 center = half2(0.5 * _T / 1.0 + 0.25, 0.5);

    half dissolve = ExponentialEaseInOut(_T);
    half strength = -_Strength / 2.0 * (cos(_PI * _T / 0.5) - 1.0);

    half3 color = half3(0.0, 0.0, 0.0);
    half total = 0.0;
    half2 toCenter = center - i.uv;
    half offset = Rand01(i.uv * _ScreenParams.xy);

#if defined(SHADER_API_D3D9) || defined(SHADER_API_MOBILE)
    half percent = 0.0;
    half weight = 0.0;
    half2 uv = half2(0.0, 0.0);

    #define SAMPLEIT(t) percent = (t + offset) / 5.0; weight = 4.0 * (percent - percent * percent); uv = i.uv + toCenter * percent * strength; color += lerp(tex2D(_MainTex, uv).rgb, tex2D(_SecondTex, RenderTextureUV(uv)).rgb, dissolve) * weight; total += weight;
    SAMPLEIT(0.0)
    SAMPLEIT(1.0)
    SAMPLEIT(2.0)
    SAMPLEIT(3.0)
    SAMPLEIT(4.0)
    #undef SAMPLEIT
#else
    for (half t = 0.0; t <= _Quality; ++t)
    {
      half percent = (t + offset) / _Quality;
      half weight = 4.0 * (percent - percent * percent);
      half2 uv = i.uv + toCenter * percent * strength;
      color += lerp(tex2D(_MainTex, uv).rgb, tex2D(_SecondTex, FixUV(uv)).rgb, dissolve) * weight;
      total += weight;
    }
#endif

    return half4(color / total, 1.0);
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