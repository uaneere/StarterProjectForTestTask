using static EventsProvider;

public class MenuScreenController : ScreenController
{
    private readonly MenuScreenView _menuView;

    public MenuScreenController(MenuScreenView view, EventManager eventManager)
        : base(view, eventManager)
    {
        _menuView = view;
    }

    public override void Open()
    {
        base.Open();
        _menuView.PlayButton.onClick.AddListener(StartGame);
        _menuView.QuitButton.onClick.AddListener(Quit);
    }

    public override void Close()
    {
        RemoveListeners();
        base.Close();
    }

    public override void Dispose()
    {
        RemoveListeners();
    }

    private void StartGame()
    {
        _eventManager.Publish(new LoadSceneEvent(SceneNames.Game));
    }

    private void Quit()
    {
        _eventManager.Publish(new QuitApplicationEvent());
    }

    private void RemoveListeners()
    {
        if (_menuView.PlayButton != null)
            _menuView.PlayButton.onClick.RemoveListener(StartGame);

        if (_menuView.QuitButton != null)
            _menuView.QuitButton.onClick.RemoveListener(Quit);
    }
}