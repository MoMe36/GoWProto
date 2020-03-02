using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class EnemyAI : MonoBehaviour
{

    public enum EnemyStates {idle, move, dodge, sprint, attack, walk_around};
    [Header("Current enemy state")]
    public EnemyStates EnemyState;  
    CharacterControl control; 
    CharacterAnimationControl anim_control; 

    Vector3 last_mvt_input; 

    [Header("Brain parameters")]
    public float ReflexionTime = 5f; 
    [SerializeField] float reflexion_counter;
    public bool NextActionReady; 

    [Header("Move parameters")]
    public float StoppingDistance = 3f; 

    [Header("Sprint Attack")]
    public float SprintAttackDistance = 3f; 

    [Header("Walk Around state")]
    public float WalkAroundTime = 5f; 
    public float MaxWalkAroundDistance = 10f; 
    float walk_around_counter; 
    Vector3 walk_around_target_position; 

    [Header("DEBUG")]
    public Transform TargetPoint; 
    public bool DEBUG_Walk; 
    public bool DEBUG_Dodge; 
    public bool DEBUG_SprintAttack;
    public bool DEBUG_Hit;  
    public bool DEBUG_WalkAround; 
    public bool MANUAL_CONTROL; 

    public Transform DebugTargetWalkAround;


    void Start(){
        control = GetComponent<CharacterControl>(); 
        anim_control = GetComponent<CharacterAnimationControl>(); 
        control.EnemyInput = ai_Inputs; 
        ResetBrain(); 
    }

    void Update(){
        GetCurrentState(); 
    }

    void GetCurrentState(){
        // if(control.CurrentState == CharacterControl.CharacterState.normal)
        //     EnemyState = EnemyStates.idle; 
        if(control.CurrentState == CharacterControl.CharacterState.dodge)
            EnemyState = EnemyStates.dodge; 
    }


    void AI_Brain(out bool walk, out bool dodge, out bool sprint_attack, out bool walk_around, out bool hit){

        // // ===============================================================================================
        // // ===============================================================================================
        // // ===================================== PROBABILISTIC SM ======================================== 

        walk = false; 
        dodge = false; 
        sprint_attack = false; 
        walk_around = false; 
        hit = false;    
        // Debug.Log(reflexion_counter); 
        // Debug.Break(); 

        if(EnemyState == EnemyStates.idle || EnemyState == EnemyStates.walk_around){
            reflexion_counter -= Time.deltaTime; 
            if(reflexion_counter < 0f){
                NextActionReady = true;
            }
        }

        if(NextActionReady){
            string result_action = ProbActionSelection(); 
            if(result_action == "dodge")
                dodge = true; 
            else if(result_action == "sprint")
                sprint_attack = true; 
            else if(result_action == "w_a")
                walk_around = true; 

            Debug.Log("Selected " + result_action); 
            ResetBrain(); 
        }
        // // ===============================================================================================
        // // ===============================================================================================
        // // ===================================== MANUAL CONTROL ========================================== 
       

        if(MANUAL_CONTROL){
            walk = DEBUG_Walk; 
            dodge = DEBUG_Dodge; 
            sprint_attack = DEBUG_SprintAttack; 
            walk_around = DEBUG_WalkAround; 
            hit = DEBUG_Hit; 

            if(dodge)
                DEBUG_Dodge = false; 
            if(sprint_attack)
                DEBUG_SprintAttack = false; 
            if(walk_around) 
                DEBUG_WalkAround = false;
            if(hit)
                DEBUG_Hit = false;  
        } 
    }

    void ResetBrain(){
        NextActionReady = false; 
        reflexion_counter = ReflexionTime; 
    }

    string ProbActionSelection(){
        float random_val = Random.Range(0f, 1f); 
        if(random_val < 0.3f)
            return "dodge"; 
        else if(random_val >= 0.3f && random_val < 0.6f)
            return "sprint"; 
        else
            return "w_a"; 
    }

    void ai_Inputs(out Vector3 mvt, out bool dodge_input, out bool sprint_input, out bool launch_sprint_attack, out bool walk_around, out bool hit){
        bool walk_order, dodge_order, sprint_attack_order, launch_attack, walk_around_order, hit_order;
        launch_attack = false; 
        Vector3 mvt_direction = Vector3.zero;  
        AI_Brain(out walk_order, out dodge_order, out sprint_attack_order, out walk_around_order, out hit_order); 

        if(EnemyState == EnemyStates.dodge || dodge_order)
            ManageDodgeState(dodge_order, out mvt_direction); 
        else if(EnemyState == EnemyStates.sprint || sprint_attack_order)
            ManageSprintAttack(sprint_attack_order, out mvt_direction, out launch_attack); 
        else if(EnemyState == EnemyStates.walk_around || walk_around_order)
            ManageWalkAroundState(walk_around_order, out mvt_direction); 

        mvt = mvt_direction; 
        dodge_input = dodge_order; 
        sprint_input = sprint_attack_order; 
        launch_sprint_attack = launch_attack; 
        walk_around = walk_around_order; 
        hit = hit_order; 
    }

    void ManageWalkAroundState(bool action_order, out Vector3 mvt_dir){
        if(action_order){
            walk_around_counter = WalkAroundTime; 
            walk_around_target_position = transform.position + Vector3.ProjectOnPlane(Random.insideUnitSphere * MaxWalkAroundDistance, Vector3.up);
            // walk_around_target_position = new Vector3(DebugTargetWalkAround.position.x, transform.position.y, DebugTargetWalkAround.position.z); 
            // Debug.DrawRay(transform.position, DebugTargetWalkAround.position - transform.position, Color.red, 1f); 
            mvt_dir = walk_around_target_position - transform.position;   
            last_mvt_input = mvt_dir; 
        }

        mvt_dir = last_mvt_input; 

    }

    void ManageDodgeState(bool dodge_order, out Vector3 dodge_dir){

        if(dodge_order){
            dodge_dir = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up).normalized; 
            last_mvt_input = dodge_dir; 
        } else
            dodge_dir = last_mvt_input; 
    }

    void ManageSprintAttack(bool sprint_order, out Vector3 sprint_dir, out bool launch_attack){

        // if(sprint_order) // will keep the same direction. 
        //     sprint_dir = Vector3.ProjectOnPlane(TargetPoint.position - transform.position, Vector3.up).normalized; 
        // else 
        //     sprint_dir = last_mvt_input; 
        launch_attack = false; 
        sprint_dir =  Vector3.ProjectOnPlane(TargetPoint.position - transform.position, Vector3.up); 
        if(Vector3.SqrMagnitude(sprint_dir) < SprintAttackDistance*SprintAttackDistance)
            launch_attack = true; 
        
        last_mvt_input = sprint_dir; 
    }

    public Vector3 GetLookDirection(){
        return TargetPoint.position - transform.position; 
    }

    public bool IsWalkAround(){
        return EnemyState == EnemyStates.walk_around;
    }

    public void EnterSprint(){
        EnemyState = EnemyStates.sprint; 
    }

    public void EnterNormal(){
        EnemyState = EnemyStates.idle; 
    }

    public void EnterWalkAround(){
        EnemyState = EnemyStates.walk_around; 
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(TargetPoint.position, SprintAttackDistance); 
        if(IsWalkAround()){
            Gizmos.color = Color.green; 
            Gizmos.DrawWireSphere(walk_around_target_position, 2f);
        }
    }

    // void ai_Inputs(out Vector3 d1, out bool dodge_input){
    //     dodge_input = false; 
    //     if(Walk){
    //         Vector3 to_target = Vector3.ProjectOnPlane(TargetPoint.position - transform.position, Vector3.up); 
    //         d1 = Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance ? Vector3.zero : to_target; 
    //         if(Vector3.SqrMagnitude(to_target) < StoppingDistance * StoppingDistance)
    //             DEBUG_Walk = false; 
    //     }
    //     else
    //         d1 = Vector3.zero;   

    //     // ManageDodgeState()
    //     if(Dodge){
    //         d1 = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up).normalized; 
    //         dodge_input = true; 
    //         Dodge = false; 
    //     }
    // }



}

}