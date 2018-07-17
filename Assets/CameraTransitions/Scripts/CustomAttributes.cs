///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class EnumAttribute : PropertyAttribute
    {
      public List<string> enumNames = new List<string>();

      private Type type;

      public EnumAttribute(Type enumType)
      {
        type = enumType;
        
        enumNames.AddRange(Enum.GetNames(type));
      }
    }

    /// <summary>
    /// .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RangeIntAttribute : PropertyAttribute
    {
      public readonly int min;

      public readonly int max;

      public readonly int reset;

      public RangeIntAttribute(int min, int max) : this(min, max, min)
      {
      }

      public RangeIntAttribute(int min, int max, int reset)
      {
        this.min = min;
        this.max = max;
        this.reset = reset;
      }
    }

    /// <summary>
    /// .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RangeFloatAttribute : PropertyAttribute
    {
      public readonly float min;

      public readonly float max;

      public readonly float reset;

      public RangeFloatAttribute(float min, float max) : this(min, max, min)
      {
      }

      public RangeFloatAttribute(float min, float max, float reset)
      {
        this.min = min;
        this.max = max;
        this.reset = reset;
      }
    }

    /// <summary>
    /// .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RangeVector2Attribute : PropertyAttribute
    {
      public readonly Vector2 min;

      public readonly Vector2 max;

      public readonly Vector2 reset;

      public RangeVector2Attribute(float minMag, float maxMag) : this(minMag, maxMag, minMag)
      {
      }

      public RangeVector2Attribute(float minMag, float maxMag, float resetMag)
      {
        this.min = new Vector2(minMag, minMag);
        this.max = new Vector2(maxMag, maxMag);
        this.reset = new Vector2(resetMag, resetMag);
      }
    }

    /// <summary>
    /// .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RangeVector3Attribute : PropertyAttribute
    {
      public readonly Vector3 min;

      public readonly Vector3 max;

      public readonly Vector3 reset;

      public RangeVector3Attribute(float minMag, float maxMag) : this(minMag, maxMag, minMag)
      {
      }

      public RangeVector3Attribute(float minMag, float maxMag, float resetMag)
      {
        this.min = new Vector3(minMag, minMag, minMag);
        this.max = new Vector3(maxMag, maxMag, maxMag);
        this.reset = new Vector3(resetMag, resetMag, resetMag);
      }
    }
  }
}
