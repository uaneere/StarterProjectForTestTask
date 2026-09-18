using System;
using UnityEngine;
using UnityEngine.UI;

public static class UiWidgetFactory
{
    public static Font Font { get; private set; }

    public static Font ResolveFont()
    {
        if (Font != null)
            return Font;

        Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (Font == null)
            Font = Font.CreateDynamicFontFromOSFont("Arial", 16);

        return Font;
    }

    public static Text CreateLabel(Transform parent, string text, int size, TextAnchor align, float height = 0f)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var label = go.AddComponent<Text>();
        label.font = ResolveFont();
        label.text = text;
        label.fontSize = size;
        label.color = Color.white;
        label.alignment = align;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = height > 0f ? height : size + 16f;
        layout.flexibleWidth = 1f;
        return label;
    }

    public static Button CreateButton(Transform parent, string text, Action click, float height = 56f)
    {
        var go = new GameObject(text);
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.color = new Color(0.32f, 0.24f, 0.16f, 1f);
        image.raycastTarget = true;

        var layout = go.AddComponent<LayoutElement>();
        layout.minHeight = height;
        layout.preferredHeight = height;
        layout.flexibleWidth = 1f;

        var button = go.AddComponent<Button>();
        if (click != null)
            button.onClick.AddListener(() => click());

        var labelGo = new GameObject("Text");
        labelGo.transform.SetParent(go.transform, false);
        var label = labelGo.AddComponent<Text>();
        label.font = ResolveFont();
        label.text = text;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 22;
        label.raycastTarget = false;
        Stretch(label.rectTransform);
        return button;
    }

    public static Button CreateImageButton(Transform parent, string name, Sprite sprite, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = true;
        var button = go.AddComponent<Button>();
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        return button;
    }

    public static Image CreateImage(Transform parent, string name, Sprite sprite, bool raycast = false)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = raycast;
        Stretch(image.rectTransform);
        return image;
    }

    public static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}