using UnityEngine;
using Zenject;

public class GameSessionInstaller : MonoInstaller
{
    public GameManager gameManager;
    public CameraController camController;
    public ConstructionController ConControll;
    public CursorController CursorControl;
    public ConstructionView ConstructionView;
    public LobbyView LobbyView;
    public UIManager UIManager;
    public GameObject LocalPlayer;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.Bind<IGameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<ICameraController>().FromInstance(camController);
        Container.Bind<IConstructionController>().FromInstance(ConControll);
        Container.Bind<ICursorController>().FromInstance(CursorControl);
        Container.Bind<IConstructionView>().FromInstance(ConstructionView);
        Container.Bind<ILobbyView>().FromInstance(LobbyView);
        Container.Bind<IUIManager>().FromInstance(UIManager);

        Container.BindFactory<CameraView, CameraView.Factory>().FromSubContainerResolve().ByNewContextPrefab(LocalPlayer);

        Container.DeclareSignal<GameStateChangedSignal>();
        Container.DeclareSignal<PickedBuildingSignal>();
        Container.DeclareSignal<BuildBuildingSignal>();
        Container.DeclareSignal<CameraStateSignal>();
        Container.DeclareSignal<CursorClickSignal>();
        Container.DeclareSignal<CursorWorldPosSignal>();


        Container.BindSignal<CursorClickSignal>()
            .ToMethod<IConstructionController>(x => x.ListenForClick).FromResolve();

        Container.BindSignal<CursorWorldPosSignal>()
            .ToMethod<IConstructionController>(x => x.ListenForPos).FromResolve();

        Container.BindSignal<PickedBuildingSignal>()
            .ToMethod<ConstructionView>(x => x.PickBuilding).FromResolve();

        Container.BindSignal<BuildBuildingSignal>()
         .ToMethod<ConstructionView>(x => x.BuildBuilding).FromResolve();

        Container.BindSignal<CameraStateSignal>()
            .ToMethod<IConstructionController>(x => x.CheckCameraState).FromResolve();

        Container.BindSignal<CameraStateSignal>()
         .ToMethod<ICursorController>(x => x.CheckForRTSMode).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
          .ToMethod<ICameraController>(x => x.CheckGameState).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
            .ToMethod<ILobbyView>(x => x.CheckGameState).FromResolve();

        Container.BindSignal<CameraStateSignal>()
            .ToMethod<IUIManager>(x => x.CheckCameraState).FromResolve();

    }





}