using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform viewPoint;
    [SerializeField] float mouseSensitivity = 1f;
    private float verticalRotStore = 0f;
    private Vector2 mouseInput;
    public bool invertLook = false;
    
    [Header("Movement data")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] bool running = true;
    private Vector3 moveDir;
    public CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity ;
        ProcessLookInput();
        Movement();
    }

    private void ProcessLookInput()
    {
        transform.Rotate(Vector3.up * mouseInput.x);
        verticalRotStore -= mouseInput.y;
        if (invertLook)
        {
            verticalRotStore += mouseInput.y * 2;
        }
        verticalRotStore = Mathf.Clamp(verticalRotStore, -80f, 60f);
     
        
        viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }

    private void Movement()
    {
        float ix = Input.GetAxisRaw("Horizontal");
        float iz = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(ix, 0f, iz);
        //controller.Move(walkSpeed*moveDir*Time.deltaTime);
        float currentSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = walkSpeed;
            running = false;
        }
        else
        {
            currentSpeed = runSpeed;
            running = true;
        }
        Vector3 moveVector = transform.forward*moveDir.z + transform.right*moveDir.x;
        controller.Move(moveVector*currentSpeed*Time.deltaTime);
        //transform.position += moveVector*currentSpeed*Time.deltaTime;
    }
}
