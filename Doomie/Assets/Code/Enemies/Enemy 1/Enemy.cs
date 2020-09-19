using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField]
    float health = 50.0f;
    [SerializeField]
    GameObject visionCone  = null;
    [SerializeField]
    GameObject soundDetection  = null;

    [Header("Sounds")]
    [SerializeField]
    Sound hurtSound  = null;
    [SerializeField]
    Sound deadSound  = null;

    [Header("Graphics")]
    [SerializeField]
    GameObject enemyGraphic  = null;

    [Header("Animator")]
    [SerializeField]
    Animator animator  = null;
    GameObject player  = null;
    Transform cam;
    bool isDead = false;
    bool isPlayerSpotted = false;
    
    public void Start()
    {
        player = PlayerManager.instance.Player;
        //player = GameObject.FindGameObjectWithTag("Player");
        deadSound.Init();
        hurtSound.Init();
        cam = Camera.main.transform;
    }

    //------------------------------------STATES------------------------------------------
    public void PlayerSpotted()
    {
        animator.SetBool("isFollowing", true);
        isPlayerSpotted = true;
    }

    //------------------------------------DAMAGE------------------------------------------
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
        if (isPlayerSpotted)
            FacePlayer();
        //graphic as billboard
        enemyGraphic.transform.forward = cam.transform.forward;
    }
}
