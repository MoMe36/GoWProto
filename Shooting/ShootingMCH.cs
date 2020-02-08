using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class ShootingMCH : MonoBehaviour
{

    public enum ShootingState {idle, aiming, recover}; 
    public ShootingState ShootState; 

    [Header("Target")]
    public Transform Target;

    [Header("ShootParameters")] 
    public float InitialRadius; 
    public float TimeToPinpoint; 
    Vector3 InitialDistance; 
    Vector3 EffectiveShootPoint; 
    float PinpointProgress; 

    [Header("Projectile")]
    public Transform InitialProjectilePosition;
    public GameObject Projectile; 

    [Header("DEBUG")]
    public bool ShowGizmos; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool shoot_flag = Input.GetAxis("R2") > 0.5f ? true : false; 
        ShootFunction(shoot_flag); 
    }

    void ShootFunction(bool shoot_action){
        if(shoot_action){
            if(ShootState == ShootingState.idle){
                BeginShoot(); 
            } else if(ShootState == ShootingState.aiming){
                KeepAiming();
            }
        } else{
            if(ShootState == ShootingState.aiming){
                DoShoot();
            }
        }

    }

    void BeginShoot(){
        ShootState = ShootingState.aiming; 
        InitialDistance = Random.insideUnitSphere * InitialRadius; 
        EffectiveShootPoint = Target.position + Vector3.ProjectOnPlane(InitialDistance, Target.position - transform.position); 
        PinpointProgress = TimeToPinpoint; 
    }

    void KeepAiming(){
        PinpointProgress -= Time.deltaTime; 
        PinpointProgress = Mathf.Max(PinpointProgress, 0f);
        EffectiveShootPoint = Vector3.Lerp(EffectiveShootPoint, Target.position, Mathf.Exp(-PinpointProgress)); 
    }

    void DoShoot(){
        Debug.Log("Fire"); 
        ShootState = ShootingState.recover; 
        CreateProjectile(); 
        Invoke("CompleteRecover", 0.5f); 
    }

    void CreateProjectile(){
        GameObject proj = Instantiate(Projectile, InitialProjectilePosition.position, Quaternion.identity) as GameObject;
        Vector3 direction = EffectiveShootPoint - InitialProjectilePosition.position; 
        proj.GetComponent<ArrowBehaviour>().Initialize(direction, true); 
    }

    void CompleteRecover(){
        ShootState =ShootingState.idle;
        Debug.Log("Ready to shoot again"); 
    }

    void OnDrawGizmos(){
        if(ShowGizmos){
            Gizmos.color = Color.blue; 
            Vector3 axis = Target.position - transform.position; 
            Vector3 axis_normal = new Vector3(axis.z, axis.y, axis.x); 
            Vector3 radius_direction = Vector3.Cross(axis_normal, axis); 
            // Debug.DrawRay(transform.position, axis, Color.green, 0.2f); 
            // Debug.DrawRay(transform.position, axis_normal, Color.red, 0.2f); 
            // Debug.DrawRay(Target.position, radius_direction, Color.cyan, 0.2f); 
            DrawCircle(Target.position, InitialRadius, axis.normalized);

            if(ShootState == ShootingState.aiming){
                Gizmos.color = Color.red;
                float current_dist = (Target.position - EffectiveShootPoint).magnitude; 
                DrawCircle(Target.position, current_dist, axis.normalized);  
            }

        }
    }

    void DrawCircle(Vector3 center, float radius, Vector3 axis){
        float inc = 36f;
        int segments = Mathf.RoundToInt(360f/inc);  
        
        Vector3 normale = Vector3.Cross(new Vector3(axis.z, axis.y, axis.x), axis); 
        Vector3 p1 = center + normale.normalized * radius; 
        Vector3 p2 = p1;  
        for(int i = 0; i<segments; i++){
            p2 = Quaternion.AngleAxis(inc, axis) * p1; 
            Gizmos.DrawLine(p1, p2); 
            Gizmos.DrawWireSphere(p2, 0.05f); 
            p1 = p2; 
        }
        Gizmos.DrawLine(p1, p2); 

    }
}

}