using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class RandomToughtMovement : MonoBehaviour
{
    Animator anim;
    public CinemachineVirtualCamera[] vCam;
    public Transform vcam2Pos;
    RotateAroundPoint rotateAroundPoint;
    
    public float moveTimeScale = 0.7f;
    public float rotationDelay = 0.5f;
    public float maxX = 40.0f;
    public float maxZ = 40.0f;
    Sequence moveSequence;
    Vector3 targetPoint;
    public float waitForNextMoveDelay = 2f;

    IEnumerator moveCoroutine;
    bool isMoving = true;

    private void Start()
    {
        vCam[0].gameObject.SetActive(true);
        vCam[1].gameObject.SetActive(false);
        rotateAroundPoint = vCam[1].gameObject.GetComponent<RotateAroundPoint>();
        rotateAroundPoint.enabled = false;
        vCam[2].gameObject.SetActive(true);

        anim = GetComponent<Animator>();
        MovePlayer();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {

            PlayerIsThinking();
        }
    }

    public void MovePlayer()
    {
        StopPlayer();
        isMoving = true;
        moveCoroutine = MovePlayerIE();
        StartCoroutine(moveCoroutine);

    }
    IEnumerator MovePlayerIE()
    {
        while(isMoving)
        {
            //targetPoint = new Vector3(Random.Range(-1 * maxX, maxX), 0, Random.Range(-1 * maxZ, maxZ));
            targetPoint = new Vector3(transform.position.x + Random.Range(-1 * maxX, maxX), 0, transform.position.z + Random.Range(-1 * maxZ, maxZ));
            var rotation = Quaternion.LookRotation(targetPoint - transform.position).eulerAngles;
            transform.DORotate(rotation, rotationDelay);
            yield return new WaitForSeconds(rotationDelay);
            anim.SetFloat("move", 0.5f);

            moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMove(targetPoint, GetTimeMovement()).SetEase(Ease.Linear));

            transform.DOMove(targetPoint, GetTimeMovement()).SetEase(Ease.Linear);
            yield return new WaitForSeconds(GetTimeMovement());
            anim.SetFloat("move", 0);
            yield return new WaitForSeconds(waitForNextMoveDelay);
        }
    }
    public void StopPlayer()
    {
        if (moveCoroutine != null)
        {
            isMoving = false;
            StopCoroutine(moveCoroutine);
        }
        if(moveSequence != null)
        {
            moveSequence.Kill();
        }
        anim.SetFloat("move", 0);
    }

    float GetTimeMovement()
    {
        return Vector3.Distance(transform.position, targetPoint) * moveTimeScale;
    }

    public void PlayerIsThinking(bool isThinking = true)
    {
        if(isThinking)
        {
            StopPlayer();
            vCam[0].gameObject.SetActive(false);
            vCam[1].transform.position = vcam2Pos.position;
            vCam[1].gameObject.SetActive(true);
        }
        else
        {
            MovePlayer();
            vCam[0].gameObject.SetActive(true);
            vCam[1].gameObject.SetActive(false);
        }
    }

    public void PlayerGotFacts()
    {
        rotateAroundPoint.enabled = true;
    }

}
