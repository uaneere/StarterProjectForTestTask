using Gameplay;
using Gameplay.Brewing;
using Gameplay.Data;
using Gameplay.Inventory;
using Gameplay.Minigame;
using Gameplay.Player;
using Zenject;

public class CoreSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameContent>().AsSingle();
        Container.Bind<HandInventory>().AsSingle();
        Container.Bind<Cabinet>().AsSingle();
        Container.Bind<Cauldron>().AsSingle();
        Container.Bind<IngredientProcessor>().AsSingle();
        Container.Bind<PlayerHealth>().AsSingle();
        Container.BindInterfacesTo<GameplayBootstrap>().AsSingle();
    }
}