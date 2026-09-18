public class ScreenController
{
    protected readonly ScreenView _view;
    protected readonly EventManager _eventManager;

    public ScreenController(ScreenView view, EventManager eventManager)
    {
        _view = view;
        _eventManager = eventManager;
    }

    public virtual void Open()
    {
        _view.OpenScreen();
    }

    public virtual void Close()
    {
        if (_view != null)
            _view.CloseScreen();
    }

    public virtual void Dispose()
    {
    }
}