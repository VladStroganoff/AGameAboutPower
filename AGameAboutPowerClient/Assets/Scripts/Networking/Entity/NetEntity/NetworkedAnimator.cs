using Assets.Scripts;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkedAnimator : MonoBehaviour
{
    private Animator Animator;

    public vThirdPersonInput CharacterController;
    WorldController playerController;
    NetEntity myEntity;
    public bool updatePlayer { get; set; }
    public float UpdateIntervals = 3;


    public void Inject(WorldController controller, NetEntity netEnt)
    {
        playerController = controller;
        playerController.UpdateNetEnts += ReceiveAnimationData;
        myEntity = netEnt;
        Animator = GetComponent<Animator>();


        updatePlayer = true;
        StartCoroutine("SendAnimationData");
    }

    IEnumerator SendAnimationData()
    {
        while (updatePlayer)
        {
            myEntity = MakeEntity.UpdateAnimator(myEntity, Animator);

            DataSender.SendServerMessage(myEntity);

            yield return new WaitForSeconds(UpdateIntervals);
        }
    }

    void ReceiveAnimationData(NetEntity entity)
    {
        if (entity.ConnectionID != myEntity.ConnectionID)
            return;

        foreach(NetComponent component in entity.Components)
        {
            if(component is NetAnimator)
            {
                NetAnimator animator = component as NetAnimator;

                foreach(NetAnimatorComponent parameter in  animator.Parameters)
                {
                    if(parameter is NetAnimatorBool)
                    {
                        NetAnimatorBool boolio = parameter as NetAnimatorBool;
                        Animator.SetBool(boolio.name, boolio.state);
                    }
                    else if(parameter is NetAnimatorFloat)
                    {
                        NetAnimatorFloat floatio = parameter as NetAnimatorFloat;
                        Animator.SetFloat(floatio.name, floatio.value);
                    }
                    else if (parameter is NetAnimatorInt)
                    {
                        NetAnimatorInt intelio = parameter as NetAnimatorInt;
                        Animator.SetInteger(intelio.name, intelio.value);
                    }
                    else if (parameter is NetAnimatorTrigger)
                    {
                        NetAnimatorTrigger triggero = parameter as NetAnimatorTrigger;
                        Animator.SetTrigger(triggero.name);
                    }
                }
            }
        }

       
    }
    private void OnDisable()
    {
        playerController.UpdateNetEnts -= ReceiveAnimationData;
    }

}