 using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
 

 public Vector3 walkPoint;
 bool walkPointSet;
 public float walkPointRange;

 public GameObject projectile;

 public float health;


 public float timeBetweenAttacks;
 bool alreadyAttacked;


 public float sightRange,attackRange;
 public bool playerInSightRange, playerInAttackRange;

 private void Awake()
 {
player = GameObject.Find("player").transform;
agent = GetComponent<NavMeshAgent>();
 }
private void Update()
{
playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
if(!playerInSightRange && !playerInAttackRange) Patroling();
if(playerInSightRange && !playerInAttackRange) ChasePlayer();
if(playerInAttackRange && playerInSightRange) AttackPlayer();
}
 private void Patroling()
 {
if (!walkPointSet) SearchWalkPoint();

if ( walkPointSet) 
agent.SetDestination(walkPoint);

Vector3 distanceToWalkPoint = transform.position - walkPoint;

if(distanceToWalkPoint.magnitude < 1f)
walkPointSet = false;
 }

 private void SearchWalkPoint(){
    float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
    float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y,transform.position.z + randomZ);
    if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
    walkPointSet = true;

 }

  private void ChasePlayer()
 {
   agent.SetDestination(player.position);
 }

 private void AttackPlayer()
 {
agent.SetDestination(transform.position);
transform.LookAt(player);

       if(!alreadyAttacked)
{
    Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
     rb.AddForce(transform.up * 8f, ForceMode.Impulse);
    
    
    
    alreadyAttacked = true;
    Invoke(nameof(ResetAttack),timeBetweenAttacks);
}
 }
 private void ResetAttack ()
 {
      alreadyAttacked = false;
 }
 public void TakeDamage(int damage)
 {
health -= damage;
if(health<=0) Invoke(nameof(DestroyEnemy),.5f);
 }

 private void DestroyEnemy()
 {
    Destroy(gameObject);
 }
 private void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
     Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, sightRange);
    

 }
}
