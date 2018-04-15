///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

using UnityEngine;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// Easing equations.
    /// </summary>
    public enum EaseType
    {
      /// <summary>
      /// Linear.
      /// </summary>
      Linear,

      /// <summary>
      /// Quad.
      /// </summary>
      Quad,

      /// <summary>
      /// Cubic.
      /// </summary>
      Cubic,

      /// <summary>
      /// Quart.
      /// </summary>
      Quart,

      /// <summary>
      /// Quint.
      /// </summary>
      Quint,

      /// <summary>
      /// Sine.
      /// </summary>
      Sine,

      /// <summary>
      /// Exp.
      /// </summary>
      Expo,

      /// <summary>
      /// Circ.
      /// </summary>
      Circ,

      /// <summary>
      /// Elastic.
      /// </summary>
      Elastic,

      /// <summary>
      /// Back.
      /// </summary>
      Back,

      /// <summary>
      /// Bounce.
      /// </summary>
      Bounce,
    }

    /// <summary>
    /// Easing modes.
    /// </summary>
    public enum EaseMode
    {
      /// <summary>
      /// Only at the beginning.
      /// </summary>
      In,

      /// <summary>
      /// Only at the end.
      /// </summary>
      Out,

      /// <summary>
      /// At the beginning and end.
      /// </summary>
      InOut,
    }

    /// <summary>
    /// Easing calcs. Chekout this web http://easings.net.
    /// </summary>
    public static class Easing
    {
      /// <summary>
      /// Do easing.
      /// </summary>
      public static float Ease(EaseType type, EaseMode mode, float from, float to, float t)
      {
        Func<float, float, float> easeFunc = null;
        switch (type)
        {
          case EaseType.Linear:   easeFunc = Linear; break;
          case EaseType.Quad:     easeFunc = Quad; break;
          case EaseType.Cubic:    easeFunc = Cubic; break;
          case EaseType.Quart:    easeFunc = Quart; break;
          case EaseType.Quint:    easeFunc = Quint; break;
          case EaseType.Sine:     easeFunc = Sine; break;
          case EaseType.Expo:     easeFunc = Expo; break;
          case EaseType.Circ:     easeFunc = Circ; break;
          case EaseType.Elastic:  easeFunc = Elastic; break;
          case EaseType.Back:     easeFunc = Back; break;
          case EaseType.Bounce:   easeFunc = Bounce; break;
        }

        switch (mode)
        {
          case EaseMode.In: return In(easeFunc, t, from, to - from);
          case EaseMode.Out: return Out(easeFunc, t, from, to - from);
          case EaseMode.InOut: return InOut(easeFunc, t, from, to - from);
        }

        return 0.0f;
      }

      private static float In(Func<float, float, float> easeFunc, float t, float b, float c, float d = 1.0f)
      {
        if (t >= d)
          return b + c;

        if (t <= 0.0f)
          return b;

        return c * easeFunc(t, d) + b;
      }

      private static float Out(Func<float, float, float> easeFunc, float t, float b, float c, float d = 1.0f)
      {
        if (t >= d)
          return b + c;

        if (t <= 0.0f)
          return b;

        return (b + c) - c * easeFunc(d - t, d);
      }

      private static float InOut(Func<float, float, float> easeFunc, float t, float b, float c, float d = 1.0f)
      {
        if (t >= d)
          return b + c;

        if (t <= 0.0f)
          return b;

        if (t < d / 2.0f)
          return In(easeFunc, t * 2.0f, b, c / 2.0f, d);

        return Out(easeFunc, (t * 2.0f) - d, b + c / 2.0f, c / 2.0f, d);
      }

      private static float Linear(float t, float d = 1.0f)
      {
        return t / d;
      }

      private static float Quad(float t, float d = 1.0f)
      {
        return (t /= d) * t;
      }

      private static float Cubic(float t, float d = 1.0f)
      {
        return (t /= d) * t * t;
      }

      private static float Quart(float t, float d = 1.0f)
      {
        return (t /= d) * t * t * t;
      }

      private static float Quint(float t, float d = 1.0f)
      {
        return (t /= d) * t * t * t * t;
      }

      private static float Sine(float t, float d = 1.0f)
      {
        return 1.0f - Mathf.Cos(t / d * (Mathf.PI / 2.0f));
      }

      private static float Expo(float t, float d = 1.0f)
      {
        return Mathf.Pow(2.0f, 10.0f * (t / d - 1.0f));
      }

      private static float Circ(float t, float d = 1.0f)
      {
        return -(Mathf.Sqrt(1.0f - (t /= d) * t) - 1.0f);
      }

      private static float Elastic(float t, float d = 1.0f)
      {
        t /= d;
        float p = d * 0.3f;
        float s = p / 4.0f;

        return -(Mathf.Pow(2.0f, 10.0f * (t -= 1.0f)) * Mathf.Sin((t * d - s) * (2.0f * Mathf.PI) / p));
      }

      private static float Back(float t, float d = 1.0f)
      {
        return (t /= d) * t * ((1.70158f + 1) * t - 1.70158f);
      }

      private static float Bounce(float t, float d = 1.0f)
      {
        t = d - t;
        if ((t /= d) < (1 / 2.75f))
          return 1.0f - (7.5625f * t * t);
        else if (t < (2.0f / 2.75f))
          return 1.0f - (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f);
        else if (t < (2.5f / 2.75f))
          return 1.0f - (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f);

        return 1.0f - (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f);
      }
    }
  }
}