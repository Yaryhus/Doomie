using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MascotBehavior : MonoBehaviour
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
    [SerializeField]
    LayerMask whatIsEnemy;
    GameObject playerGO = null;
    Transform player;
    Transform enemy;
    /*
        [Header("Patrol")]
        [SerializeField]
        Vector3 walkPoint;
        [SerializeField]
        float walkPointRange;
        bool walkPointSet;
    */
    [Header("Attack")]
    [SerializeField]
    float timeBetweenAttacks;
    [SerializeField]
    float meleeDamageAmmount = 15.0f;
    bool alreadyAttacked;
    [SerializeField]
    GameObject projectile;

    [Header("States")]

    [SerializeField]
    float sightRange;
    [SerializeField]
    float attackRange;
    //[SerializeField]
    public bool enemyInSightRange;
    //[SerializeField]
    public bool enemyInAttackRange;

    [Header("Sounds")]
    [SerializeField]
    Sound hurtSound = null;
    [SerializeField]
    Sound deadSound = null;

    [Header("Animator")]
    [SerializeField]
    Animator animator = null;

    [Header("Graphics")]
    [SerializeField]
    GameObject enemyGraphic = null;

    bool isDead = false;
    Transform cam;
    Collider[] hitColliders;

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
        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
        if (!isDead)
        {
            if (!enemyInSightRange && !enemyInAttackRange) FollowPlayer();
            if (enemyInSightRange && !enemyInAttackRange) ChaseEnemy();
            if (enemyInAttackRange && enemyInSightRange) AttackEnemy();
        }

    }

    private void FollowPlayer()
    {
        /*
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
            */

        //Debug.Log("I follow");
        animator.SetBool("isFollowing", true);
        //follow the player
        agent.SetDestination(player.position);
    }
    /*
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    */
    void ChaseEnemy()
    {
        //We get all the enemies whitin the mascot radius
        hitColliders = Physics.OverlapSphere(transform.position, sightRange, whatIsEnemy);
        enemy = hitColliders[0].gameObject.transform;
        foreach (Collider d in hitColliders)
        {
            Debug.Log(d.gameObject.name);
        }
        animator.SetBool("isFollowing", true);
        //follow the enemy
        agent.SetDestination(enemy.position);
    }
    void AttackEnemy()
    {
        if (!isDead)
        {
            hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);
            enemy = hitColliders[0].gameObject.transform;
            //Debug.Log("I attack");
            //Stop and look at the enemy
            agent.SetDestination(transform.position);
            transform.LookAt(enemy);

            if (!alreadyAttacked)
            {
                //Attack code: raycast, projectile, etc.
                Debug.Log("Woof woof ataco");
                enemy.GetComponent<EnemyBehavior>().TakeDamage(meleeDamageAmmount);

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
        animator.SetTrigger("dead");
        //is dead, so no more "takeDamage"
        isDead = true;
        Destroy(gameObject, 15.0f);
    }
    //------------------------------------FACE PLAYER------------------------------------------
    //the gameobject looks at the player


    void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    //Look at the user
    private void LateUpdate()
    {
        //if on a chase look at the player
        FacePlayer();
        //graphic as billboard
        enemyGraphic.transform.forward = cam.transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
