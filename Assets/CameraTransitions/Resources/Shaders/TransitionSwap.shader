///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Swap"
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

  float _SwapPerspective;
  float _SwapDepth;
  float _SwapReflection;

  inline bool InBounds(float2 p)
  {
    return all(0.0 < p) && all(p < 1.0);
  }

  inline float3 BackgroundColor(float2 p, float2 pfr, float2 pto, sampler2D from, sampler2D to)
  {
    float3 pixel = 0.0; // Black.

    pfr.y *= -1.0;

    if (InBounds(pfr))
      pixel += lerp(0.0, tex2D(from, pfr), _SwapReflection * lerp(1.0, 0.0, pfr.y));

    pto.y *= -1.0;

    if (InBounds(pto))
      pixel += lerp(0.0, tex2D(to, pto), _SwapReflection * lerp(1.0, 0.0, pto.y));

    return pixel;
  }

  float4 frag(v2f_img i) : COLOR
  {
    float2 pfr = -1.0;
    float2 pto = -1.0;
    float size = lerp(1.0, _SwapDepth, _T);
    float persp = _SwapPerspective * _T;

    pfr = (i.uv + float2(0.0, -0.5)) * float2(size / (1.0 - _SwapPerspective * _T), size / (1.0 - size * persp * i.uv.x)) + float2(0.0, 0.5);

    size = lerp(1.0, _SwapDepth, 1.0 - _T);
    persp = _SwapPerspective * (1.0 - _T);

    pto = (i.uv + float2(-1.0, -0.5)) * float2(size / (1.0 - _SwapPerspective * (1.0 - _T)), size / (1.0 - size * persp * (0.5 - i.uv.x))) + float2(1.0, 0.5);

    float4 final = 0.0;

#if MODE_REVERSE
    if (_T < 0.5)
    {
      if (InBounds(pfr))
        final = tex2D(_SecondTex, pfr);
      else if (InBounds(pto))
        final = tex2D(_MainTex, RenderTextureUV(pto));
      else
        final = float4(BackgroundColor(i.uv, pfr, pto, _SecondTex, _MainTex), 1.0);
    }
    else
    {
      if (InBounds(pto))
        final = tex2D(_MainTex, RenderTextureUV(pto));
      else if (InBounds(pfr))
        final = tex2D(_SecondTex, pfr);
      else
        final = float4(BackgroundColor(i.uv, pfr, pto, _SecondTex, _MainTex), 1.0);
    }
#else
    if (_T < 0.5)
    {
      if (InBounds(pfr))
        final = tex2D(_MainTex, pfr);
      else if (InBounds(pto))
        final = tex2D(_SecondTex, RenderTextureUV(pto));
      else
        final = float4(BackgroundColor(i.uv, pfr, pto, _MainTex, _SecondTex), 1.0);
    }
    else
    {
      if (InBounds(pto))
        final = tex2D(_SecondTex, RenderTextureUV(pto));
      else if (InBounds(pfr))
        final = tex2D(_MainTex, pfr);
      else
        final = float4(BackgroundColor(i.uv, pfr, pto, _MainTex, _SecondTex), 1.0);
    }
#endif

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