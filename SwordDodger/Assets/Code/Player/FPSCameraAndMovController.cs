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

    [Header("Opciones de camara")]
    public Camera cam;
    public float mouseHorizontal = 3.0f;
    public float mouseVertical = 2.0f;
    public float minRotation = -65.0f;
    public float maxRotation = 60.0f;
    
    float h_mouse, v_mouse;
    private Vector3 move = Vector3.zero;
    public bool vertCameraLocked = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

    }



    void Update()
    {

        h_mouse = mouseHorizontal * Input.GetAxis("Mouse X");
        v_mouse += mouseVertical * Input.GetAxis("Mouse Y");

        v_mouse = Mathf.Clamp(v_mouse, minRotation, maxRotation);

        cam.transform.localEulerAngles = new Vector3(-v_mouse, 0, 0);
        transform.Rotate(0, h_mouse, 0);

        if (characterController.isGrounded)
        {
            //We may lock vert camera for the blade mode
            if (!vertCameraLocked)
            {
                move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            }
            else
                move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);

            /*
            if (Input.GetKey(KeyCode.LeftShift))
                move = transform.TransformDirection(move) * runSpeed;
            else*/

            move = transform.TransformDirection(move) * walkSpeed;

            if (Input.GetKey(KeyCode.Space))
                move.y = jumpSpeed;
        }
        move.y -= gravity * Time.deltaTime;

        characterController.Move(move * Time.deltaTime);
    }

    public void LockVerticalCamera(bool what)
    {
        vertCameraLocked = what;
    }
    //No se usa
    public void Shake(float shakeMagnitude, float shakeRoughness, float shakeFadeInTime, float shakeFadeOutTime)
    {
        CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);

    }

}
