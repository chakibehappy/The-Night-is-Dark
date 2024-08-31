using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerController : MonoBehaviour
{
    public GameMaster GM;
    CharacterController controller;
    Animator animator;
    public CinemachineFreeLook thirdCam;
    
    public float moveSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private Vector3 movement;

    bool isRecenterCamera;

    protected void Start()
    {
        SetVariable();
    }

    protected void Update()
    {
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        InputHandler();
    }

    
    protected void FixedUpdate()
    {
        Movement();
    }

    private void SetVariable()
    {
        GM = GameMaster.GM;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        RecenterCamera();
    }

    private void InputHandler()
    {
        if (GM.canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (Input.GetMouseButton(2))
            {
                vertical = 1;
            }

            movement = new Vector3(horizontal, 0, vertical).normalized;


            if (Input.GetMouseButtonDown(1))
            {
                RecenterCamera();
            }
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool menuActive = GM.INVENTORY.inventoryUI.activeInHierarchy;
            thirdCam.gameObject.SetActive(menuActive);
            GM.canMove = menuActive;
            //GM.TIME.ResumeTime(menuActive);
            //GM.STAMINA.ResumeStamina(menuActive);
            GM.isOpenMenu = !menuActive;
            GM.INVENTORY.DisplayInventory(!menuActive);
        }
    }


    private void Movement()
    {
        if (movement.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            animator.SetFloat("move", 1);
        }
        else
        {
            animator.SetFloat("move", 0);
        }
    }

    public void RecenterCamera()
    {
        if (!isRecenterCamera)
        {
            isRecenterCamera = true;
            StartCoroutine(RecenterCameraIE());
        }
    }
    IEnumerator RecenterCameraIE()
    {
        thirdCam.m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForSeconds(2f);
        isRecenterCamera = false;
        thirdCam.m_RecenterToTargetHeading.m_enabled = false;
    }
}
