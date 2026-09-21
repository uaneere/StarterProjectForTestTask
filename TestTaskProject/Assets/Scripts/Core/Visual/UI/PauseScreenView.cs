using Gameplay.Presentation;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenView : ScreenView
{
    public Button ResumeButton { get; private set; }
    public Button ExitButton { get; private set; }

    public void Build()
    {
        var rect = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        UiWidgetFactory.Stretch(rect);

        var dim = gameObject.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.35f);
        dim.raycastTarget = true;

        var panel = new GameObject("Panel");
        panel.transform.SetParent(transform, false);
        var panelImage = panel.AddComponent<Image>();
        panelImage.sprite = VisualCatalog.BackgroundPause;
        panelImage.preserveAspect = true;
        panelImage.raycastTarget = true;
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(720, 480);

        ExitButton = UiWidgetFactory.CreateImageButton(panel.transform, "Menu", VisualCatalog.MenuButton, new Vector2(280, 100));
        var menuRect = ExitButton.GetComponent<RectTransform>();
        menuRect.anchorMin = menuRect.anchorMax = menuRect.pivot = new Vector2(0.5f, 0.5f);
        menuRect.anchoredPosition = Vector2.zero;

        ResumeButton = UiWidgetFactory.CreateImageButton(panel.transform, "Close", VisualCatalog.CloseButton, new Vector2(72, 72));
        var closeRect = ResumeButton.GetComponent<RectTransform>();
        closeRect.anchorMin = closeRect.anchorMax = closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-18f, -46f);
    }

    public override ScreenController Construct(EventManager eventManager)
    {
        return new PauseScreenController(this, eventManager);
    }
}