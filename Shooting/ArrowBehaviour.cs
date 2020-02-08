using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class ArrowBehaviour : MonoBehaviour
{

    public float ProjectileSpeed = 1f;  
    public Vector3 Direction; 
    public float Gravity; 
    CharacterController controller; 
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    void Awake(){
        controller = GetComponent<CharacterController>(); 
        Destroy(gameObject, 10f); 
    }

    public void Initialize(Vector3 speed, bool look_at = false){
        Direction = speed.normalized;
        if(look_at)
            transform.LookAt(transform.position + Direction); 
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(Direction * ProjectileSpeed * Time.deltaTime); 
        Direction += Vector3.down * Gravity * Time.deltaTime; 
    }
}

}