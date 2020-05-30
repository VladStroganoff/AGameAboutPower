using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Scripties", menuName = "Installers/Scripties")]
public class Scripties : ScriptableObjectInstaller<Scripties>
{
    public override void InstallBindings()
    {
        Container.Bind<IWorldModel>().To<WorldModel>().AsSingle();
    }
}