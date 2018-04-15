///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Fold"
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

  float4 frag(v2f_img i) : COLOR
  {
    float3 from = tex2D(
#if MODE_REVERSE      
      _SecondTex,
#else
      _MainTex,
#endif
#ifdef MODE_HORIZONTAL
      (i.uv - float2(_T, 0.0)) / float2(1.0 - _T, 1.0));
#else
      (i.uv - float2(0.0, _T)) / float2(1.0, 1.0 - _T));
#endif

    float3 to = tex2D(
#if MODE_REVERSE      
      _MainTex,
#else
      _SecondTex,
#endif
      RenderTextureUV(i.uv) /
#ifdef MODE_HORIZONTAL
      float2(_T, 1.0)).rgb;
#else
      float2(1.0, _T)).rgb;
#endif

    return float4(lerp(from, to,
#ifdef MODE_HORIZONTAL
                       step(i.uv.x, _T)), 1.0);
#else
                       step(i.uv.y, _T)), 1.0);
#endif
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
      #pragma multi_compile ___ MODE_HORIZONTAL
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}