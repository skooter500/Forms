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

using UnityEngine;
using UnityEditor;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// Utilities for the Editor.
    /// </summary>
    public static class EditorHelper
    {
      /// <summary>
      /// Indent level.
      /// </summary>
      public static int IndentLevel
      {
        get { return EditorGUI.indentLevel; }
        set { EditorGUI.indentLevel = value; }
      }

      /// <summary>
      /// Label width.
      /// </summary>
      public static float LabelWidth
      {
        get { return EditorGUIUtility.labelWidth; }
        set { EditorGUIUtility.labelWidth = value; }
      }

      /// <summary>
      /// Field width.
      /// </summary>
      public static float FieldWidth
      {
        get { return EditorGUIUtility.fieldWidth; }
        set { EditorGUIUtility.fieldWidth = value; }
      }

      /// <summary>
      /// GUI enabled?
      /// </summary>
      public static bool Enabled
      {
        get { return GUI.enabled; }
        set { GUI.enabled = value; }
      }

      /// <summary>
      /// GUI changed?
      /// </summary>
      public static bool Changed
      {
        get { return GUI.changed; }
        set { GUI.changed = value; }
      }

      private static HeaderStyle headerStyle;

      static EditorHelper()
      {
        headerStyle = new HeaderStyle();
      }

      /// <summary>
      /// Reset somr GUI variables.
      /// </summary>
      public static void Reset(int indentLevel = 0, float labelWidth = 0.0f, float fieldWidth = 0.0f, bool guiEnabled = true)
      {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 0.0f;
        EditorGUIUtility.fieldWidth = 0.0f;
        GUI.enabled = true;
      }

      /// <summary>
      /// Nice foldout.
      /// </summary>
      public static bool Foldout(bool display, string title)
      {
        Rect rect = GUILayoutUtility.GetRect(16.0f, 22.0f, headerStyle.header);
        GUI.Box(rect, title, headerStyle.header);

        Rect toggleRect = new Rect(rect.x + 4.0f, rect.y + 2.0f, 13.0f, 13.0f);
        if (Event.current.type == EventType.Repaint)
          EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

        Event e = Event.current;
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition) == true)
        {
          display = !display;
          e.Use();
        }

        return display;
      }

      /// <summary>
      /// Nice header.
      /// </summary>
      public static bool Header(ref bool display, bool enabled, string title)
      {
        Rect rect = GUILayoutUtility.GetRect(16.0f, 22.0f, headerStyle.header);
        GUI.Box(rect, title, headerStyle.header);

        Rect toggleRect = new Rect(rect.x + 4.0f, rect.y + 4.0f, 13.0f, 13.0f);
        if (Event.current.type == EventType.Repaint)
          headerStyle.headerCheckbox.Draw(toggleRect, false, false, enabled, false);

        Event e = Event.current;
        if (e.type == EventType.MouseDown)
        {
          if (toggleRect.Contains(e.mousePosition) == true)
          {
            enabled = !enabled;
            e.Use();
            GUI.changed = true;
          }
          else if (rect.Contains(e.mousePosition) == true)
          {
            display = !display;
            e.Use();
            GUI.changed = true;
          }
        }

        return enabled;
      }

      /// <summary>
      /// Button with confirmation.
      /// </summary>
      public static bool ConfirmationButton(string buttonText, Color buttonColor, string dialogTitle, string dialogMessage)
      {
        bool confirmation = false;

        GUI.color = buttonColor;

        if (GUILayout.Button(buttonText) == true)
          confirmation = EditorUtility.DisplayDialog(dialogTitle, dialogMessage, @"OK", @"Cancel");

        GUI.color = UnityEngine.Color.white;

        return confirmation;
      }

      /// <summary>
      /// Line separator.
      /// </summary>
      public static void Line()
      {
        EditorGUILayout.Separator();

        GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1.0f));
      }

      /// <summary>
      /// Separator.
      /// </summary>
      public static void Separator()
      {
        EditorGUILayout.Separator();
      }

      /// <summary>
      /// Begin vertical.
      /// </summary>
      public static void BeginVertical()
      {
        EditorGUILayout.BeginVertical();
      }

      /// <summary>
      /// End vertical.
      /// </summary>
      public static void EndVertical()
      {
        EditorGUILayout.EndVertical();
      }

      /// <summary>
      /// Begin horizontal.
      /// </summary>
      public static void BeginHorizontal()
      {
        EditorGUILayout.BeginHorizontal();
      }

      /// <summary>
      /// End horizontal.
      /// </summary>
      public static void EndHorizontal()
      {
        EditorGUILayout.EndHorizontal();
      }

      /// <summary>
      /// Flexible space.
      /// </summary>
      public static void FlexibleSpace()
      {
        GUILayout.FlexibleSpace();
      }

      /// <summary>
      /// Label.
      /// </summary>
      public static void Label(string label, string tooltip = default(string))
      {
        EditorGUILayout.LabelField(new GUIContent(label, tooltip));
      }

      /// <summary>
      /// Button.
      /// </summary>
      public static bool Button(string label, string tooltip = default(string), GUIStyle style = null)
      {
        return GUILayout.Button(new GUIContent(label, tooltip), style != null ? style : GUI.skin.button);
      }

      /// <summary>
      /// Toggle with reset.
      /// </summary>
      public static bool Toggle(string label, string tooltip, bool value, bool resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.Toggle(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Toggle.
      /// </summary>
      public static bool Toggle(string label, string tooltip, bool value)
      {
        return EditorGUILayout.Toggle(new GUIContent(label, tooltip), value);
      }

      /// <summary>
      /// Toggle with reset.
      /// </summary>
      public static bool Toggle(string label, bool value, bool resetValue)
      {
        return Toggle(label, string.Empty, value, resetValue);
      }

      /// <summary>
      /// Enum popup with reset.
      /// </summary>
      public static Enum EnumPopup(string label, string tooltip, Enum selected, Enum resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          selected = EditorGUILayout.EnumPopup(new GUIContent(label, tooltip), selected);

          if (ResetButton(resetValue) == true)
            selected = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return selected;
      }

      /// <summary>
      /// Enum popup with reset.
      /// </summary>
      public static Enum EnumPopup(string label, Enum selected, Enum resetValue)
      {
        return EnumPopup(label, string.Empty, selected, resetValue);
      }

      /// <summary>
      /// Slider with reset.
      /// </summary>
      public static float Slider(string label, string tooltip, float value, float minValue, float maxValue, float resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.Slider(new GUIContent(label, tooltip), value, minValue, maxValue);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Slider with reset.
      /// </summary>
      public static float Slider(string label, float value, float minValue, float maxValue, float resetValue)
      {
        return Slider(label, string.Empty, value, minValue, maxValue, resetValue);
      }

      /// <summary>
      /// Float field with reset.
      /// </summary>
      public static float Float(string label, string tooltip, float value, float resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.FloatField(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Float field with reset.
      /// </summary>
      public static float Float(string label, float value, float resetValue)
      {
        return Float(label, string.Empty, value, resetValue);
      }

      /// <summary>
      /// Int field with reset.
      /// </summary>
      public static int IntSlider(string label, string tooltip, int value, int minValue, int maxValue, int resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.IntSlider(new GUIContent(label, tooltip), value, minValue, maxValue);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Int field with reset.
      /// </summary>
      public static int IntSlider(string label, int value, int minValue, int maxValue, int resetValue)
      {
        return IntSlider(label, string.Empty, value, minValue, maxValue, resetValue);
      }

      /// <summary>
      /// Int field with reset.
      /// </summary>
      public static int Int(string label, string tooltip, int value, int resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.IntField(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Int field with reset.
      /// </summary>
      public static int Int(string label, int value, int resetValue)
      {
        return Int(label, value, resetValue);
      }

      /// <summary>
      /// Int popup field with reset.
      /// </summary>
      public static int IntPopup(string label, int value, string[] names, int[] values, int resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.IntPopup(label, value, names, values);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Min-max slider with reset.
      /// </summary>
      public static void MinMaxSlider(string label, string tooltip, ref float minValue, ref float maxValue, float minLimit, float maxLimit, float defaultMinLimit, float defaultMaxLimit)
      {
        EditorGUILayout.BeginHorizontal();
        {
          EditorGUILayout.MinMaxSlider(new GUIContent(label, tooltip), ref minValue, ref maxValue, minLimit, maxLimit);

          if (GUILayout.Button(@"R", GUILayout.Width(18.0f), GUILayout.Height(17.0f)) == true)
          {
            minValue = defaultMinLimit;
            maxValue = defaultMaxLimit;
          }
        }
        EditorGUILayout.EndHorizontal();
      }

      /// <summary>
      /// Min-max slider with reset.
      /// </summary>
      public static void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, float defaultMinLimit, float defaultMaxLimit)
      {
        MinMaxSlider(label, ref minValue, ref maxValue, minLimit, maxLimit, defaultMinLimit, defaultMaxLimit);
      }

      /// <summary>
      /// Color field with reset.
      /// </summary>
      public static Color Color(string label, string tooltip, Color value, Color resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.ColorField(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Color field with reset.
      /// </summary>
      public static Color Color(string label, Color value, Color resetValue)
      {
        return Color(label, value, resetValue);
      }

      /// <summary>
      /// Animation curve.
      /// </summary>
      public static AnimationCurve Curve(string label, string tooltip, AnimationCurve value)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.CurveField(new GUIContent(label, tooltip), value);

          if (ResetButton() == true)
            value = new AnimationCurve(new Keyframe(1.0f, 0.0f, 0.0f, 0.0f), new Keyframe(0.0f, 1.0f, 0.0f, 0.0f));
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Animation curve.
      /// </summary>
      public static AnimationCurve Curve(string label, AnimationCurve curve)
      {
        return Curve(label, string.Empty, curve);
      }

      /// <summary>
      /// Vector2 field with reset.
      /// </summary>
      public static Vector2 Vector2(string label, string tooltip, Vector2 value, Vector2 resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.Vector2Field(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Vector2 field with reset.
      /// </summary>
      public static Vector2 Vector2(string label, Vector2 value, Vector2 resetValue)
      {
        return Vector2(label, string.Empty, value, resetValue);
      }

      /// <summary>
      /// Vector3 field with reset.
      /// </summary>
      public static Vector3 Vector3(string label, string tooltip, Vector3 value, Vector3 resetValue)
      {
        EditorGUILayout.BeginHorizontal();
        {
          value = EditorGUILayout.Vector3Field(new GUIContent(label, tooltip), value);

          if (ResetButton(resetValue) == true)
            value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return value;
      }

      /// <summary>
      /// Vector3 field with reset.
      /// </summary>
      public static Vector3 Vector3(string label, Vector3 value, Vector3 resetValue)
      {
        return Vector3(label, string.Empty, value, resetValue);
      }

      /// <summary>
      /// Layermask field with reset.
      /// </summary>
      public static LayerMask LayerMask(string label, LayerMask layerMask, int resetValue)
      {
        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();

        for (int i = 0; i < 32; ++i)
        {
          string layerName = UnityEngine.LayerMask.LayerToName(i);
          if (string.IsNullOrEmpty(layerName) == false)
          {
            layers.Add(layerName);
            layerNumbers.Add(i);
          }
        }

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; ++i)
        {
          if (((1 << layerNumbers[i]) & layerMask.value) > 0)
            maskWithoutEmpty |= (1 << i);
        }

        EditorGUILayout.BeginHorizontal();
        {
          maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
          int mask = 0;
          for (int i = 0; i < layerNumbers.Count; ++i)
          {
            if ((maskWithoutEmpty & (1 << i)) > 0)
              mask |= (1 << layerNumbers[i]);
          }

          layerMask.value = mask;

          if (ResetButton(resetValue) == true)
            layerMask.value = resetValue;
        }
        EditorGUILayout.EndHorizontal();

        return layerMask;
      }

      /// <summary>
      /// Creates a texture for use in the Editor.
      /// </summary>
      public static Texture2D MakeTexture(int width, int height, Color color)
      {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; ++i)
          pixels[i] = color;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();

        return result;
      }

      /// <summary>
      /// Marks object target as dirty.
      /// </summary>
      public static void SetDirty(UnityEngine.Object target)
      {
        EditorUtility.SetDirty(target);
      }

      private static bool ResetButton<T>(T resetValue)
      {
        return GUILayout.Button(new GUIContent(@"R", string.Format("Reset to '{0}'.", resetValue)), GUILayout.Width(18.0f), GUILayout.Height(17.0f));
      }

      private static bool ResetButton()
      {
        return GUILayout.Button(@"R", GUILayout.Width(18.0f), GUILayout.Height(17.0f));
      }

      private class HeaderStyle
      {
        public GUIStyle header = @"ShurikenModuleTitle";
        public GUIStyle headerCheckbox = @"ShurikenCheckMark";

        internal HeaderStyle()
        {
          header.font = new GUIStyle(@"Label").font;
          header.border = new RectOffset(15, 7, 4, 4);
          header.fixedHeight = 22;
          header.contentOffset = new Vector2(20.0f, -2.0f);
        }
      }
    }
  }
}