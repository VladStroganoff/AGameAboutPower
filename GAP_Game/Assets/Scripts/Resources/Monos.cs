using UnityEngine;
using Zenject;

public class Monos : MonoInstaller
{
    public GameManager gameManager;
    public CameraController camController;
    public ConstructionController ConControll;
    public CursorController CursorControl;

    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().FromInstance(gameManager);
        Container.Bind<ICameraController>().FromInstance(camController);
        Container.Bind<IConstrcuController>().FromInstance(ConControll);
        Container.Bind<ICursorController>().FromInstance(CursorControl);
    }
}