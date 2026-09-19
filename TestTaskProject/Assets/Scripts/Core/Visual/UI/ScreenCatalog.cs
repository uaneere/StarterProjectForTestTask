using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScreenCatalog : IInitializable
{
    private readonly UIController _uiController;

    public ScreenCatalog(UIController uiController)
    {
        _uiController = uiController;
    }

    public void Initialize()
    {
        var canvas = _uiController.GetComponent<Canvas>() ?? _uiController.GetComponentInChildren<Canvas>(true);
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 500;
        }

        var scaler = _uiController.GetComponent<CanvasScaler>() ?? _uiController.GetComponentInChildren<CanvasScaler>(true);
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.dynamicPixelsPerUnit = 4f;
        }

        _uiController.RegisterFactory(ScreenIds.Menu, parent => Create<MenuScreenView>(parent, ScreenIds.Menu, view => view.Build()));
        _uiController.RegisterFactory(ScreenIds.Pause, parent => Create<PauseScreenView>(parent, ScreenIds.Pause, view => view.Build()));
    }

    private static TView Create<TView>(Transform parent, string id, System.Action<TView> build)
        where TView : ScreenView
    {
        var go = new GameObject(id, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        UiWidgetFactory.Stretch(go.GetComponent<RectTransform>());
        var view = go.AddComponent<TView>();
        view.Configure(id);
        build(view);
        go.SetActive(true);
        return view;
    }
}