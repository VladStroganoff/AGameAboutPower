using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedAnimator : MonoBehaviour
{
    Animator Animations;

    void Start()
    {
        Animations = GetComponent<Animator>();
    }

    public void Set(NetAnimator animator)
    {
        foreach (NetAnimationParam component in animator.Parameters)
        {
            if (component is BoolParam)
            {
                BoolParam boolio = component as BoolParam;
                Animations.SetBool(boolio.Name, boolio.State);
            }
            else if (component is FloatParam)
            {
                FloatParam floatio = component as FloatParam;
                Animations.SetFloat(floatio.Name, floatio.State);
            }
            else if (component is IntigerParam)
            {
                IntigerParam intelio = component as IntigerParam;
                Animations.SetInteger(intelio.Name, intelio.State);
            }
            else if (component is TriggerParam)
            {
                TriggerParam triggero = component as TriggerParam;
                Animations.SetTrigger(triggero.Name);
            }
        }
    }
}
