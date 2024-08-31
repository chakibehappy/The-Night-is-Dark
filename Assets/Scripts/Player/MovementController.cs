using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Aceline.RPG;

public class MovementController : MonoBehaviour
{
    GameMaster GM;

    MainGame input;
    NavMeshAgent agent;

    InputAction move;
    InputAction lookAction;

    [Header("Point And Click")]
    [SerializeField] ParticleSystem clickFx;
    [SerializeField] LayerMask clickableLayers;
    float lookRotationSpeed = 8f;
    bool isMovingByClick = false;
    IEnumerator moveByClickCroutine;


    [Header("Movement by Axis")]
    public float moveSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private Vector2 moveValue;
    CharacterController controller;
    Vector3 movement;

    [SerializeField] LayerMask NPCLayer;
    GameObject activeNPC = null;
    GameObject clickedObject = null;
    GameBoard activeDialogueBoard = null;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        input = new MainGame();
        AssignInputs();
    }

    void Start()
    {
        GM = GameMaster.GM;
        agent.speed = moveSpeed;
    }


    void Update()
    {
        if (GM == null)
            return;

        if (GM.canMove && !GM.cameraIsChange)
        {
            MovementByAxis();
            FaceTarget();
        }
    }

    void AssignInputs()
    {
        input.Player.PointAndClick.performed += ctx => PointAndClick();
    }

    void PointAndClick()
    {
        if (GM == null)
            return;

        if (activeNPC != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, NPCLayer))
            {
                clickedObject = hit.collider.gameObject;
                if (clickedObject == activeNPC)
                    ShowDialogue();
                else
                    ClickToMove();
            }
            else
            {
                ClickToMove();
            }
        }
        else
        {
            ClickToMove();
        }
    }

    void ClickToMove()
    {
        if (!GM.canMove)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            clickedObject = hit.collider.gameObject;
            agent.enabled = true;
            isMovingByClick = true;
            GM.ShowActionInfo(false);
            agent.destination = hit.point;
            if(clickFx != null)
            {
                ParticleSystem fx = Instantiate(clickFx, hit.point += new Vector3(0, 0.1f, 0), clickFx.transform.rotation);
                Destroy(fx.gameObject, 1);
            }
            WaitUntilDestination();
        }
    }

    void WaitUntilDestination()
    {
        StopWaitUntilDestination();
        moveByClickCroutine = WaitTillDestinationIE();
        StartCoroutine(moveByClickCroutine);
    }
    void StopWaitUntilDestination()
    {
        if(moveByClickCroutine != null)
        {
            StopCoroutine(moveByClickCroutine);
            moveByClickCroutine = null;
        }
    }
    IEnumerator WaitTillDestinationIE()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
        isMovingByClick = false;
    }

    public void ShowDialogue(bool isShow = true)
    {
        GM.ShowActionInfo(!isShow);
        GM.canMove = !isShow;
        if (isShow)
        {
            input.Disable();
            move.Disable();
            GM.StartDialogue(this, activeDialogueBoard);
        }
        else
        {
            StartCoroutine(EnableMovementInputIE());
        }
    }

    IEnumerator EnableMovementInputIE()
    {
        yield return new WaitForSeconds(0.1f);
        input.Enable();
        move.Enable();
    }

    void OnEnable()
    {
        input.Enable();
        move = input.Player.Move;
        move.Enable();
        lookAction = input.Player.Look;
        lookAction.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        move.Disable();
        lookAction.Disable();
    }


    private void MovementByAxis()
    {
        moveValue = move.ReadValue<Vector2>();
        movement = new Vector3(moveValue.x, 0, moveValue.y).normalized;

        if (movement.magnitude > 0.1f)
        {
            StopMovementFromClick();

            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            //animator.SetFloat("move", 1);
        }
        else
        {
            //animator.SetFloat("move", 0);
        }
    }

    void StopMovementFromClick()
    {
        if (agent.isActiveAndEnabled)
        {
            isMovingByClick = false;
            StopWaitUntilDestination();
            agent.enabled = false;
        }
    }

    void FaceTarget()
    {
        if (isMovingByClick)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            if (direction.sqrMagnitude == 0f)
                return;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if(activeNPC == null)
            {
                activeNPC = other.gameObject;
                NPCController npc = activeNPC.GetComponent<NPCController>();
                activeDialogueBoard = npc.dialogueBoard;

                if (isMovingByClick)
                {
                    StopMovementFromClick();
                    if (clickedObject == activeNPC)
                    {
                        ShowDialogue(true);
                    }
                    else
                    {
                        GM.ShowActionInfo(true, "Talk");
                    }
                }
                else
                {
                    GM.ShowActionInfo(true, "Talk");
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            other.transform.LookAt(new Vector3(transform.position.x, other.transform.position.y, transform.position.z));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            activeNPC = null;
            GM.ShowActionInfo(false);
        }
    }
}
