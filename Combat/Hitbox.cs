using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class Hitbox : MonoBehaviour
{
    [Header("Hitbox State")]
    public bool Active; 
    public enum HBType {hit, hurt}; 
    public HBType BoxType; 
    public string BoxName; 

    [Header("VFX")]
    public GameObject ImpactEffect; 

    void Start(){

    }

    void Update(){

    }

    public void SetState(bool state, HitData hd){
        Active = state; 
        if(Active) 
            Activate();
        else
            Deactivate();
    }

    void Activate(){
        Active = true; 
        // ADD ENABLE VFX

        LaunchVFX();  
    }

    void LaunchVFX(){
        GameObject effect = Instantiate(ImpactEffect, transform.position, ImpactEffect.transform.rotation) as GameObject; 
    }

    void Deactivate(){
        Active = false; 
        // REMOVE VFX
    }

    void ReceiveDamage(){
        Debug.Log("Detected"); 
    }

    void OnTriggerEnter(Collider other){
        if(Active){
            Hitbox other_hb = other.GetComponent<Hitbox>(); 
            if(other_hb != null){
                other_hb.ReceiveDamage(); 
            }
        }
    }


}

}
