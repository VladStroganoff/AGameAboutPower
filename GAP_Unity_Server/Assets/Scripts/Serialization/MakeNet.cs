using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MakeNet
{
    public static NetAnimator Animator(Animator animator)
    {
                NetAnimator netAnimator = new NetAnimator();
                netAnimator.Parameters = new NetAnimationParam[animator.parameters.Length];

                for(int j =0; j < animator.parameters.Length; j++)
                {
                    switch (animator.parameters[j].type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            {
                        BoolParam animBool = new BoolParam();
                                animBool.Name = animator.parameters[j].name;
                                animBool.State = animator.GetBool(animator.parameters[j].name);
                                netAnimator.Parameters[j] = animBool;
                                continue;
                            }
                        case AnimatorControllerParameterType.Float:
                            {
                        FloatParam animfloat = new FloatParam();
                                animfloat.Name = animator.parameters[j].name;
                                animfloat.State = animator.GetFloat(animator.parameters[j].name) ;
                                netAnimator.Parameters[j] = animfloat;
                                continue;
                            }
                        case AnimatorControllerParameterType.Trigger:
                            {
                        TriggerParam animtrigger = new TriggerParam();
                                animtrigger.Name = animator.parameters[j].name;
                                animtrigger.State = animator.parameters[j].defaultBool;
                                netAnimator.Parameters[j] = animtrigger;
                                continue;
                            }
                        case AnimatorControllerParameterType.Int:
                            {
                        IntigerParam animint = new IntigerParam();
                                animint.Name = animator.parameters[j].name;
                                animint.State = animator.GetInteger(animator.parameters[j].name);
                                netAnimator.Parameters[j] = animint;
                                continue;
                            }
                    }

            }
        return netAnimator;
    }
}

