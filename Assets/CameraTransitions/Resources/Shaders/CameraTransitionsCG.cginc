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

// Unity.

half4 _MainTex_TexelSize;

// Constants.

#define _PI			3.141592653589

// Gamma <-> Linear.

inline half3 sRGB(half3 pixel)
{
  return (pixel <= half3(0.0031308, 0.0031308, 0.0031308)) ? pixel * 12.9232102 : 1.055f * pow(pixel, 0.41666) - 0.055;
}

inline half4 sRGB(half4 pixel)
{
  return (pixel <= half4(0.0031308, 0.0031308, 0.0031308, pixel.a)) ? pixel * 12.9232102 : 1.055 * pow(pixel, 0.41666) - 0.055;
}

inline half3 Linear(half3 pixel)
{
  return (pixel <= half3(0.0404482, 0.0404482, 0.0404482)) ? pixel / 12.9232102 : pow((pixel + 0.055) * 0.9478672, 2.4);
}

inline half4 Linear(half4 pixel)
{
  return (pixel <= half4(0.0404482, 0.0404482, 0.0404482, pixel.a)) ? pixel / 12.9232102 : pow((pixel + 0.055) * 0.9478672, 2.4);
}

// Luminance.
inline half Luminance601(half3 pixel)
{
  return dot(pixel, fixed3(0.299, 0.587, 0.114));
}

// Rand [0, 1].
inline half Rand01(half2 n)
{
  return frac(sin(dot(n, half2(12.9898, 78.233))) * 43758.5453);
}

// Signed rand [-1, 1].
inline half SRand(half2 n)
{
  return Rand01(n) * 2.0 - 1.0;
}

// Mod.
inline half2 Mod(half2 x, half y)
{
  return x - y * floor(x / y);
}

// Render-To-Texture UV.
inline half2 RenderTextureUV(half2 uv)
{

#if UNITY_UV_STARTS_AT_TOP
  if (_MainTex_TexelSize.y < 0)
    uv.y = 1.0 - uv.y;
#endif

#if INVERT_RENDERTEXTURE
  uv.y = 1.0 - uv.y;
#endif
  
  return uv;
}

// Render-To-Texture UV fix.
inline half2 FixUV(half2 uv)
{
#if UNITY_UV_STARTS_AT_TOP || SHADER_API_D3D9 || SHADER_API_D3D11
  if (_MainTex_TexelSize.y < 0.0)
    uv.y = 1.0 - uv.y;
#endif

  return uv;
}
