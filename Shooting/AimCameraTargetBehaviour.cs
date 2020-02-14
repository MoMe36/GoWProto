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
            CurrentAngle = Mathf.Clamp(CurrentAngle, -MaxAngle, MaxAngle); 
            // transform.position = ReferenceObject.position + Vector3.up * InitialHeight + ReferenceObject.rotation * Quaternion.AngleAxis(CurrentAngle, ReferenceObject.right) * InitialOffset;  
            Quaternion rot = ReferenceObject.rotation * Quaternion.AngleAxis(CurrentAngle, Vector3.right); 
            Vector3 center = ReferenceObject.position + Vector3.up * InitialHeight; 
            BehindCharacter.position = center + rot * InitialBehindOffset + ReferenceObject.right * LateralBehindOffset;
            FrontCharacter.position = center + rot * InitialFrontOffset; 

        }



    }



}