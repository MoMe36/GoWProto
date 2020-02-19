using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 
using Cinemachine; 

namespace Pathless{


public class CharacterLifePoints : MonoBehaviour
{

    public int MaxLifePoints; 
    public int CurrentLifePoints; 

    CharacterAnimationControl anim_control; 
    CharacterControl char_control; 


    void Start(){

        anim_control = GetComponent<CharacterAnimationControl>(); 
        char_control = GetComponent<CharacterControl>(); 

        CurrentLifePoints = MaxLifePoints; 
    } 

    public void AcknowledgeDamages(int damage_value){
        CurrentLifePoints -= damage_value; 
        if(CurrentLifePoints < 0)   
            LaunchExecution(); 
    }

    public void LaunchExecution(){
        anim_control.BoolControl("FinishExec", true);
        char_control.ExecPerformerAnim.BoolControl("FinishExec", true);  
    }


}
}