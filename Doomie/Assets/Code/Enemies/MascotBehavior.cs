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
    [Header("Following")]
    [SerializeField]
    float playerSightRange;
    [SerializeField]
    float spreadFactorX;
    [SerializeField]
    float spreadFactorY;
    [Header("States")]

    [SerializeField]
    float sightRange;
    [SerializeField]
    float attackRange;
    //[SerializeField]
    public bool playerInSightRange;
    public bool enemyInSightRange;
    //[SerializeField]
    public bool enemyInAttackRange;
    [Header("Sounds")]
    [SerializeField]
    Sound hurtSound = null;
    [SerializeField]
    Sound deadSound = null;
    [SerializeField]
    Sound barkSound = null;
    [SerializeField]
    Sound seesEnemySound = null;
    [SerializeField]
    Sound followsPlayerSound = null;
    [SerializeField]
    Sound wanderSound = null;
    [SerializeField]
    Sound attackSound = null;
    [SerializeField]
    Sound footstepsSound = null;

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
            wanderSound.Init();
            barkSound.Init();
            footstepsSound.Init();
            attackSound.Init();
            followsPlayerSound.Init();
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
        wanderSound.Init();
        barkSound.Init();
        footstepsSound.Init();
        attackSound.Init();
        followsPlayerSound.Init();

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
        //Check if mascot sees player
        playerInSightRange = Physics.CheckSphere(transform.position, playerSightRange, whatIsPlayer);

        if (!isDead)
        {
            if (Input.GetButtonDown("CallMascot")) FollowPlayer();
            if (!playerInSightRange && !enemyInSightRange && !enemyInAttackRange) FollowPlayer();
            if (!enemyInSightRange && !enemyInAttackRange) Wander();
            if (enemyInSightRange && !enemyInAttackRange) ChaseEnemy();
            if (enemyInAttackRange && enemyInSightRange) AttackEnemy();
        }

    }
    private void Wander()
    {
        wanderSound.Play(transform);
    }
    private void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= spreadFactorX)
        {
            //Already too close, bark
            Debug.Log("Woof");
            barkSound.Play(transform);
        }
        else
        {
            animator.SetBool("isFollowing", true);
            followsPlayerSound.Play(transform);
            Vector3 playerLocation = player.position;
            //A minimun distance from the player
            while (Vector3.Distance(transform.position, player.position) >= spreadFactorX)
            {
                playerLocation.x += Random.Range(-spreadFactorX, spreadFactorX);
                playerLocation.y += Random.Range(-spreadFactorY, spreadFactorY);

            }
            //follow the player
            agent.SetDestination(playerLocation);
        }
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
        seesEnemySound.Play(transform);
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
                attackSound.Play(transform);
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerSightRange);
    }

}
