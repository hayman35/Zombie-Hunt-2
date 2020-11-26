using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float hp = 100.0f, damage = 20.25f, bodieRemovalTime = 10.0f, moveSpeed = 2.0f, fieldOfView = 45.0f, viewDistance = 5.0f, playerSearchInterval = 1.0f, minChase = 5.0f, maxChase = 10.0f, minWander = 5.0f, maxWander = 20.0f;
     /*
      * Health points of this zombie.
      * Damage of the attacks of this zombie.
      * Time to wait for deleting the dead bodie.
      * GLOBAL SPEED OF THE ZOMBIE I RECOMMEND 2 FOR WALKERS AND 7 FOR RUNNERS, DEPENDS ON YOUR GAME
      * FIELD OF VIEW OF THE ZOMBIE
      * VIEW DISTANCE OF THE ZOMBIE
      * INTERVAL USED TO CHECK IF THE ZOMBIE IS LOOKING AT THE PLAYER, JUST TO PREVENT OVERUSE OF RESOURCES
      SET 0 IF YOU DON'T CARE OR KEEP IT BETWEEN 1.0F AND 0.5F, RECOMMENDED
      *Min time to chase the player
      *Max time to chase the player (will be random between these 2 values)
      *Min time to wander around
      *Max time to wander around (works the same way)
     */
     public string playerTag = "Player";
     /*
      * SET HERE THE TAG TO IDENTIFY YOUR PLAYER
      * YOUR PLAYER DEAD BODIES
     */
     public bool canRun = false;
     /*
      * CAN YOUR ZOMBIE RUN?
      * WILL IT EAT THE DEAD PLAYER BODIES?
     */
     public Transform zombieHead = null;//Pivot point to use as reference, use it as if it were the eyes of the zombie.
     public LayerMask checkLayers;//Layers to check when searching for the player (after the check interval)
 
     /*Zombie sounds AnimationStates
      * 0 = Sound on wandering, idle or whatever.
      * 1 = Sound for chasing player.
      * 2 = Sound for losing player or extras.
      * 3 = Sound while eating.
      * Length of 4.
     */
     public AudioClip[] audioClips = new AudioClip[4];
     /*
      * If you want to set a new clip, you must change the length of the array and edit the inspector script.
      * You can find the needed line by searching "AnimationStates", on the script ZombieUI.cs
     */
 
     private bool playerChase = false, wandering = false;
     /*
      * Will be true when the zombie is chasing the player, then the code will randomize when will stop doing it.
     */
     private float lastCheck = -10.0f, lastChaseInterval = -10.0f, lastWander = -10.0f;
     /*
     *Last time we checked the player's position.
     *Last or next time we chased the player before losing him.
     *Last time we set a wander position
     */
     private NavMeshAgent agent;//This zombie's nav mesh agent
     private Animator Anim;//This zombie's animator
     [SerializeField] private Transform player = null;//Player position/transform
     private Vector3 lastKnownPos = Vector3.zero;//Last known player position
     private AudioSource SNDSource;//zombieHead should always have 2 audioSources
 
     void Start () {
         //THIS IS USED TO GET THE LOCAL NAV MESH AGENT, wanderManager AND ANIMATOR OF THIS ZOMBIE
         agent = GetComponent<NavMeshAgent>();
         Anim = GetComponent<Animator>();
         SNDSource = zombieHead.GetComponent<AudioSource>();
 
         //SET THE MAIN VALUES
         agent.speed = moveSpeed;
         agent.acceleration = moveSpeed * 40;
         agent.angularSpeed = 999;
         resetZombie();
             
     }
 
     void Update(){
         /* IN CASE YOU CHANGE YOUR PLAYER GAMEOBJECT OR THE ZOMBIE IS SPAWNINg */
         if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
             return;
 
         /* ACTUAL ZOMBIE CODE */
 
         if(Time.time > lastCheck){//SEARCH FOR THE PLAYER AFTER INTERVAL
             checkView();
             lastCheck = Time.time + playerSearchInterval;
         }
 
         
             /* PLAYER SEARCH ALGORITHMS */
             if(playerChase && Anim.GetBool("isChasing")){
                 if(Time.time > lastChaseInterval){
                     gotoLastKnown();
                 }else{
                     chasePlayer();
                 }
             }
 
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
 
         //MAKE THE ZOMBIE WANDER AROUND THE MAP
         if(wandering){
             if(reachedPathEnd()){
                 resetZombie();
             }
         }
 
     }
 
     //Just to make this code prettier, simplify with functions...
     void checkView(){
         RaycastHit hit = new RaycastHit();
         Vector3 checkPosition = player.position - zombieHead.position;
         if(Vector3.Angle(checkPosition, zombieHead.forward) < fieldOfView){ //Check if player is inside the field of view
             if (Physics.Raycast(zombieHead.position, checkPosition, out hit, viewDistance, checkLayers)) {
                 if(hit.collider.tag == playerTag){//do this..
                     chasePlayer();
                     lastChaseInterval = Time.time + Random.Range(minChase, maxChase);
                 }
             }
         }else if(meleeDistance()){
             chasePlayer();
             lastChaseInterval = Time.time + Random.Range(minChase, maxChase);
         }
     }
 
     void gotoLastKnown(){
         Anim.SetBool("isChasing", true);
 
         if(canRun)
             Anim.SetBool("isRunning", true);
 
         Anim.SetBool("isIdle", false);
         playerChase = true;
         agent.SetDestination(lastKnownPos);
         agent.Resume();
         wandering = true;
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
         playSound(audioClips[1], false, false, true, true);
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
 
     void resetZombie(){
         Anim.SetBool("isIdle", true);
         Anim.SetBool("isChasing", false);
 
         if(canRun)
             Anim.SetBool("isRunning", false);
 
         playerChase = false;
         agent.Stop();
 
         wandering = true;
         playSound(audioClips[0], false, false, true, true);
     }
 
     void setNewWanderPos(Vector3 targetPos){
         Anim.SetBool("isIdle", false);
         Anim.SetBool("isChasing", true);
 
         if(canRun)
             Anim.SetBool("isRunning", true);
 
         playerChase = false;
         agent.SetDestination(targetPos);
         agent.Resume();
         lastWander = Time.time + Random.Range(minWander, maxWander);
         playSound(audioClips[2], false, false, true, true);
     }
 
     /* PARAMS:
      * Sound: Sound clip AudioClip
      * loop: boolean, to make the audio loop
      * randomStart: boolean, Start the clip at a random time
      * checkSameClip: boolean, prevent the replay of the audio if it is the same clip
      * isPlayingCheck: boolean, when preventing replay, should check if it's still playing while the same clip?
     */
     void playSound(AudioClip sound, bool loop, bool randomStart = false, bool checkSameClip = false, bool isPlayingCheck = false){
         if(checkSameClip && SNDSource.clip == sound && !SNDSource.isPlaying)
             return;
 
         if(isPlayingCheck && SNDSource.clip == sound && SNDSource.isPlaying)
             return;
 
         SNDSource.clip = sound;
         SNDSource.loop = loop;
 
         if(randomStart)
             SNDSource.time = Random.Range(0.0f, sound.length);
         
         SNDSource.Play();
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
 
     bool meleeDistance(){
         if(Vector3.Distance(transform.position, player.position) < 2.0f){
             return true;
         }
 
         return false;
     }
 }