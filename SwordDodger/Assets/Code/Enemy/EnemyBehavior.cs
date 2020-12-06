using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Basic Variables")]
    [SerializeField]
    float health = 50.0f;
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    LayerMask whatIsGround;
    [SerializeField]
    LayerMask whatIsPlayer;
    GameObject playerGO = null;
    Transform player;
    [SerializeField]
    Transform shootPoint = null;

    [Header("Patrol")]
    [SerializeField]
    Vector3 walkPoint;
    [SerializeField]
    float walkPointRange;
    bool walkPointSet;

    [Header("Attack")]
    [SerializeField]
    float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField]
    GameObject projectile;

    [Header("States")]

    [SerializeField]
    float sightRange;
    [SerializeField]
    float attackRange;
    //[SerializeField]
    public bool playerInSightRange;
    //[SerializeField]
    public bool playerInAttackRange;

    [Header("Sounds")]
    [SerializeField]
    Sound hurtSound = null;
    [SerializeField]
    Sound deadSound = null;
    [SerializeField]
    Sound shootSound = null;

    [Header("Animator")]
    [SerializeField]
    Animator animator = null;

    bool isDead = false;
    Transform cam;

    // Start is called before the first frame update
    private void Start()
    {
        {
            //Sounds
            deadSound.Init();
            hurtSound.Init();
            //Camera
            cam = Camera.main.transform;
            //Player and agent
            playerGO = PlayerManager.instance.Player;
            player = playerGO.transform;
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Awake()
    {
        //Sounds
        deadSound.Init();
        hurtSound.Init();
        //Camera
        cam = Camera.main.transform;
        //Player and agent
        //playerGO = PlayerManager.instance.Player;
        //player = playerGO.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!isDead)
        {
            if (!playerInSightRange && !playerInAttackRange) Patrol();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        //Debug.Log("I follow");
        //animator.SetBool("isFollowing", true);
        //follow the player
        agent.SetDestination(player.position);
    }
    void AttackPlayer()
    {
        if (!isDead)
        {
            //Debug.Log("I attack");
            //Stop and look at the player
            agent.SetDestination(transform.position);
            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                //Attack code: raycast, projectile, etc.
                //Rigidbody rb = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                GameObject proj = Instantiate(projectile, shootPoint.transform.position + transform.forward.normalized, Quaternion.identity);
                proj.GetComponent<SimpleProjectile>().setOwner(this.gameObject);
                shootSound.Play(transform);
                //
                //Cooldown
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float amount)
    {
        if (!isDead)
        {
            hurtSound.Play(transform);
            health -= amount;
            if (health <= 0)
                Die();
        }
    }
    void Die()
    {
        deadSound.Play(transform);
        //animator.SetTrigger("dead");
        //is dead, so no more "takeDamage"
        isDead = true;
        Destroy(gameObject, 15.0f);
    }
    //------------------------------------FACE PLAYER------------------------------------------
    //the gameobject looks at the player

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
