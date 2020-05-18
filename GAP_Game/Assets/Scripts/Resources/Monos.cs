using UnityEngine;
using Zenject;

public class Monos : MonoInstaller
{
    public GameManager gameManager;
    public CameraController cameraController;

    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().FromInstance(gameManager);
        Container.Bind<ICameraController>().FromInstance(cameraController);
    }
}