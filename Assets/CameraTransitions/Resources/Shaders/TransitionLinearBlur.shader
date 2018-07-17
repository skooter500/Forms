///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "Hidden/Camera Transitions/Linear Blur"
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

  float _Intensity;
  int _Passes;

  float4 frag(v2f_img i) : COLOR
  {
    float3 from = 0.0, to = 0.0;
    float displacement = _Intensity * (0.5 - distance(0.5, _T));
	  float2 secondUV = RenderTextureUV(i.uv);

#if SHADER_API_D3D9
	  _Passes = 3;
#endif
    for (int xi = 0; xi < _Passes; ++xi)
    {
      float x = fixed(xi) / fixed(_Passes) - 0.5;

      for (int yi = 0; yi < _Passes; ++yi)
      {
        float y = fixed(yi) / fixed(_Passes) - 0.5;
      
        float2 v = float2(x, y);
        from += tex2D(_MainTex, i.uv + displacement * v).rgb;
        to += tex2D(_SecondTex, secondUV + displacement * v).rgb;
      }
    }

    from /= fixed(_Passes * _Passes);
    to /= fixed(_Passes * _Passes);

    return float4(lerp(from, to, _T), 1.0);
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
      #pragma multi_compile ___ INVERT_RENDERTEXTURE
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
    }
  }

  Fallback "Transition Fallback"
}