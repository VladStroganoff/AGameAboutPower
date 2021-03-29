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



public class Greeter
{
    readonly string _message;

    public Greeter(string message)
    {
        _message = message;
    }

    public void DisplayGreeting()
    {
        Debug.Log(_message);
    }
}
