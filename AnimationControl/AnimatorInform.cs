using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless;

namespace Pathless{

public class AnimatorInform : StateMachineBehaviour {

    public string Information;
    public string[] ResetOnEnter;
    public string[] ResetOnExit; 

    [Header("Intermediate call")]
    public bool UseCallback; 
    public float AfterRatio = 0.8f;
    public string AfterInformation;  
    public bool OnlyOnce = true; 
    bool only_once_counter; 

    [Header("HitInfo")]
    public bool IsHitAnimation; 
    public string HitboxName; 
    public HitData HitParameters; 

    [Header("LayerWeight")]
    public bool ChangeLayerWeight; 
    public bool ChangeOnEnter; 
    public float LayerWeightTo; 

     // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Call(animator, true); 
        ManageLayerWeight(animator, true);
        

        if(UseCallback)
            only_once_counter = true; 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(UseCallback){
            if(stateInfo.normalizedTime > AfterRatio){
                if(OnlyOnce){
                    if(only_once_counter){
                        only_once_counter = false; 
                        IntermediateCall(animator, AfterInformation);
                    }
                } else
                    IntermediateCall(animator, AfterInformation); 
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Call(animator, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    // }

    void IntermediateCall(Animator animator, string msg){
        animator.gameObject.GetComponent<CharacterAnimationControl>().CallbackInform(msg);
    }

    void ManageLayerWeight(Animator animator, bool on_enter){
        if(ChangeLayerWeight){
            if(ChangeOnEnter)
                animator.gameObject.GetComponent<CharacterAnimationControl>().ManageLayerWeight(LayerWeightTo); 
        }
    }

    [ContextMenu("Fill HitParams")]
    void FillHitParams(){
        HitParameters.Impulse.Strengh = 150f; 
        HitParameters.Impulse.Direction = new Vector3(0f, 0f, 1f);  
        HitParameters.Impulse.Delay = 0.3f;
        HitParameters.Impulse.Duration = 0.5f;  
        HitParameters.Impulse.Accel = 10f; 
        HitParameters.Impulse.Deccel =10f; 
    }

    void Call(Animator animator, bool state)
    {
        

        animator.gameObject.GetComponent<CharacterAnimationControl>().Inform(Information, state);

        if(IsHitAnimation){
            animator.gameObject.GetComponent<CharacterControl>().CombatInform(HitboxName, state, HitParameters);
        }

        if(state)
            DoReset(animator, ResetOnEnter); 
        else
            DoReset(animator, ResetOnExit); 

    }

    void DoReset(Animator animator, string [] triggers){
        if(triggers.Length > 0){
            foreach(string trigger in triggers){
                animator.ResetTrigger(trigger);
                animator.SetBool(trigger, false);  
            } 
        } 
    }
}

}