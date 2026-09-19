using Gameplay.Presentation;
using Zenject;

namespace Gameplay
{
    public class GameplayBootstrap : IInitializable
    {
        private readonly DiContainer _container;

        public GameplayBootstrap(DiContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.InstantiateComponentOnNewGameObject<GameWorld>("GameWorld");
        }
    }
}