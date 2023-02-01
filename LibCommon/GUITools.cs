﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

namespace LibCommon
{
    /// <summary>
    /// Common tools for building dynamic GUIs from code.
    /// </summary>
    public static class GUITools
    {
        public static readonly Color DEFAULT_PANEL_BORDER_COLOR = new Color(121f / 255, 125f / 255, 245f / 255, 1f);
        public static readonly Color DEFAULT_PANEL_COLOR = new Color(231f / 255, 227f / 255, 243f / 255, 1f);
        public static readonly Color DEFAULT_BOX_COLOR = new Color(121f / 255, 125f / 255, 245f / 255, 1f);
        public static readonly Color DEFAULT_BOX_COLOR_HOVER = new Color(161f / 255, 165f / 255, 245f / 255, 1f);


        /// <summary>
        /// Check if a key is pressed in the current frame while also not being in an input field.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool IsKeyDown(KeyCode keyCode)
        {
            GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            return (currentSelectedGameObject == null || !currentSelectedGameObject.TryGetComponent<InputField>(out _))
                && Input.GetKeyDown(keyCode);
        }

        /// <summary>
        /// Get the mouse position converted to a screen-centered coordinate system used by the canvas.
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMouseCanvasPos()
        {
            var mousePos = Input.mousePosition;
            return new Vector2(-Screen.width / 2 + mousePos.x, -Screen.height / 2 + mousePos.y);
        }

        /// <summary>
        /// Is the given point within the given rectangle.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool Within(RectTransform rt, Vector2 vec)
        {
            var x = rt.localPosition.x - rt.sizeDelta.x / 2;
            var y = rt.localPosition.y - rt.sizeDelta.y / 2;
            var x2 = x + rt.sizeDelta.x;
            var y2 = y + rt.sizeDelta.y;
            return x <= vec.x && vec.x <= x2 && y <= vec.y && vec.y <= y2;
        }

        /// <summary>
        /// Is the given point within the parent and within rt relative to parent?
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="rt"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool Within(RectTransform parent, RectTransform rt, Vector2 vec)
        {
            var x = parent.localPosition.x + rt.localPosition.x - rt.sizeDelta.x / 2;
            var y = parent.localPosition.y + rt.localPosition.y - rt.sizeDelta.y / 2;
            var x2 = x + rt.sizeDelta.x;
            var y2 = y + rt.sizeDelta.y;
            return x <= vec.x && vec.x <= x2 && y <= vec.y && vec.y <= y2;
        }

        /// <summary>
        /// Load a file as texture.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Texture2D LoadPNG(string filename)
        {
            Texture2D tex = new Texture2D(100, 200);
            tex.LoadImage(File.ReadAllBytes(filename));

            return tex;
        }

        /// <summary>
        /// Create a text label with a color background around it.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="textColor"></param>
        /// <returns></returns>
        public static GameObject CreateBox(GameObject parent, string name, string text, int fontSize, Color backgroundColor, Color textColor)
        {
            var box = new GameObject(name);
            box.transform.SetParent(parent.transform);
            var img = box.AddComponent<Image>();
            img.color = backgroundColor;

            var textGo = new GameObject(name + "_Text");
            textGo.transform.SetParent(box.transform);

            var txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = fontSize;
            txt.color = textColor;
            txt.resizeTextForBestFit = false;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.text = text;

            var rect = textGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(txt.preferredWidth, txt.preferredHeight);

            var rectbox = box.GetComponent<RectTransform>();
            rectbox.sizeDelta = new Vector2(rect.sizeDelta.x + 4, rect.sizeDelta.y + 4);

            return box;
        }

        /// <summary>
        /// Creates a plain text label.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameObject CreateText(GameObject parent, string name, string text, int fontSize, Color color)
        {
            var textGo = new GameObject(name + "_Text");
            textGo.transform.SetParent(parent.transform);

            var txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = fontSize;
            txt.color = color;
            txt.resizeTextForBestFit = false;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.text = text;

            var rect = textGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(txt.preferredWidth, txt.preferredHeight);

            return textGo;
        }

        /// <summary>
        /// Returns the preferred width of a Text component inside the GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetPreferredWidth(GameObject go)
        {
            return Mathf.CeilToInt(go.GetComponentInChildren<Text>().preferredWidth);
        }

        /// <summary>
        /// Sets the position of the GameObject.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetLocalPosition(GameObject go, float x, float y)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.localPosition = new Vector2(x, y);
        }
    }
}