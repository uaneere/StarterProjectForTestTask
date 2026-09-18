public static class EventsProvider
{
    public class OpenScreenEvent
    {
        public readonly string ScreenId;

        public OpenScreenEvent(string screenId)
        {
            ScreenId = screenId;
        }
    }

    public class CloseScreenEvent
    {
    }

    public class LoadSceneEvent
    {
        public readonly string SceneName;

        public LoadSceneEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }

    public class GamePauseChangedEvent
    {
        public readonly bool IsPaused;

        public GamePauseChangedEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }

    public class CabinetSlotClickedEvent
    {
        public readonly int Index;

        public CabinetSlotClickedEvent(int index)
        {
            Index = index;
        }
    }

    public class FinishPotionRequestedEvent
    {
    }

    public class QuitApplicationEvent
    {
    }
}