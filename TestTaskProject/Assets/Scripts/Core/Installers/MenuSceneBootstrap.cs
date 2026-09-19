using UnityEngine;
using Zenject;
using static EventsProvider;

public class MenuSceneBootstrap : IInitializable
{
    private readonly EventManager _eventManager;

    public MenuSceneBootstrap(EventManager eventManager, ScreenCatalog screenCatalog)
    {
        _eventManager = eventManager;
        if (screenCatalog == null)
            throw new System.ArgumentNullException(nameof(screenCatalog));
    }

    public void Initialize()
    {
        var camera = Camera.main;
        if (camera != null)
        {
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(0.09f, 0.08f, 0.11f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        _eventManager.Publish(new OpenScreenEvent(ScreenIds.Menu));
    }
}