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
Shader "Hidden/Camera Transitions/Valentine"
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
  half _ValentineBorder;
  half3 _ValentineColor;

  inline bool Heart(half2 p, half2 center, half size)
  {
    if (size == 0.0)
      return false;

    half2 o = (FixUV(p) - center) / (1.6 * size);

    return pow(o.x * o.x + o.y * o.y - 0.3, 3.0) < o.x * o.x * pow(o.y, 3.0);
  }

  half4 frag(v2f_img i) : COLOR
  {
    half3 from = tex2D(_MainTex, i.uv).rgb;
    half3 to = tex2D(_SecondTex, FixUV(i.uv)).rgb;

    half h1 = Heart(i.uv, half2(0.5, 0.4), _T) ? 1.0 : 0.0;
    half h2 = Heart(i.uv, half2(0.5, 0.4), _T + 0.001 * _ValentineBorder) ? 1.0 : 0.0;

    half border = max(h2 - h1, 0.0);

    return half4(lerp(from, to, h1) * (1.0 - border) + _ValentineColor * border, 1.0);
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