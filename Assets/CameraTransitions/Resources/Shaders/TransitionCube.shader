///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Cube"
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

  float _CubePerspective;
  float _CubeZoom;
  float _CubeReflection;
  float _CubeElevantion;

  inline bool InBounds(float2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline float3 BackgroundColorGamma(float2 p, float2 pfr, float2 pto, sampler2D from, sampler2D to)
  {
    float3 pixel = float3(0.0, 0.0, 0.0);

    pfr.y *= -1.2;
    pfr.y += _CubeElevantion;

    if (all(0.0 < pfr) && all(pfr < 1.0))
      pixel += lerp(float3(0.0, 0.0, 0.0), tex2D(from, pfr), _CubeReflection * (1.0 - pfr.y));
      
    pto.y *= -1.2;
    pto.y += _CubeElevantion;

    if (all(0.0 < pto) && all(pto < 1.0))
      pixel += lerp(float3(0.0, 0.0, 0.0), tex2D(to, pto), _CubeReflection * (1.0 - pto.y));

    return pixel;
  }

  inline float3 BackgroundColorLinear(float2 p, float2 pfr, float2 pto, sampler2D from, sampler2D to)
  {
    float3 pixel = float3(0.0, 0.0, 0.0);

    pfr.y *= -1.2;
    pfr.y += _CubeElevantion;

    if (all(0.0 < pfr) && all(pfr < 1.0))
      pixel += lerp(float3(0.0, 0.0, 0.0), sRGB(tex2D(from, pfr).rgb), _CubeReflection * (1.0 - pfr.y));

    pto.y *= -1.2;
    pto.y += _CubeElevantion;

    if (all(0.0 < pto) && all(pto < 1.0))
      pixel += lerp(float3(0.0, 0.0, 0.0), sRGB(tex2D(to, RenderTextureUV(pto)).rgb), _CubeReflection * (1.0 - pto.y));

    return pixel;
  }

  inline float2 XSkew(float2 p, float persp, float center)
  {
    float x = lerp(p.x, 1.0 - p.x, center);

    float2 coord = float2(x, (p.y - 0.5 * (1.0 - persp) * x) / (1.0 + (persp - 1.0) * x)) - float2(0.5 - distance(center, 0.5), 0.0);

    float d = 0.5 / distance(center, 0.5);

    if (center > 0.5)
      d *= -1.0;

    coord.x *= d;

    if (center > 0.5)
      coord.x += 1.0;

    return coord;
  }

  float4 frag(v2f_img i) : COLOR
  {
    float uz = _CubeZoom * 2.0 * (0.5 - distance(0.5, _T));
    float2 p = -uz * 0.5 + (1.0 + uz) * i.uv;
    
    float2 fromP = XSkew((p - float2(_T, 0.0)) / float2(1.0 - _T, 1.0), 1.0 - lerp(_T, 0.0, _CubePerspective), 0.0);
    float2 toP = XSkew(p / float2(_T, 1.0), lerp(pow(_T, 2.0), 1.0, _CubePerspective), 1.0);

    float4 final = 0.0;

    if (InBounds(fromP))
#if MODE_REVERSE
      final = tex2D(_SecondTex, fromP);
#else
      final = tex2D(_MainTex, fromP);
#endif
    else if (InBounds(toP))
#if MODE_REVERSE
      final = tex2D(_MainTex, RenderTextureUV(toP));
#else
      final = tex2D(_SecondTex, RenderTextureUV(toP));
#endif
    else
      final = float4(BackgroundColorGamma(i.uv, fromP, toP,
#if MODE_REVERSE
        _SecondTex, _MainTex),
#else
        _MainTex, _SecondTex),
#endif
        1.0);
  
    return final;
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