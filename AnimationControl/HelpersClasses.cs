using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{

    [System.Serializable]
    public class Impulsion{
        public float Strengh; 
        public Vector3 Direction; 
        public float Delay;
        public float Accel; 
        public float Deccel;
        public float Duration;  
        public bool Active; 

        float counter_duration; 
        float counter_delay; 

        public Impulsion(){
            Direction = Vector3.forward; 
        }

        public Impulsion(float s, Vector3 v, float d, float a, float dd){
            Strengh = s; 
            Delay = d; 
            Deccel = dd; 
            Accel = a; 
            Direction = v; 
            if(Delay <= 0f)
                Active = true; 
            else
                Active = false;  
        }

        public bool Step(){
            bool is_active = false; 
            if(counter_delay >= 0f)
                counter_delay -= Time.deltaTime;
            else{
                if(counter_duration >= 0f){
                    counter_duration -= Time.deltaTime;
                    is_active = true; 
                } else{
                    is_active = false; 
                    Reset(); 
                }   
            }
            Active = is_active; 
            return Active; 
        }

        public void Reset(){
            counter_delay = Delay; 
            counter_duration = Duration; 
        }

        public Impulsion Clone(){
            // Impulsion result = new Impulsion(Strengh, Direction, Delay, Accel, Deccel); 

            // result.Strengh = Strengh; 
            // result.Direction = Direction; 
            // result.Delay = Delay; 
            // result.Accel = Accel; 
            // result.Deccel = Deccel;  

            // Impulsion result = this.MemberWiseClone();  

            return this; 
        }


    }


    [System.Serializable]
    public class HitData{
        public Impulsion Impulse; 
        public int Damage; 
    }



}