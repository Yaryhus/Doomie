﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLookController : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 100f;
    [SerializeField]
    Transform playerBody = null;

    float xRotation = 0f;
    bool vertCameraLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        //Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        //We may lock vert camera for the blade mode
        if (!vertCameraLocked)
        {

            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
        }

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);      
        playerBody.Rotate(Vector3.up * mouseX);


    }
    public void LockVerticalCamera(bool what)
    {
            vertCameraLocked = what;
    }
}
