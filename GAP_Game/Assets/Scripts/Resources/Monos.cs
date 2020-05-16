using UnityEngine;
using Zenject;

public class Monos : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().To<GameManager>().AsSingle();
        Container.Bind<ICameraView>().To<CameraView>().AsSingle();
    }
}