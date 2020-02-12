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
    public float LateralReturnSpeed = 1f; 
    public float CurveMultiplier = 1.5f; 
    public AnimationCurve LateralInfluence;
    float curve_eval;
    float initial_sqr_dist; 
    Transform HandTarget;
    Vector3 to_hand_direction; 
    Vector3 lateral_hand_direction; 

    public float ReturnRotationSpeed = 5f; 

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
        to_hand_direction = HandTarget.position - transform.position; 
        lateral_hand_direction = Quaternion.AngleAxis(90f, Vector3.up) * to_hand_direction; 
        
        curve_eval = 0f; 
        initial_sqr_dist = Vector3.SqrMagnitude(to_hand_direction); 
    }

    public void AcknowledgeReturn(){
        AxeState = AxeStates.Held; 
    }

    void Start(){
        AxeState = AxeStates.Held;  
        RotationAxis = RotationAxis.normalized; 
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
        curve_eval = Mathf.Clamp01(curve_eval + Time.deltaTime * LateralReturnSpeed); 
        float influence = LateralInfluence.Evaluate(curve_eval) * CurveMultiplier; 
        Vector3 target_position = HandTarget.position * (1f - influence) + (HandTarget.position + lateral_hand_direction) * influence; 
        transform.position = Vector3.MoveTowards(transform.position, target_position, ReturnSpeed * Time.deltaTime); 
        RotateAxe(-ReturnRotationSpeed); 
        // transform.position = Vector3.MoveTowards(transform.position, HandTarget.position, ReturnSpeed * Time.deltaTime); 
        CurrentHandDistance = Vector3.SqrMagnitude(to_hand_direction); 
        if(CurrentHandDistance < 1f){
            MainController.ReceiveAxe(); 
            AcknowledgeReturn(); 
        }

    }

    void RotateAxe(float speed){
        Quaternion rot_value = Quaternion.AngleAxis(speed, RotationAxis); 
        transform.rotation *= rot_value; 
    }

    void AxeMovement(){
        transform.position = transform.position + ThrowDirection * Time.deltaTime; 
        ThrowDirection.y -= GravityForce*Time.deltaTime; 

        // Vector3 rot_axis = transform.rotation * RotationAxis.normalized;
        // Quaternion rot_value = Quaternion.AngleAxis(RotationSpeed, RotationAxis.normalized); 
        // transform.rotation = transform.rotation * rot_value; 
        RotateAxe(RotationSpeed); 
    }


}


}