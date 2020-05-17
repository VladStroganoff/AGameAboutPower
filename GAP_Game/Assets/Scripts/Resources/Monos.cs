using UnityEngine;
using Zenject;

public class Monos : MonoInstaller
{
    public GameManager gameManager;

    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().FromInstance(gameManager);
    }
}