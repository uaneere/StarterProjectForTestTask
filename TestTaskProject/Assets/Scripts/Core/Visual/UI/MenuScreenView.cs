using Gameplay.Presentation;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreenView : ScreenView
{
    public Button PlayButton { get; private set; }
    public Button QuitButton { get; private set; }

    public void Build()
    {
        var rect = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        UiWidgetFactory.Stretch(rect);

        var background = gameObject.AddComponent<Image>();
        background.sprite = VisualCatalog.BackgroundMenu;
        background.preserveAspect = false;
        background.raycastTarget = true;

        PlayButton = UiWidgetFactory.CreateImageButton(transform, "Start", VisualCatalog.StartButton, new Vector2(360, 110));
        var playRect = PlayButton.GetComponent<RectTransform>();
        playRect.anchorMin = playRect.anchorMax = playRect.pivot = new Vector2(0.5f, 0.5f);
        playRect.anchoredPosition = new Vector2(0f, 40f);

        QuitButton = UiWidgetFactory.CreateImageButton(transform, "Exit", VisualCatalog.ExitButton, new Vector2(360, 110));
        var quitRect = QuitButton.GetComponent<RectTransform>();
        quitRect.anchorMin = quitRect.anchorMax = quitRect.pivot = new Vector2(0.5f, 0.5f);
        quitRect.anchoredPosition = new Vector2(0f, -90f);
    }

    public override ScreenController Construct(EventManager eventManager)
    {
        return new MenuScreenController(this, eventManager);
    }
}