using UnityEngine;
using Zenject;

public class Monos : MonoInstaller
{
    public GameManager gameManager;
    public CameraController camController;
    public ConstructionController ConControll;
    public CursorController CursorControl;
    public ConstructionView ConstructionView;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.Bind<IGameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<ICameraController>().FromInstance(camController);
        Container.Bind<IConstrcuController>().FromInstance(ConControll);
        Container.Bind<ICursorController>().FromInstance(CursorControl);
        Container.Bind<IConstructionView>().FromInstance(ConstructionView);

        Container.DeclareSignal<PickedBuildingSignal>();
        Container.DeclareSignal<BuildBuildingSignal>();

        Container.BindSignal<PickedBuildingSignal>()
            .ToMethod<ConstructionView>(x => x.PickBuilding).FromResolve();

        Container.BindSignal<BuildBuildingSignal>()
           .ToMethod<ConstructionView>(x => x.BuildBuilding).FromResolve();
    }
}