using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathless; 

namespace Pathless{


public class CharacterControl : MonoBehaviour
{

    CharacterController controller; 
    CharacterAnimationControl anim_control; 

    public enum CharacterState {normal, dash, aim, hit, dizzy, cinematic};
    [Header("Character States")] 
    public CharacterState CurrentState; 

    public enum MovingState {normal, stop, turn}; 
    public MovingState MoveState; 
    // public enum MovementState {normal, airborne};
    // [Header("Character States")] 
    // public MovementState CurrentMovementState; 
    public enum AxeStates{InHand, InHolder, AxeOut}; 
    [Header("Axe Control")]
    public AxeStates AxeState;  
    public Transform Axe; 
    public Transform AxeHand; 
    public Transform AxeHolder; 
    public AxeBehaviour AxeControl; 
    public float ThrowForce; 
    public float ThrowAngle; 

    [Header("Camera")]
    public Transform CurrentCamera; 

    [Header("Ground Movement Variables")]
    public float GroundSpeed; 
    public float Acceleration; 
    public float Decceleration; 
    public float RotationSpeed; 
    public float Gravity; 

    public float current_horizontal_speed;
    public float current_vertical_speed; 
    Vector3 LastPosition; 
    Vector3 PositionDiff; 
    Vector3 movement_dir; 
    Vector3 last_movement_dir; 
    float intensity; 
    float last_intensity; 


    [Header("Run Tilting")]
    public Transform TiltTarget; 
    public bool AllowTilt; 
    public float TiltMaxAngle = 45f; 
    public float TiltMaxRotation = 20f; 
    public float TiltLerpSpeed = 2f;
    [Range(-1f, 1f)] public float current_tilt_value; 
    Quaternion initial_tilt_rotation;  

    [Header("Aerial Variables")]
    public Transform AerialStateCheck; 
    public float HeightCheck;    
    public float JumpSpeed; 
    public float GroundedGravity; 
    public float AirborneGravity; 
    public float MaxFallSpeed;    
    public LayerMask HitboxLayer; 

    [Header("Cinematic Move Params")]
    public Vector3 CinematicTargetPosition; 
    public float CinematicMaxMoveSpeed; 

    [Header("Dash Variables")]
    public float DashForce = 10f; 
    public float DashAcceleration; 

    [Header("Aim Variables")]
    public float AimSpeed = 10f; 
    public float AimAcceleration; 
    public float AimAerialGravity; 


    [Header("Fight")]
    // public face_directionHitbox[] Hitboxes; 
    public Dictionary<string, Hitbox> HitDict; 
    Impulsion CharacterImpulse; 


    // DIZZY STATE
    [Header("DizzyState")]
    public GameObject PrefabExecDetector;
    GameObject ExecDetector;  
    public float InstantiateDistance = 1f; 
    bool has_exec_target; 
    CharacterControl ExecTarget; 
    CharacterAnimationControl ExecTargetAnim; 


    [Header("DEBUG")]
    public bool ShowDebug; 
    public bool IsGrounded;  
    public float DashDuration = 0.1f; 
    float dash_timing; 
    public Transform Ennemy; 


    [Header("TRIGGERS")]
    public bool jump_trigger; 
    bool dash_trigger; 

    void Start(){
        controller = GetComponent<CharacterController>(); 
        anim_control = GetComponent<CharacterAnimationControl>(); 

        LastPosition = transform.position; 
        HitDict = CharacterCombat.GetHBDict(gameObject); 

        if(AllowTilt){
            current_tilt_value = 0f; 
            initial_tilt_rotation = TiltTarget.transform.rotation; 
        }

        UpdateAxeState(); 
    }

    void UpdateAxeState(){
        if(Axe != null){
            if(AxeState == AxeStates.InHand){
                Axe.parent = AxeHand; 
                anim_control.SetAxeState(true); 
            } else if(AxeState == AxeStates.InHolder){
                Axe.parent = AxeHolder;
                anim_control.SetAxeState(false);  
            }

            Axe.transform.position = Axe.transform.parent.position;
            Axe.transform.rotation = Axe.transform.parent.rotation;
        }
    }

    void Update(){

        bool grounded = CharacterMovement.CheckGrounded(AerialStateCheck, HeightCheck, ignore_layer: HitboxLayer); 
        bool landed = (!IsGrounded && grounded); 
        bool fall = (!grounded && IsGrounded); 
        IsGrounded = grounded;
        
        if(Axe != null)
            PlayerMove(grounded, landed, fall); 

    }


    void PlayerMove(bool grounded, bool landed, bool fall){

        Vector3 user_dir = new Vector3(Input.GetAxis("Horizontal"), 
                                       0f,
                                       Input.GetAxis("Vertical")); 

        bool jump_input = Input.GetButtonDown("AButton"); 
        bool dash_input = Input.GetButtonDown("BButton"); 
        bool aim = Input.GetAxis("L2") > 0.2f ? true : false; 
        bool hit = Input.GetButtonDown("XButton"); 
        bool change_weapon_state = Input.GetButtonDown("YButton"); 
        bool axe_action = Input.GetButton("R1"); 
        bool call_axe = Input.GetButton("L1") && AxeState == AxeStates.AxeOut; 


        intensity = Vector3.SqrMagnitude(user_dir); 
        if(intensity < 0.2f * 0.2f)
            user_dir = Vector3.zero; 

        movement_dir = CharacterMovement.ProjectInputOnPlane(user_dir, CurrentCamera);         

        bool run_turn, run_stop; 
        RunStopsLogic(out run_turn, out run_stop); 

        SendTriggerToAnimator(jump_input, landed, fall, dash_input, hit, run_turn, run_stop, change_weapon_state, call_axe);
        SendAnimatorBool(aim, axe_action); 

        MoveLogic(movement_dir, intensity, grounded);
        ResetTriggers(); 

    }

    void LateUpdate(){
        PositionDiff = (transform.position - LastPosition) / Time.deltaTime;
        LastPosition = transform.position; 

        last_movement_dir = movement_dir; 
        last_intensity = intensity; 
    }

    void RunStopsLogic(out bool turn, out bool stop ){
        turn = stop = false; 

        if(Mathf.Abs(Vector3.SignedAngle(movement_dir, last_movement_dir, Vector3.up)) > 50f)
            turn = true; //anim_control.Launch("run_turn") // Set in function

        if(last_intensity - intensity > 0.7f)
            stop = true; 
    }

    void MoveLogic(Vector3 mvt_dir, float strengh, bool grounded){



        if(CurrentState ==CharacterState.normal){
            float current_rot_speed = RotationSpeed;
            if(MoveState == MovingState.normal){
                float a = 0f; 
            }
            else if(MoveState == MovingState.turn){
                current_horizontal_speed = Mathf.MoveTowards(current_horizontal_speed, 0f, 5f * Time.deltaTime);
                strengh = 1f ;
                // current_rot_speed *= 0.1f; 
                current_horizontal_speed = 0f; 
            } else if(MoveState == MovingState.stop){
                strengh = 0f; 
            } else {
                // Debug.Break(); 
            }

            CharacterMovement.MoveCharacter(controller, transform, 
                                    mvt_dir, strengh, ref current_horizontal_speed,  
                                    GroundSpeed, Acceleration, Decceleration, 
                                    ref current_vertical_speed, JumpSpeed, 
                                    GroundedGravity, AirborneGravity, MaxFallSpeed, 
                                    grounded, jump_trigger, current_rot_speed, true, Vector3.zero); 

            if(AllowTilt){
                CharacterMovement.UpdateTilt(transform, mvt_dir, strengh, ref current_tilt_value, TiltMaxRotation, TiltLerpSpeed); 
                CharacterMovement.TiltCharacter(TiltTarget, transform, current_tilt_value, initial_tilt_rotation, 
                                                TiltMaxAngle, Vector3.forward);  

            } 


        } else if(CurrentState == CharacterState.dash){
            CharacterMovement.MoveCharacter(controller, transform, 
                                    mvt_dir, intensity :1f, ref current_horizontal_speed,  
                                    DashForce, DashAcceleration, Decceleration, 
                                    ref current_vertical_speed, JumpSpeed, 
                                    GroundedGravity, AirborneGravity, MaxFallSpeed, 
                                    true, false, 0f, false, Vector3.zero);

            // if(AllowTilt)   // RESET TILT VALUE
            //     CharacterMovement.TiltCharacter(transform, transform.forward, initial_tilt_rotation,  
            //                             60f, TiltMaxAngle, transform.forward, 
            //                             TiltLerpSpeed * 2f, ref current_tilt_value, intensity, lerp:true);

        } else if(CurrentState == CharacterState.aim){

            Vector3 look_at_ennemy = Ennemy.position - transform.position; 
            CharacterMovement.MoveCharacter(controller, transform, 
                                    mvt_dir, intensity : strengh, ref current_horizontal_speed,  
                                    GroundSpeed, AimAcceleration, Decceleration, 
                                    ref current_vertical_speed, JumpSpeed, 
                                    GroundedGravity, AimAerialGravity, MaxFallSpeed, 
                                    grounded, jump_trigger, RotationSpeed, false, face_direction: look_at_ennemy);
        } else if(CurrentState == CharacterState.hit){

            mvt_dir = CharacterImpulse.Direction;  
            strengh = CharacterImpulse.Step() ? 1f : 0f;  
            CharacterMovement.MoveCharacter(controller, transform, mvt_dir, intensity : strengh, 
                                            ref current_horizontal_speed, CharacterImpulse.Strengh, 
                                            CharacterImpulse.Accel, CharacterImpulse.Deccel, 
                                            ref current_vertical_speed, JumpSpeed, GroundedGravity, AirborneGravity, 
                                            MaxFallSpeed, grounded, jump_trigger, 0f, true, Vector3.zero); 
        } else if(CurrentState == CharacterState.cinematic){
            // PLACEHOLDER. SHOULD BE SET WITHIN MOVECHARACTER CLASS
            transform.position = Vector3.MoveTowards(transform.position, CinematicTargetPosition, Time.deltaTime * CinematicMaxMoveSpeed); 
            transform.LookAt(ExecTarget.gameObject.transform.position); 
        }
        

    }

    void SendAnimatorBool(bool aim, bool axe_action){
            anim_control.BoolControl("Aim", aim); 
            if(aim && axe_action)
                anim_control.Launch("Throw"); 
    }

    void SendTriggerToAnimator(bool jump, bool landed, bool fall, 
                               bool dash, bool hit, bool run_turn, 
                               bool run_stop, bool change_weapon_state, 
                               bool call_axe){
        if(jump){
            if(IsGrounded){
                anim_control.Launch("jump"); 
                anim_control.ResetTrigger("land"); 
            }
        } 

        if(landed){
            anim_control.Launch("land"); 
        }  

        if(fall){
            anim_control.Launch("fall"); 
        }

        if(IsGrounded){
            if(dash){
                anim_control.Launch("dash"); 
            }
        }

        if(run_turn)
            anim_control.Launch("run_turn"); 


        if(run_stop)
            anim_control.Launch("run_stop"); 

        if(hit){
            if(CurrentState == CharacterState.hit){
                anim_control.HitAnimation("follow"); 
            } else {
                if(has_exec_target){
                    if(CurrentState == CharacterState.cinematic){
                        anim_control.Launch("ExecPunch");
                        ExecTargetAnim.Launch("ExecPunch");
                    } else{
                        LaunchExecutionProcedure(); 
                    }
                } else {
                    anim_control.HitAnimation("hit"); 
                }
            }
        }

         if(change_weapon_state){
            anim_control.Launch("GetAxe"); 
        }

        if(call_axe)
            anim_control.Launch("CallAxe"); 
    }

    public void DoThrow(){
        Axe.parent = null; 
        AxeState = AxeStates.AxeOut; 
        AxeControl.SetThrowParams(Quaternion.AngleAxis(ThrowAngle, transform.right) * transform.forward * ThrowForce); 
        anim_control.SetThrowParams(); 
    }

    public void CallAxe(){
        AxeControl.ReturnOrder(AxeHand);
    }

    public void ReceiveAxe(){
        AxeState = AxeStates.InHand; 
        UpdateAxeState(); 

    }

    void OnDrawGizmos(){
        if(ShowDebug){
            ShowContactPoint(); 
        }
    }

    public Vector3 GetManualSpeed(){
        return PositionDiff; 
    }

    public void MovementInform(string msg){
        if(msg == "jump")
            jump_trigger = true; 
    }

    public void ExitDash(){
        CharacterMovement.RotateCharacter(transform, movement_dir, 1f, flat : true, instantaneous : true); 
        // Debug.Break(); 
    }

    public void CombatInform(string hb_name, bool enter, HitData hd){
        if(HitDict.ContainsKey(hb_name)){
            HitDict[hb_name].SetState(enter, hd);
            if(enter){
                CombatMovement(hd);
            }
        }
    }

    public void CombatMovement(HitData hd){
        if(CharacterImpulse != null)
            CharacterImpulse.Reset(); 
        CharacterImpulse = hd.Impulse.Clone(); 

    }

    public void SetState(string new_state_name){
        if(new_state_name == "normal")
            CurrentState = CharacterState.normal; 
        else if(new_state_name == "dash")
            CurrentState = CharacterState.dash; 
        else if(new_state_name == "hit")
            CurrentState = CharacterState.hit; 

    }

    public void ChangeWeaponState(){
        if(AxeState == AxeStates.InHand){
            AxeState = AxeStates.InHolder; 
        } else if(AxeState == AxeStates.InHolder){
            AxeState = AxeStates.InHand;
        }

        UpdateAxeState(); 
    }

    public void SetAxeState(string msg){
        if(msg == "hand")
            AxeState = AxeStates.InHand;
        else
            AxeState = AxeStates.InHolder; 

        UpdateAxeState(); 
    }

    public void SetExecTarget(CharacterControl to_be_exec, CharacterAnimationControl exec_anim, Vector3 exec_pos){
        has_exec_target = true; 
        ExecTarget = to_be_exec; 
        ExecTargetAnim = exec_anim; 
        CinematicTargetPosition = exec_pos; 
    }

    public void EnterDizzy(){
        CurrentState = CharacterState.dizzy; 
        InitializeDizzy(); 
    }

    public void ExitDizzy(){
        DestroyDizzy(); 
    }

    void LaunchExecutionProcedure(){
        // Move character into place 
        CurrentState = CharacterState.cinematic; // So it moves to the right place

        // Set Axe in back 
        SetAxeState("Holder"); 
        // Launch animations on both characters
        anim_control.Launch("ExecWulver"); 
        ExecTarget.AcknowledgeExec(); 
    }

    void AcknowledgeExec(){
        CurrentState = CharacterState.cinematic; 
        anim_control.Launch("Exec"); 
    }

    void InitializeDizzy(){
        ExecDetector = Instantiate(PrefabExecDetector, transform.position + transform.forward * InstantiateDistance, Quaternion.identity) as GameObject; 
        ExecDetector.GetComponent<ExecutionDetector>().SetParent(this); 
    }

    void DestroyDizzy(){
        Destroy(ExecDetector); 
    }

    public void SetRunningState(string new_state_name){
        if(new_state_name == "normal")
            MoveState = MovingState.normal; 
        else if(new_state_name == "turn")
            MoveState = MovingState.turn; 
        else if(new_state_name == "stop")
            MoveState = MovingState.stop; 
    }

    public bool IsAiming(){
        return Input.GetAxis("L2") > 0.2f ? true : false;   
    } 

    public float GetCharacterSpeed(string speed_type, bool normalized){
        
        float current_speed; 
        float current_max; 
        if(speed_type == "h"){
            current_speed = current_horizontal_speed; 
            current_max = GroundSpeed; 
        } else{
            current_speed = current_vertical_speed;
            current_max = MaxFallSpeed; 
        }
        if(normalized){
            float result = Mathf.Clamp01(current_speed / current_max); 
            return result; 
        } else{
            return current_speed; 
        }
    }

    public bool IsAirborne(){
        return IsGrounded; 
    }

    void ResetTriggers(){
        jump_trigger = false; 

        // Debug.Log("Called"); 
        // bool[] triggers = new bool[]{jump_trigger}; 
        // for(int i = 0; i < triggers.Length; i ++ )
        //     triggers[i] = false;  
        // foreach(bool trigger in new bool[]{jump_trigger, dash_trigger})
            // trigger = false; 
    }


    void ShowContactPoint(){
        Gizmos.DrawLine(AerialStateCheck.position, AerialStateCheck.position + Vector3.down * HeightCheck); 
    }
}

}
