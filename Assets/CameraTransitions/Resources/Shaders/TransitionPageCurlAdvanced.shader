///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Page Curl Advanced"
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

  float _Radius;
  float2 _Angle;
  int _Obtuse;

  int _FrontShadow;
  int _BackShadow;
  int _InnerShadow;
  float _BackTransparency;

  float2 PageCurl(float t, float maxt, float cyl)
  {
    float2 ret = float2(t, 1.0);

    if (t < cyl - _Radius)
      return ret;

    if (t > cyl + _Radius)
      return float2(-1.0, -1.0);

    float a = asin((t - cyl) / _Radius);
    float ca = -a + _PI;

    ret.x = cyl + ca * _Radius;
    ret.y = cos(ca);

    if (ret.x < maxt)
      return ret;

    if (t < cyl)
      return float2(t, 1.0);

    ret.x = cyl + a * _Radius;
    ret.y = cos(a);

    return (ret.x < maxt) ? ret : float2(-1.0, -1.0);
  }

  float4 frag(v2f_img i) : COLOR
  {
    float2 uv = (_Obtuse == 0) ? i.uv : float2(1.0 - i.uv.x, i.uv.y);

    float2 angle = _Angle * _T;
    float d = length(angle * (1.0 + 4.0 * _Radius)) - 2.0 * _Radius;
    float3 cyl = float3(normalize(angle), d);

    d = dot(uv, cyl.xy);
    float2 end = abs((1.0 - uv) / cyl.xy);
    float maxt = d + min(end.x, end.y);
    float2 cf = PageCurl(d, maxt, cyl.z);
    float2 tuv = i.uv + cyl.xy * (cf.x - d);

    float shadow = 0.0;
    if (_FrontShadow == 1)
    {
      shadow = 1.0 - smoothstep(0.0, _Radius * 2.0, -(d - cyl.z));
      shadow *= (smoothstep(-_Radius, _Radius, (maxt - (cf.x + 1.5 * _PI * _Radius + _Radius))));
    }

#if MODE_REVERSE
    float3 from = tex2D(_MainTex, RenderTextureUV(tuv)).rgb;
#else
    float3 from = tex2D(_SecondTex, RenderTextureUV(tuv)).rgb;
#endif
    from = cf.y > 0.0 ? from * (_InnerShadow == 1 ? cf.y : 1.0) * (1.0 - shadow) : (from * _BackTransparency + (1.0 - _BackTransparency)) * (_BackShadow == 1 ? -cf.y : 1.0);

    shadow = _BackShadow == 1 ? smoothstep(0.0, _Radius * 2.0, (d - cyl.z)) : 1.0;

#if MODE_REVERSE
    float3 to = tex2D(_SecondTex, i.uv).rgb * shadow;
#else
    float3 to = tex2D(_MainTex, i.uv).rgb * shadow;
#endif

    return float4(cf.x > 0.0 ? from : to, 1.0);
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