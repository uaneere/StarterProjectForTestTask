using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static EventsProvider;

public class SceneLoader : IInitializable, IDisposable
{
    private readonly EventManager _eventManager;
    private readonly UIController _uiController;

    public SceneLoader(EventManager eventManager, UIController uiController)
    {
        _eventManager = eventManager;
        _uiController = uiController;
    }

    public void Initialize()
    {
        _eventManager.Subscribe<LoadSceneEvent>(Load);
        _eventManager.Subscribe<QuitApplicationEvent>(Quit);
    }

    public void Dispose()
    {
        _eventManager.Unsubscribe<LoadSceneEvent>(Load);
        _eventManager.Unsubscribe<QuitApplicationEvent>(Quit);
    }

    private void Load(LoadSceneEvent loadEvent)
    {
        if (string.IsNullOrEmpty(loadEvent.SceneName))
            return;

        if (SceneManager.GetActiveScene().name == loadEvent.SceneName)
            return;

        Time.timeScale = 1f;
        _uiController.Clear(destroyImmediate: true);
        SceneManager.LoadScene(loadEvent.SceneName, LoadSceneMode.Single);
    }

    private static void Quit(QuitApplicationEvent _)
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}