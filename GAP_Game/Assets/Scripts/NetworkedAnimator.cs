﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedAnimator : MonoBehaviour
{
    public Animator Animations;


    public void Set(NetAnimator animator)
    {
        
        if (Animations == null)
        {
            FDebug.Log.Message("animator was null, trying to reset it");
            Animations = GetComponent<Animator>();
            return;
        }

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
