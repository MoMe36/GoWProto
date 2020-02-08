using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


    public class CharacterAnimationControl : MonoBehaviour
    {

        Animator anim; 
        CharacterControl movement_control; 

        void Start(){

            movement_control = GetComponent<CharacterControl>(); 
            anim = GetComponent<Animator>(); 
        }   

        void Update(){

            // CharacterAnimation.GetAngles(GunBarrel, Center, EnnemyCenter, out XAngle, out YAngle);
            // CharacterAnimation.LerpAnimatorValues(anim, "x_aim", Mathf.Clamp(XAngle / MaxAngle, -1f, 1f), LerpSpeed); 
            // CharacterAnimation.LerpAnimatorValues(anim, "y_aim", Mathf.Clamp(YAngle / MaxAngle, -1f, 1f), LerpSpeed); 
            // CharacterAnimation.SetAnimatorValues(anim, "speed", movement_control.GetCharacterSpeed(speed_type : "h", normalized : true));

            // anim.SetBool("Aim", movement_control.IsAiming()); 
            SetMovementValues(); 

        }


        void SetMovementValues(){
            // speed 
            float speed_value = movement_control.GetCharacterSpeed(speed_type : "h", normalized : true);
            CharacterAnimation.SetAnimatorValues(anim, "speed", speed_value);

            Vector3 current_speed = movement_control.GetManualSpeed(); 
            float angle = Vector3.SignedAngle(transform.forward, current_speed, Vector3.up); 
            CharacterAnimation.SetAnimatorValues(anim, "X", Mathf.Sin(Mathf.Deg2Rad * angle));
            CharacterAnimation.SetAnimatorValues(anim, "Y", Mathf.Cos(Mathf.Deg2Rad * angle));
            // jump 

            // character state
        }


        public void Inform(string msg, bool enter){
            // if(msg == "weapon_set"){
            //     SetWeapon(); 
            // }
            
            if(msg == "dash"){
                if(enter){
                    movement_control.SetState("dash"); 
                } 
            } else if(msg == "move"){
                if(enter){
                    movement_control.SetState("normal");
                    movement_control.SetRunningState("normal"); 
                }
            } else if(msg == "hit"){
                if(enter)
                    movement_control.SetState("hit"); 
            } else if(msg == "run_turn"){
                if(enter)
                    movement_control.SetRunningState("turn"); 
            } else if(msg == "run_stop"){
                if(enter)
                    movement_control.SetRunningState("stop"); 
            }
        }


        public void CallbackInform(string msg){
            if(msg == "dash_exit")
                movement_control.ExitDash(); 
        }

        public void InformEvent(string msg){
            Inform(msg, true); 
        }

        public void Launch(string trigger_name){
            anim.SetTrigger(trigger_name); 
        }

        public void Activate(string bool_name){
            anim.SetBool(bool_name, true); 
        }

        public void HitAnimation(string msg){
            if(msg == "hit"){
                Launch(msg); 
                Debug.Log("Launched"); 
            }
            else if(msg == "follow")
                Activate("hit_follow"); 
            
        }

        public void ResetTrigger(string trigger_name){
            anim.ResetTrigger(trigger_name); 
        }
        // void SetWeapon(){
        //     GunInHand = !GunInHand; 
        //     Transform gun_parent = GunInHand ? HandHolder : ThighHolder; 
        //     Gun.SetParent(gun_parent);

        //     Gun.transform.position = gun_parent.position; 
        //     Gun.transform.rotation = gun_parent.rotation; 

        // }


    }

}