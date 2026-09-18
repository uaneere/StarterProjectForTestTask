using UnityEngine;

public class ScreenView : MonoBehaviour
{
    public string Id => _id;

    private string _id;

    public void Configure(string id)
    {
        _id = id;
    }

    public virtual ScreenController Construct(EventManager eventManager)
    {
        return new ScreenController(this, eventManager);
    }

    public void OpenScreen()
    {
        gameObject.SetActive(true);
    }

    public void CloseScreen()
    {
        gameObject.SetActive(false);
    }
}