namespace Gameplay.Player
{
    public class PlayerHealth
    {
        public const int MaxHearts = 3;

        public int Hearts { get; private set; } = MaxHearts;
        public bool IsDead => Hearts <= 0;

        public void Damage()
        {
            if (Hearts > 0)
                Hearts--;
        }

        public void Reset()
        {
            Hearts = MaxHearts;
        }
    }
}