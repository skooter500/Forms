///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity.
float4 _MainTex_TexelSize;

// Variables.
sampler2D _MainTex;
sampler2D _SecondTex;

float _T;
float4 _CustomTime;

// Constants.
#define _PI			3.141592653589

// Gamma <-> Linear.

inline float3 sRGB(float3 pixel)
{
  return (pixel <= float3(0.0031308, 0.0031308, 0.0031308)) ? pixel * 12.9232102 : 1.055f * pow(pixel, 0.41666) - 0.055;
}

inline float4 sRGB(float4 pixel)
{
  return (pixel <= float4(0.0031308, 0.0031308, 0.0031308, pixel.a)) ? pixel * 12.9232102 : 1.055 * pow(pixel, 0.41666) - 0.055;
}

inline float3 Linear(float3 pixel)
{
  return (pixel <= float3(0.0404482, 0.0404482, 0.0404482)) ? pixel / 12.9232102 : pow((pixel + 0.055) * 0.9478672, 2.4);
}

inline float4 Linear(float4 pixel)
{
  return (pixel <= float4(0.0404482, 0.0404482, 0.0404482, pixel.a)) ? pixel / 12.9232102 : pow((pixel + 0.055) * 0.9478672, 2.4);
}

// Luminance.
inline float Luminance601(float3 pixel)
{
  return dot(pixel, float3(0.299, 0.587, 0.114));
}

// Rand [0, 1].
inline float Rand01(float2 n)
{
  return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
}

// Signed rand [-1, 1].
inline float SRand(float2 n)
{
  return Rand01(n) * 2.0 - 1.0;
}

// Mod.
inline float2 Mod(float2 x, float y)
{
  return x - y * floor(x / y);
}

// Render-To-Texture UV.
inline float2 RenderTextureUV(float2 uv)
{
#ifdef INVERT_RENDERTEXTURE
  return float2(uv.x, 1.0 - uv.y);
#elif defined(UNITY_UV_STARTS_AT_TOP)
  if (_MainTex_TexelSize.y < 0)
    return float2(uv.x, 1.0 - uv.y);
#endif
  return uv;
}
