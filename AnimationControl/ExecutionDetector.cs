using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

    public class ExecutionDetector : MonoBehaviour{

        [Header("Detection Params")]
        public float DetectionDistance = 1f; 

        [Header("Parent params")]
        public CharacterControl parent_controller; 

        [Header("Debug")]
        public bool Show; 


        void OnTriggerEnter(Collider other){
            CharacterControl other_controller = other.gameObject.GetComponentInChildren<CharacterControl>();
            if(other_controller != null){
                other_controller.SetExecTarget(parent_controller, parent_controller.gameObject.GetComponent<CharacterAnimationControl>(), transform.position); 
            }
        }

        public void SetParent(CharacterControl controller){
            parent_controller = controller; 
            GetComponent<SphereCollider>().radius = DetectionDistance;
        }

        void OnDrawGizmos(){
            if(Show){
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, DetectionDistance); 
            }
        }

    }



}