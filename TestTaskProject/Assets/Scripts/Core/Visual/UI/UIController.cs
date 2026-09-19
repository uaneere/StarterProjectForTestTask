using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static EventsProvider;

public class UIController : MonoBehaviour
{
    public ScreenController CurrentScreen => _currentScreen;

    [SerializeField] private ScreenView[] _views = new ScreenView[0];

    private readonly Dictionary<string, Func<Transform, ScreenView>> _factories = new();
    private ScreenController _currentScreen;
    private ScreenView _currentView;
    private EventManager _eventManager;
    private DiContainer _container;

    [Inject]
    public void Initialize(EventManager eventManager, DiContainer container)
    {
        if (_eventManager != null)
        {
            _eventManager.Unsubscribe<OpenScreenEvent>(OpenScreen);
            _eventManager.Unsubscribe<CloseScreenEvent>(OnCloseScreen);
        }

        _eventManager = eventManager;
        _container = container;
        _eventManager.Subscribe<OpenScreenEvent>(OpenScreen);
        _eventManager.Subscribe<CloseScreenEvent>(OnCloseScreen);
    }

    public void RegisterFactory(string id, Func<Transform, ScreenView> factory)
    {
        _factories[id] = factory;
    }

    public void RegisterView(ScreenView view)
    {
        if (view == null)
            return;

        var views = new List<ScreenView>(_views);
        if (!views.Contains(view))
            views.Add(view);

        _views = views.ToArray();
    }

    public void OpenScreen(OpenScreenEvent screenEvent)
    {
        if (string.IsNullOrEmpty(screenEvent.ScreenId)) return;
        if (_currentView != null && _currentView.Id == screenEvent.ScreenId) return;

        Clear();

        if (_factories.TryGetValue(screenEvent.ScreenId, out var factory))
        {
            _currentView = factory(transform);
            _container.InjectGameObject(_currentView.gameObject);
            _currentScreen = _currentView.Construct(_eventManager);
            _currentScreen.Open();
            return;
        }

        foreach (var view in _views)
        {
            if (view == null || view.Id != screenEvent.ScreenId) continue;

            _currentView = Instantiate(view, transform);
            _container.InjectGameObject(_currentView.gameObject);
            _currentScreen = _currentView.Construct(_eventManager);
            _currentScreen.Open();
            return;
        }

        Debug.LogWarning($"Screen '{screenEvent.ScreenId}' is not registered.", this);
    }

    private void OnCloseScreen(CloseScreenEvent _)
    {
        Clear();
    }

    public void Clear(bool destroyImmediate = false)
    {
        if (_currentScreen != null)
        {
            _currentScreen.Close();
            _currentScreen.Dispose();
        }

        if (_currentView != null)
        {
            if (destroyImmediate)
                DestroyImmediate(_currentView.gameObject);
            else
                Destroy(_currentView.gameObject);
        }

        _currentScreen = null;
        _currentView = null;
    }

    private void OnDestroy()
    {
        if (_eventManager != null)
        {
            _eventManager.Unsubscribe<OpenScreenEvent>(OpenScreen);
            _eventManager.Unsubscribe<CloseScreenEvent>(OnCloseScreen);
        }

        Clear();
    }
}