using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

    public class CharacterAnimation : MonoBehaviour
    {
    
        public static void DoSomething(){


        }

        public static void GetAngles(Transform weapon_tip, Transform frame_reference, Transform target, out float x_angle, out float y_angle){
            Vector3 target_direction = target.position - frame_reference.position;
            Vector3 frame_plane = Vector3.ProjectOnPlane(frame_reference.forward, Vector3.up);  
            y_angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(target_direction, Vector3.up), frame_plane, Vector3.up); 
            x_angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(target_direction, frame_reference.right), frame_plane, Vector3.up);

        }

        public static void LerpAnimatorValues(Animator anim, string var_name, float target_val, float speed){
            float x = anim.GetFloat(var_name); 
            float lerped = Mathf.MoveTowards(x, target_val, speed * Time.deltaTime); 
            anim.SetFloat(var_name, lerped); 
        }

        public static void SetAnimatorValues(Animator anim, string var_name, float target_val){
            anim.SetFloat(var_name, target_val); 
        }


    }

}