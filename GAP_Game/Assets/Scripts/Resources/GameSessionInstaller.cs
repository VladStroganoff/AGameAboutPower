using UnityEngine;
using Zenject;

public class GameSessionInstaller : MonoInstaller
{
    public GameManager gameManager;
    public CameraController camController;
    public CursorController CursorControl;
    public ConstructionController ConControll;
    public ConstructionView ConstructionView;
    public LobbyView LobbyView;
    public UIManager UIManager;
    public GameObject LocalPlayer;
    public LoadController Loader;
    public InventoryController InventoryControl;
    public InventoryView InventoryView;


    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.Bind<IGameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<ILoadController>().FromInstance(Loader);
        Container.Bind<ICameraController>().FromInstance(camController);
        Container.DeclareSignal<ItemLoadedSignal>();

        Container.Bind<ICursorController>().FromInstance(CursorControl);
        Container.Bind<IInventoryController>().FromInstance(InventoryControl);
        Container.Bind<IInventoryView>().FromInstance(InventoryView);
        Container.DeclareSignal<PlayerDresserSpawned>();

        Container.BindSignal<GameStateChangedSignal>()
          .ToMethod<IInventoryController>(x => x.CheckGameState).FromResolve();

        if (LobbyView != null)
        Container.Bind<ILobbyView>().FromInstance(LobbyView);
        Container.Bind<IUIManager>().FromInstance(UIManager);
        Container.Bind<ClientHandle>().AsSingle();

        Container.BindFactory<CameraView, CameraView.Factory>().FromSubContainerResolve().ByNewContextPrefab(LocalPlayer);

        Container.DeclareSignal<GameStateChangedSignal>();

        Container.DeclareSignal<CameraStateSignal>();
        Container.DeclareSignal<CursorClickSignal>();
        Container.DeclareSignal<CursorWorldPosSignal>();

        InitConstruction();

        Container.BindSignal<CameraStateSignal>()
         .ToMethod<ICursorController>(x => x.CheckForRTSMode).FromResolve();

        Container.BindSignal<GameStateChangedSignal>()
          .ToMethod<ICameraController>(x => x.CheckGameState).FromResolve();

        if (LobbyView != null)
            Container.BindSignal<GameStateChangedSignal>()
            .ToMethod<ILobbyView>(x => x.CheckGameState).FromResolve();

        Container.BindSignal<CameraStateSignal>()
            .ToMethod<IUIManager>(x => x.CheckCameraState).FromResolve();

    }


    void InitConstruction()
    {
        Container.Bind<IConstructionView>().FromInstance(ConstructionView);
        
        Container.Bind<IConstructionController>().FromInstance(ConControll);

        Container.DeclareSignal<PickedBuildingSignal>();
        
        Container.DeclareSignal<SendBuildingSignal>();

        Container.BindSignal<CameraStateSignal>().ToMethod<IConstructionView>(x => x.CheckCameraState).FromResolve();

        Container.BindSignal<CursorClickSignal>().ToMethod<IConstructionView>(x => x.ListenForClick).FromResolve();
        Container.BindSignal<CursorWorldPosSignal>().ToMethod<IConstructionView>(x => x.ListenForPos).FromResolve();

        Container.BindSignal<SendBuildingSignal>().ToMethod<IConstructionController>(x => x.SendBuilding).FromResolve();
    }


}