using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{

    public CameraView camView;

    public override void InstallBindings()
    {
        Container.Bind<ICameraView>().FromInstance(camView);
        Container.BindSignal<CameraStateSignal>().ToMethod<CameraView>(x => x.CheckCameraState).FromResolve();
    }
}