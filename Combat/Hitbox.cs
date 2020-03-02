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
    [HideInInspector] public HitData current_hit_data; 

    [Header("VFX")]
    public GameObject ImpactEffect; 

    // MainController
    [HideInInspector] public CharacterControl main_controller; 

    void Start(){

    }

    void Update(){

    }

    public void SetMainController(CharacterControl control){
        main_controller = control; 
    }

    public void SetState(bool state, HitData hd){
        Active = state; 
        if(Active) 
            Activate(hd);
        else
            Deactivate();
    }

    void Activate(HitData hd){

        Active = true; 
        current_hit_data = hd; 
        // ADD ENABLE VFX
        LaunchVFX();  
    }

    void LaunchVFX(){
        // GameObject effect = Instantiate(ImpactEffect, transform.position, ImpactEffect.transform.rotation) as GameObject; 
    }

    void Deactivate(){
        Active = false; 
        // REMOVE VFX
    }

    void ReceiveDamage(int damage){
        main_controller.AcknowledgeImpact(damage); 
    }

    void OnTriggerEnter(Collider other){
        if(Active){
            Hitbox other_hb = other.GetComponent<Hitbox>(); 
            if(other_hb != null){
                if(other_hb.main_controller != main_controller){
                    if(other_hb.BoxType == HBType.hurt)
                        other_hb.ReceiveDamage(current_hit_data.Damage); 
                }
                    
            }
        }
    }


}

}
