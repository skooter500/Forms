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
Shader "Hidden/Camera Transitions/Page Curl NoShadows"
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
	half _Radius;
	half2 _Angle;
  int _Obtuse;

	half2 PageCurl(half t, half maxt, half cyl)
	{
		half2 ret = half2(t, 1.0);

		if (t < cyl - _Radius)
			return ret;

		if (t > cyl + _Radius)
      return half2(-1.0, -1.0);

		half a = asin((t - cyl) / _Radius);
		half ca = -a + _PI;

		ret.x = cyl + ca * _Radius;
		ret.y = cos(ca);

		if (ret.x < maxt)
			return ret;

		if (t < cyl)
			return half2(t, 1.0);

		ret.x = cyl + a * _Radius;
		ret.y = cos(a);

		return (ret.x < maxt) ? ret : half2(-1.0, -1.0);
	}

	half4 frag(v2f_img i) : COLOR
	{
    half2 uv = (_Obtuse == 0) ? i.uv : half2(1.0 - i.uv.x, i.uv.y);

    half2 angle = _Angle * _T;
    half d = length(angle * (1.0 + 4.0 * _Radius)) - 2.0 * _Radius;
    half3 cyl = half3(normalize(angle), d);

    d = dot(uv, cyl.xy);
    half2 end = abs((1.0 - uv) / cyl.xy);
    half maxt = d + min(end.x, end.y);
    half2 cf = PageCurl(d, maxt, cyl.z);
    half2 tuv = i.uv + cyl.xy * (cf.x - d);

		half3 from = tex2D(_SecondTex, FixUV(tuv)).rgb;
		from = cf.y > 0.0 ? from : (from * 0.25 + 0.75);

		return half4(cf.x > 0.0 ? from : tex2D(_MainTex, i.uv).rgb, 1.0);
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