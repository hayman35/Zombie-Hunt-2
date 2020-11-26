using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class test : MonoBehaviour
{
     public float hp = 100.0f, damage = 20.25f, bodieRemovalTime = 10.0f, moveSpeed = 2.0f, fieldOfView = 45.0f, viewDistance = 5.0f, playerSearchInterval = 1.0f, minChase = 5.0f, maxChase = 10.0f, minWander = 5.0f, maxWander = 20.0f;

     public bool canRun = false;

     public Transform zombieHead = null;//Pivot point to use as reference, use it as if it were the eyes of the zombie.
     public LayerMask checkLayers;//Layers to check when searching for the player (after the check interval)

     private bool playerChase = false, wandering = false;

     private float lastCheck = -10.0f, lastChaseInterval = -10.0f, lastWander = -10.0f;

     private NavMeshAgent agent;//This zombie's nav mesh agent
     private Animator Anim;//This zombie's animator
     [SerializeField] private Transform player = null;//Player position/transform
     private Vector3 lastKnownPos = Vector3.zero;//Last known player position


     void Start () {
         //THIS IS USED TO GET THE LOCAL NAV MESH AGENT, wanderManager AND ANIMATOR OF THIS ZOMBIE
         agent = GetComponent<NavMeshAgent>();
         Anim = GetComponent<Animator>();

         //SET THE MAIN VALUES
         agent.speed = moveSpeed;
         agent.acceleration = moveSpeed * 40;
         agent.angularSpeed = 999;
     }

     private void Update() 
     {
        chasePlayer();
             
          //SET THE ATTACK AND RESET IT
             AnimatorStateInfo state = Anim.GetCurrentAnimatorStateInfo(0);
             if(!state.IsName("Attack") && playerChase && reachedPathEnd()){//READY TO ATTACK!
                 Anim.SetTrigger("doAttack");
                 Anim.SetBool("isIdle", false);
                 Anim.SetBool("isChasing", false);
                 playerChase = false;
             }
             if(state.IsName("Attack") && state.normalizedTime > 0.90f){
                 chasePlayer();
                 Attack();//HERE WE CALL THE PLAYER'S DAMAGE
             }
     }

      //Check if agent reached the player
     bool reachedPathEnd (){
         if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
             if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.0f){
                 return true;
             }
             return true;
         }
         return false;
     }

     void chasePlayer(){
         Anim.SetBool("isChasing", true);
 
         if(canRun)
         Anim.SetBool("isRunning", true);
 
         Anim.SetBool("isIdle", false);
         playerChase = true;
         agent.SetDestination(player.position);
         lastKnownPos = player.position;
         agent.Resume();
         wandering = false;
     }

     void stopChase(){
         Anim.SetBool("isChasing", false);
 
         if(canRun)
             Anim.SetBool("isRunning", false);
 
         Anim.SetBool("isIdle", true);
         playerChase = false;
         agent.Stop();
         wandering = false;
     }

     bool meleeDistance(){
         if(Vector3.Distance(transform.position, player.position) < 2.0f){
             return true;
         }
 
         return false;
     }

     void Attack(){
         if(!meleeDistance())//DON'T DO ANYTHING IF WE ARE NOT AT A MELEE DISTANCE TO THE PLAYER
             return;
 
         /*
          * HERE YOU SET DAMAGE TO PLAYER YOU MUST DO THE CODE BY YOURSELF
          * REMEMBER THE PLAYER VARIABLE IS ALREADY SET
         */
         agent.updateRotation = false;
         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.destination - transform.position, transform.up), 7.0f *Time.deltaTime);
         agent.updateRotation = true;
     }

     void Damage(float dmg){
         /*HERE YOU CALL DAMAGE FOR ZOMBIE EXAMPLE: 
         *zombieObject.SendMessage("ZombieDamage", float value, SendMessageOptions.RequireReceiver);
         */
         hp -= dmg;
 
         if(player != null)
             chasePlayer();
 
         if(hp < 0.0f || hp == 0.0f){//This zombie is freakin' dead!
            
         }
     }
}
