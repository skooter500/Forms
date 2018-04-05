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
Shader "Hidden/Camera Transitions/Glitch"
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
  half _GlitchStrength;

  inline half3 Glitch(sampler2D tex, half2 uv, half progress)
  {
    half2 block = floor(_ScreenParams.xy / 16.0);
    half2 uvNoise = block / 64.0;
    uvNoise += floor(_T * half2(1200.0, 3500.0)) / 64.0;

    half blockThresh = pow(frac(_T * 1200.0), 2.0) * 0.2;
    half lineThresh = pow(frac(_T * 2200.0), 3.0) * 0.7;
    
    half2 red = uv, green = uv, blue = uv, o = uv;

    half2 dist = (frac(uvNoise) - 0.5) * _GlitchStrength * progress;
    red += dist * 0.1;
    green += dist * 0.2;
    blue += dist * 0.125;
    
    return half3(tex2D(tex, red).r, tex2D(tex, green).g, tex2D(tex, blue).b);
  }

  half4 frag(v2f_img i) : COLOR
  {
    half smoothed = smoothstep(0.0, 1.0, _T);

    return half4(lerp(Glitch(_MainTex, i.uv, sin(smoothed)), Glitch(_SecondTex, FixUV(i.uv), sin(1.0 - smoothed)), smoothed), 1.0);
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