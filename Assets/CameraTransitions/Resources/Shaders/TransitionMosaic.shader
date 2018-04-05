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
Shader "Hidden/Camera Transitions/Mosaic"
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
  int _StepX;
  int _StepY;
  int _Rotate;

  inline half2 Rotate(half2 v, half a)
  {
    return mul(v, half2x2(cos(a), -sin(a), sin(a), cos(a)));
  }

  half4 frag(v2f_img i) : COLOR
  {
    half2 uv = i.uv - 0.5;
    half st = _T * 2.0 - 1.0;

    uv *= abs(-(st * st * 2.0) + 3.0);
    uv += lerp(half2(0.5, 0.5), half2(half(_StepX) + 0.5, half(_StepY) + 0.5), _T * _T);
    
    half2 mrp = Mod(uv, 1.0);
    bool onEnd = int(floor(uv.x)) == _StepX && int(floor(uv.y)) == _StepY;

    if (onEnd == false && _Rotate == 1)
    {
      half ang = half(int(Rand01(floor(uv)) * 4.0)) * 0.5 * _PI;
      mrp = half2(0.5, 0.5) + Rotate(mrp - half2(0.5, 0.5), ang);
    }

    half3 pixel = 0.0;
    if (onEnd == true || Rand01(floor(uv)) > 0.5)
      pixel = tex2D(_SecondTex, FixUV(mrp)).rgb;
    else
      pixel = tex2D(_MainTex, mrp).rgb;

    return half4(pixel, 1.0);
  }
  ENDCG

  // Techniques (http://unity3d.com/support/documentation/Components/SL-SubShader.html).
  SubShader
  {
    // Tags (http://docs.unity3d.com/Manual/SL-CullAndDepth.html).
    ZTest Always
    Cull Off
    ZWrite Off
    Fog{ Mode off }

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