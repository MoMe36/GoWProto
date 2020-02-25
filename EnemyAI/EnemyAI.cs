using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class EnemyAI : MonoBehaviour
{

    public enum EnemyStates {idle, move, dodge, sprint, attack};
    [Header("Current enemy state")]
    public EnemyStates EnemyState;  
    CharacterControl control; 

    Vector3 last_mvt_input; 

    [Header("Move parameters")]
    public float StoppingDistance = 3f; 

    [Header("DEBUG")]
    public bool DEBUG_Walk; 
    public Transform TargetPoint; 
    public bool DEBUG_Dodge; 


    void Start(){
        control = GetComponent<CharacterControl>(); 
        control.EnemyInput = ai_Inputs; 
    }

    void Update(){
        GetCurrentState(); 
    }

    void GetCurrentState(){
        if(control.CurrentState == CharacterControl.CharacterState.normal)
            EnemyState = EnemyStates.idle; 
        else if(control.CurrentState == CharacterControl.CharacterState.dodge)
            EnemyState = EnemyStates.dodge; 
    }


    void AI_Brain(out bool walk, out bool dodge){
        walk = DEBUG_Walk; 
        dodge = DEBUG_Dodge; 

        if(dodge)
            DEBUG_Dodge = false; 
    }


    void ai_Inputs(out Vector3 mvt, out bool dodge_input){
        bool walk_order, dodge_order;
        Vector3 mvt_direction = Vector3.zero;  
        AI_Brain(out walk_order, out dodge_order); 

        if(EnemyState == EnemyStates.dodge || dodge_order)
            ManageDodgeState(dodge_order, out mvt_direction); 
        // ManageWalkState(); 

        mvt = mvt_direction; 
        dodge_input = dodge_order; 
    }

    void ManageDodgeState(bool dodge_order, out Vector3 dodge_dir){

        if(dodge_order){
            dodge_dir = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up).normalized; 
            last_mvt_input = dodge_dir; 
        } else
            dodge_dir = last_mvt_input; 

    }

    // void ai_Inputs(out Vector3 d1, out bool dodge_input){
    //     dodge_input = false; 
    //     if(Walk){
    //         Vector3 to_target = Vector3.ProjectOnPlane(TargetPoint.position - transform.position, Vector3.up); 
    //         d1 = Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance ? Vector3.zero : to_target; 
    //         if(Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance)
    //             DEBUG_Walk = false; 
    //     }
    //     else
    //         d1 = Vector3.zero;   

    //     // ManageDodgeState()
    //     if(Dodge){
    //         d1 = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up).normalized; 
    //         dodge_input = true; 
    //         Dodge = false; 
    //     }
    // }



}

}