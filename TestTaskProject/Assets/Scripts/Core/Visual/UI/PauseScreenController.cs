using static EventsProvider;

public class PauseScreenController : ScreenController
{
    private readonly PauseScreenView _pauseView;

    public PauseScreenController(PauseScreenView view, EventManager eventManager)
        : base(view, eventManager)
    {
        _pauseView = view;
    }

    public override void Open()
    {
        base.Open();
        _pauseView.ResumeButton.onClick.AddListener(Resume);
        _pauseView.ExitButton.onClick.AddListener(ExitToMenu);
        _eventManager.Publish(new GamePauseChangedEvent(true));
    }

    public override void Close()
    {
        RemoveListeners();
        _eventManager.Publish(new GamePauseChangedEvent(false));
        base.Close();
    }

    public override void Dispose()
    {
        RemoveListeners();
    }

    private void Resume()
    {
        _eventManager.Publish(new CloseScreenEvent());
    }

    private void ExitToMenu()
    {
        _eventManager.Publish(new LoadSceneEvent(SceneNames.Menu));
    }

    private void RemoveListeners()
    {
        if (_pauseView.ResumeButton != null)
            _pauseView.ResumeButton.onClick.RemoveListener(Resume);

        if (_pauseView.ExitButton != null)
            _pauseView.ExitButton.onClick.RemoveListener(ExitToMenu);
    }
}