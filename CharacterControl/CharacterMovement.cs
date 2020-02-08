using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class CharacterMovement : MonoBehaviour
{

    public static Vector3 ProjectInputOnPlane(Vector3 direction, Transform frame){
        if(direction == Vector3.zero)
            return Vector3.zero; 
        Vector3 v = Vector3.ProjectOnPlane(direction, Vector3.up); 
        Vector3 forward = Vector3.ProjectOnPlane(frame.forward, Vector3.up); 
        Vector3 right = Quaternion.AngleAxis(90f, Vector3.up) * forward; 

        return (forward * v.z + right*v.x).normalized; 
    }

    public static void RotateCharacter(Transform character_transform, 
                                       Vector3 look_direction, 
                                       float rotation_speed, 
                                       bool flat = true, bool instantaneous = false){
        Vector3 facing = flat ? Vector3.ProjectOnPlane(character_transform.forward, Vector3.up) : character_transform.forward;
        Vector3 going = flat ? Vector3.ProjectOnPlane(look_direction, Vector3.up) : look_direction; 
        float signed_angle = Vector3.SignedAngle(facing, going, Vector3.up); 
        // character_transform.rotation = Quaternion.RotateTowards(character_transform.rotation, character_transform.rotation * Quaternion.FromToRotation(facing, going), rotation_speed * Time.deltaTime);
        
        if(instantaneous)
            character_transform.rotation = character_transform.rotation * Quaternion.AngleAxis(signed_angle, Vector3.up);  
        else
            character_transform.rotation = Quaternion.RotateTowards(character_transform.rotation, character_transform.rotation * Quaternion.AngleAxis(signed_angle, Vector3.up), rotation_speed * Time.deltaTime);
        
    }   

    public static void MoveCharacter(CharacterController controller, Transform character_transform, 
                                    Vector3 move, float intensity, ref float h_speed,  
                                    float max_speed, float accel, float decel, 
                                    ref float v_speed, float jump_speed, float grounded_gravity, float airborne_gravity,
                                    float max_fall_speed, bool grounded, bool jump,  
                                    float rotation_speed, bool do_rotate, Vector3 face_direction){

        ComputeHorizontalSpeed(ref h_speed, intensity, max_speed, accel, decel); 
        ComputeVerticalSpeed(ref v_speed, jump_speed, grounded_gravity, airborne_gravity, max_fall_speed, grounded, jump); 
        if(do_rotate){
            Vector3 move_vector = character_transform.forward * h_speed + Vector3.up * v_speed; 
            controller.Move(move_vector * Time.deltaTime); 
            RotateCharacter(character_transform, move, rotation_speed, true); 
        } else{
            Vector3 move_vector = move.normalized * h_speed; 

            if(face_direction != Vector3.zero){ // THAT MEANS THE CHARACTER IS AIMING -> BE AFFECTED BY GRAVITY
                RotateCharacter(character_transform, face_direction, rotation_speed, true); 
                move_vector += Vector3.up * v_speed; 
            }
            
            controller.Move(move_vector * Time.deltaTime); 
        }
    }

    // public static void TiltCharacter(Transform target, Vector3 current_movement_direction,  
    //                                 Quaternion initial_rotation, float max_angle, float max_tilt, Vector3 tilt_around, 
    //                                 float tilt_speed, ref float current_tilt, float intensity, bool lerp = true){

    //     float angle = 0f;
    //     float tilt_target = 0f;  
    //     if(intensity > 0.5f){

    //         angle = Vector3.SignedAngle(target.forward, Vector3.ProjectOnPlane(current_movement_direction, Vector3.up), Vector3.down); 
    //         angle = Mathf.Clamp(angle/max_angle,-1f,1f); 
    //         tilt_target = angle * max_tilt; 
    //     }

    //     if(lerp)
    //         current_tilt = Mathf.MoveTowards(current_tilt, tilt_target, Time.deltaTime * tilt_speed); 
    //     else 
    //         current_tilt = tilt_target;  

    //     Quaternion rot = initial_rotation * Quaternion.AngleAxis(current_tilt, tilt_around);
    //     // target.rotation = Quaternion.MoveTowards(target.rotation, rot, tilt_speed * Time.deltaTime);  
    //     // if(lerp)
    //     // // else 
    //     target.rotation = rot; 
    // }

    public static void UpdateTilt(Transform target, Vector3 movement_dir, float intensity, ref float current_tilt_value, float max_rot, float lerp_speed){
        float angle = 0f; 

        if(intensity < 0.3f){
            lerp_speed *= 2.5f;         
        } else{
            angle = Vector3.SignedAngle(target.forward, Vector3.ProjectOnPlane(movement_dir, Vector3.up), Vector3.down); 
            angle = Mathf.Clamp(angle/max_rot, -1f, 1f); 
        }

        current_tilt_value = Mathf.MoveTowards(current_tilt_value, angle, lerp_speed * Time.deltaTime); 

    }

    public static void TiltCharacter(Transform target, Transform root_target, float tilt_val, Quaternion initial_rot, float max_angle, Vector3 tilt_around){
        target.rotation = root_target.rotation * Quaternion.AngleAxis(tilt_val * max_angle, tilt_around) * initial_rot; 
    }



    public static void ComputeHorizontalSpeed(ref float speed, float intensity, 
                                     float max_speed, float accel, float decel){

        float current_accel = intensity * max_speed > speed ? accel : decel; 
        speed = Mathf.MoveTowards(speed, intensity * max_speed, current_accel*Time.deltaTime); 

    }

    public static void ComputeVerticalSpeed(ref float vertical_speed, float jump_speed, 
                                            float grounded_gravity, float airborne_gravity, 
                                            float max_fall_speed, bool grounded, bool jump){

        float g = airborne_gravity;
        float v = max_fall_speed; 
       
        if (grounded){
            g = grounded_gravity; 
            v *= 0.5f; 
            if(jump){
                vertical_speed = jump_speed; 
            }
        }
        vertical_speed = Mathf.MoveTowards(vertical_speed, v, Time.deltaTime * g); 
    }

    public static bool CheckGrounded(Transform start_from, float height, int ignore_layer){
        Ray ray = new Ray(start_from.position, Vector3.down); 
        RaycastHit hit; 

        int layer_mask = 1 << ignore_layer; 
        // layer_mask = ~layer_mask; 

        bool touched = false; 
        if(Physics.Raycast(ray, out hit, height, layer_mask)){
            Debug.Log(hit.collider.gameObject); 
            touched = true;
        }
        return touched; 
    }

}

}
