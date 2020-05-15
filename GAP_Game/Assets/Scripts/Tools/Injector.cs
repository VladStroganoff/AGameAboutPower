using UnityEngine;
using System.Collections.Generic;

public abstract class Controller : MonoBehaviour
{
}

public class Injector 
{

    private Dictionary<System.Type, Controller> Controllers = new Dictionary<System.Type, Controller>();


    public void Register(Controller controller)
    {
        if (Controllers.ContainsKey(controller.GetType()))
            return;

        Controllers[controller.GetType()] = controller;
    }

    public void Unregister(Controller service)
    {
        if (!Controllers.ContainsKey(service.GetType())) return;
        Controllers.Remove(service.GetType());
    }

    public T GetController<T>() where T : class
    {
        if (!Controllers.ContainsKey(typeof(T)))
        {
            return null;
        }

        return Controllers[typeof(T)] as T;
    }

    public bool ControllerExists<T>() where T : class
    {
        return (Controllers.ContainsKey(typeof(T)));
    }


}