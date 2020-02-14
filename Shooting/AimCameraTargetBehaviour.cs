using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

    public class AimCameraTargetBehaviour : MonoBehaviour{

        public Transform ReferenceObject; 
        public Transform BehindCharacter; 
        public Transform FrontCharacter; 
        public float LateralBehindOffset; 
        public float MaxAngle = 40f;
        public float CurrentAngle; 
        Vector3 InitialBehindOffset;
        Vector3 InitialFrontOffset;  
        float InitialHeight; 
      
        void Start(){

            InitialBehindOffset = BehindCharacter.position - ReferenceObject.position;
            InitialFrontOffset = FrontCharacter.position - ReferenceObject.position;

            InitialHeight = (transform.position - ReferenceObject.position).y; 
            InitialFrontOffset = Vector3.ProjectOnPlane(InitialFrontOffset, Vector3.up); 
            InitialBehindOffset = Vector3.ProjectOnPlane(InitialBehindOffset, Vector3.up); 

        }

        void Update(){
            UpdateTargets(); 
        }

        void UpdateTargets(){
            Quaternion rot = ReferenceObject.rotation * Quaternion.AngleAxis(CurrentAngle, Vector3.right); 
            Vector3 center = ReferenceObject.position + Vector3.up * InitialHeight; 
            BehindCharacter.position = center + rot * InitialBehindOffset + ReferenceObject.right * LateralBehindOffset;
            FrontCharacter.position = center + rot * InitialFrontOffset; 

        }

        public void UpdateAngle(float angle_inc){
            CurrentAngle = Mathf.Clamp(CurrentAngle + angle_inc, -MaxAngle, MaxAngle); 
        }

        public void GetThrowAngle(out float val, out float normalized_val){
            val = CurrentAngle; 
            normalized_val = CurrentAngle/MaxAngle; 
        }



    }



}