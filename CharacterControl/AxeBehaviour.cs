using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

public class AxeBehaviour : MonoBehaviour{

    public enum AxeStates {Held, Thrown, Called, Stuck};
    [Header("Axe State")] 
    public AxeStates AxeState; 

    [Header("Main Controller")]
    public CharacterControl MainController; 

    [Header("Movement Params")]
    public Vector3 ThrowDirection; 
    public float GravityForce; 

    [Header("Return Params")]
    public float ReturnSpeed; 
    Transform HandTarget;
    Vector3 to_hand_direction; 

    [Header("Rotation Params")]
    public Vector3 RotationAxis = Vector3.right; 
    public float RotationSpeed = 5f; 


    [Header("Debug")]
    public float CurrentHandDistance; 
    // CharacterController kinematic_controller; 


    public void SetThrowParams(Vector3 v){
        ThrowDirection = v; 
        AxeState = AxeStates.Thrown; 

    }

    public void ReturnOrder(Transform target){
        AxeState = AxeStates.Called; 
        HandTarget = target; 
    }

    public void AcknowledgeReturn(){
        AxeState = AxeStates.Held; 
    }

    void Start(){
        AxeState = AxeStates.Held;  
        // kinematic_controller = GetComponent<CharacterController>(); 
    }

    void Update(){
        if(AxeState == AxeStates.Thrown)
            AxeMovement(); 
        else if(AxeState == AxeStates.Called)
            AxeReturn(); 
    }

    void AxeReturn(){
        to_hand_direction = HandTarget.position - transform.position; 
        transform.position = Vector3.MoveTowards(transform.position, HandTarget.position, ReturnSpeed * Time.deltaTime); 
        CurrentHandDistance = Vector3.SqrMagnitude(to_hand_direction); 
        if(CurrentHandDistance < 1f){
            MainController.ReceiveAxe(); 
            AcknowledgeReturn(); 
        }

    }

    void AxeMovement(){
        transform.position = transform.position + ThrowDirection * Time.deltaTime; 
        ThrowDirection.y -= GravityForce*Time.deltaTime; 

        // Vector3 rot_axis = transform.rotation * RotationAxis.normalized;
        Quaternion rot_value = Quaternion.AngleAxis(RotationSpeed, RotationAxis.normalized); 
        transform.rotation = transform.rotation * rot_value; 
    }


}


}