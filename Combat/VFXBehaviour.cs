using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

public class VFXBehaviour : MonoBehaviour{

    public enum EffectType {scale}; 
    [Header("Effect type")]
    public EffectType EffectBehaviour; 


    [Header("Scale effect")]
    public float ScalerSpeed; 
    public Vector3 ScalingVector = Vector3.one; 

    [Header("Lifetime")]
    public float Lifetime = 1f; 

    void Awake(){
        Destroy(gameObject, Lifetime); 
    }

    void Update(){

        if(EffectBehaviour == EffectType.scale){
            DoScale(); 
        }
    }

    void DoScale(){
        transform.localScale += ScalingVector * ScalerSpeed * Time.deltaTime;  
    }


}


}