using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class EnemyAI : MonoBehaviour
{

    public enum EnemyStates {idle, move, sprint, attack};
    [Header("Current enemy state")]
    public EnemyStates EnemyState;  
    CharacterControl control; 

    [Header("Move parameters")]
    public float StoppingDistance = 3f; 

    [Header("DEBUG")]
    public bool Walk; 
    public Transform TargetPoint; 


    void Start(){
        control = GetComponent<CharacterControl>(); 
        control.EnemyInput = AIInputs; 
    }

    void Update(){
        // GO TO Point 
    }


    void AIInputs(out Vector3 d1){
        if(Walk){
            Vector3 to_target = Vector3.ProjectOnPlane(TargetPoint.position - transform.position, Vector3.up); 
            d1 = Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance ? Vector3.zero : to_target; 
            if(Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance)
                Walk = false; 
        }
        else
            d1 = Vector3.zero;   
    }



}

}