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
        Container.DeclareSignal<CameraStateSignal>();
        Container.DeclareSignal<GameStateChangedSignal>();
        Container.DeclareSignal<CursorClickSignal>();
        Container.DeclareSignal<CursorWorldPosSignal>();


        Container.BindSignal<CursorClickSignal>()
            .ToMethod<ConstructionController>(x => x.ListenForClick).FromResolve();

        Container.BindSignal<CursorWorldPosSignal>()
            .ToMethod<ConstructionController>(x => x.ListenForPos).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
            .ToMethod<CameraController>(x => x.CheckGameState).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
            .ToMethod<LobbyView>(x => x.CheckState).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
           .ToMethod<CameraController>(x => x.CheckGameState).FromResolve();

        Container.BindSignal<PickedBuildingSignal>()
            .ToMethod<ConstructionView>(x => x.PickBuilding).FromResolve();

        Container.BindSignal<BuildBuildingSignal>()
         .ToMethod<ConstructionView>(x => x.BuildBuilding).FromResolve();

        Container.BindSignal<CameraStateSignal>()
            .ToMethod<ConstructionController>(x => x.CheckCameraState).FromResolve();

        //Container.BindSignal<CameraStateSignal>()
        //   .ToMethod<CameraView>(x => x.CheckCameraState).FromResolve();

        Container.BindSignal<CameraStateSignal>()
           .ToMethod<UIManager>(x => x.CheckCameraState).FromResolve();

        Container.BindSignal<CameraStateSignal>()
         .ToMethod<CursorController>(x => x.CheckForRTSMode).FromResolve();

      
    }
}