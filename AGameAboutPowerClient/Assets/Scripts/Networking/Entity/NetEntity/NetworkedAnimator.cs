using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkedAnimator : MonoBehaviour
{

    public List<NetAnimatorBool> BoolParameters = new List<NetAnimatorBool>();
    public List<NetAnomatorFloat> FloatParameters = new List<NetAnomatorFloat>();

    private Animator Animator;
    private AnimatorControllerParameter[] AnimParams;

    public vThirdPersonInput CharacterController;
    PlayerController playerController;


    public NetworkedAnimator Inject(PlayerController controller)
    {
        playerController = controller;
        return this;
    }

    private void Reset()
    {
        SetupAnimator();
    }

    void SetupAnimator()
    {
        Animator = GetComponent<Animator>();
        AnimParams = Animator.parameters;

        foreach (AnimatorControllerParameter param in AnimParams)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    {
                        NetAnimatorBool newBool = new NetAnimatorBool();
                        newBool.name = param.name;
                        newBool.state = param.defaultBool;
                        BoolParameters.Add(newBool);
                        continue;
                    }
                case AnimatorControllerParameterType.Float:
                    {
                        NetAnomatorFloat newFloat = new NetAnomatorFloat();
                        newFloat.name = param.name;
                        newFloat.value = param.defaultFloat;
                        FloatParameters.Add(newFloat);
                        continue;
                    }
                case AnimatorControllerParameterType.Trigger:
                    {
                        Debug.Log("Trigger animtor parameter found but not added");
                        continue;
                    }
                case AnimatorControllerParameterType.Int:
                    {
                        Debug.Log("Int animtor parameter found but not added");
                        continue;
                    }
            }

        }
    }

    private void Start()
    {
        ScyncAnimationData();
    }

    void ScyncAnimationData()
    {
        if(CharacterController.isLocalPlayer == true)
        {

        }
        else
        {

        }
    }

   
}