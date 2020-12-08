using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretBehavior : MonoBehaviour
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
    [SerializeField]
    GameObject body = null;

    [Header("Attack")]
    [SerializeField]
    float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField]
    GameObject projectile;

    [Header("States")]

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
    [SerializeField]
    Sound alertedSound = null;

    [Header("Animator")]
    [SerializeField]
    Animator animator = null;

    bool isDead = false;
    Transform cam;
    private Ray sight;

    // Start is called before the first frame update
    private void Start()
    {
        {
            //Sounds
            deadSound.Init();
            hurtSound.Init();
            alertedSound.Init();
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
        sight.origin = new Vector3(shootPoint.transform.position.x, shootPoint.transform.position.y + 0.5f, shootPoint.transform.position.z);
        sight.direction = shootPoint.transform.forward;
        RaycastHit rayHit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
    
        Debug.DrawLine(transform.position, player.position, Color.cyan);
        if (Physics.Linecast(transform.position, player.position, out rayHit))
        {
            //Debug.DrawLine(sight.origin, rayHit.point, Color.red);
            if (rayHit.collider.tag == "Player")
            {
                playerInSightRange = true;
            }

            if (rayHit.collider.tag != "Player" && rayHit.collider.tag != "Enemy")
            {
                playerInSightRange = false;
            }
        }

        //if enemy loses body = dies by being cut in half
        if (body == null)
            Die();

        //Check for sight and attack range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!isDead)
        {
            if (!playerInSightRange && !playerInAttackRange) Patrol();
            if (playerInSightRange && !playerInAttackRange) Patrol();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

    }
    void Patrol()
    {
        gameObject.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 1f);
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
        Destroy(gameObject);
    }
    //------------------------------------FACE PLAYER------------------------------------------
    //the gameobject looks at the player

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
