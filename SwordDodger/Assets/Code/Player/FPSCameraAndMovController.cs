using Cinemachine;
using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraAndMovController : MonoBehaviour
{

    CharacterController characterController;

    [Header("Opciones de personaje")]
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float health = 100f;
    public float blinkDistance = 5.0f;
    public float dashTime;
    public float dashSpeed;
    public Vector3 moveDir;

    [Header("Opciones de camara")]
    public CinemachineVirtualCamera cam;
    public float mouseHorizontal = 3.0f;
    public float mouseVertical = 2.0f;
    public float minRotation = -65.0f;
    public float maxRotation = 60.0f;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem dashParticle;
    [SerializeField]
    ParticleSystem bounceParticle;

    [Header("Sonido")]
    [SerializeField]
    Sound hurtSound = null;
    [SerializeField]
    Sound deadSound = null;
    [SerializeField]
    Sound walkSound = null;
    [SerializeField]
    float timeBetweenSteps = 0.2f;
    [SerializeField]
    Sound jumpSound = null;
    [SerializeField]
    Sound dashSound = null;
    [SerializeField]
    Sound bounceSound = null;

    bool playerIsMoving = false;
    float h_mouse, v_mouse;
    private Vector3 move = Vector3.zero;
    public bool vertCameraLocked = false;
    bool isDead = false;
    public Animator animator;

    //Headbob
    private bool left;
    private bool right;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        //animator = GetComponent<Animator>();
        jumpSound.Init();
        walkSound.Init();

        InvokeRepeating("CallFootsteps", 0, timeBetweenSteps);
    }

    void Update()
    {

        h_mouse = mouseHorizontal * Input.GetAxis("Mouse X");

        //We may lock vert camera for the blade mode
        if (!vertCameraLocked)
        {
            v_mouse += mouseVertical * Input.GetAxis("Mouse Y");
        }

        v_mouse = Mathf.Clamp(v_mouse, minRotation, maxRotation);

        cam.transform.localEulerAngles = new Vector3(-v_mouse, 0, 0);
        transform.Rotate(0, h_mouse, 0);

        if (Input.GetButtonDown("Dash") && characterController.velocity.magnitude > 0.1f)
        {
            dashSound.PlayOneShot(transform);
            StartCoroutine(Dash());
            dashParticle.Play();
        }

        //Movement
        if (characterController.isGrounded)
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

            /*
            if (Input.GetKey(KeyCode.LeftShift))
                move = transform.TransformDirection(move) * runSpeed;
            else*/

            move = transform.TransformDirection(move) * walkSpeed;
            animator.SetBool("IsMoving", true);

            if (Input.GetKey(KeyCode.Space))
                move.y = jumpSpeed;
        }

        //Paramos
        if (characterController.velocity.magnitude < 0.1f)
        {
            animator.SetBool("IsMoving", false);

        }

        move.y -= gravity * Time.deltaTime;

        //Direction of the movement, to use dash
        moveDir = move.normalized;

        characterController.Move(move * Time.deltaTime);
    }

    IEnumerator Dash()
    {

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(moveDir * dashSpeed * Time.deltaTime);
            yield return null;

        }

    }

    public void LockVerticalCamera(bool what)
    {
        vertCameraLocked = what;
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
        isDead = true;
        //Sound and anim
        deadSound.Play(transform);
        animator.SetTrigger("Dead");
        StartCoroutine(DeathSequence(0.85f));

        /*
        deadSound.Play(transform);
        animator.SetTrigger("Dead");
        //is dead, so no more "takeDamage"
        isDead = true;
        //Debug.Log("Im dead");
        this.gameObject.SetActive(false);

        Time.timeScale = 0.0f;
        LevelManager.instance.PlayerDied();
        */
        //Destroy(gameObject, 15.0f);
    }
    IEnumerator DeathSequence(float deathTime)
    {
        yield return new WaitForSeconds(deathTime);
        //Debug.Log("Im dead");
        //this.gameObject.SetActive(false);
        characterController.gameObject.SetActive(false);

        Time.timeScale = 0.0f;
        LevelManager.instance.PlayerDied();
    }

    void CallFootsteps()
    {
        if (playerIsMoving == true)
        {
            //Debug.Log("Step Sound");
            //Debug.Log ("Player is moving");
            walkSound.Play(transform);
        }

    }

    public void PlayBounce()
    {
        Debug.Log("Animacion y Sonido de rebote!");
        bounceParticle.Play();
        bounceSound.Play(transform);
    }

}
