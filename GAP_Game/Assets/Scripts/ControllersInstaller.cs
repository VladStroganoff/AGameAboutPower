using UnityEngine;
using Zenject;

public class ControllersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ICameraController>().To<CameraController>().AsSingle();
        Container.Bind<IGameManager>().To<GameManager>().AsSingle();
        Container.Bind<ICameraView>().To<CameraView>().AsSingle();
    }
}