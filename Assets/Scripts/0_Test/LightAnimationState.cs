using Rooms.RoomSystem;
using UnityEngine;


public class LightAnimationState : StateMachineBehaviour
{
    [SerializeField] private RoomDistanceState lightState;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LightParentController testScript = animator.GetComponent<LightParentController>();
        // switch (lightState)
        // {
        //     case RoomDistanceState.Far:
        //         testScript.farLightParent.SetActive(true);
        //         break;
        //     case RoomDistanceState.Near:
        //         testScript.nearLightParent.SetActive(true);
        //         break;
        //     case RoomDistanceState.Current:
        //         testScript.currentLightParent.SetActive(true);
        //         break;
        // }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LightParentController testScript = animator.GetComponent<LightParentController>();
        // switch (lightState)
        // {
        //     case RoomDistanceState.Far:
        //         testScript.farLightParent.SetActive(false);
        //         break;
        //     case RoomDistanceState.Near:
        //         testScript.nearLightParent.SetActive(false);
        //         break;
        //     case RoomDistanceState.Current:
        //         testScript.currentLightParent.SetActive(false);
        //         break;
        // }
    }
}
