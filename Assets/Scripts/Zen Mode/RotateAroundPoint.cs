using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    public bool lookAtTarget;
    public Transform target;
    public Transform stopPoint;
    public float distanceToStop = 1.5f;
    public float moveSpeed = 0.1f;
    public float rotationSpeed = 60f;
    public bool stopWhenReachingPoint = true;
    bool isActive = true;

    private void OnEnable()
    {
        isActive = true;
    }

    private void Update()
    {
        if (isActive)
        {
            if (Vector3.Distance(transform.position, target.position) > distanceToStop)
            {
                transform.position = Vector3.MoveTowards(transform.position, stopPoint.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                if (stopWhenReachingPoint)
                {
                    isActive = false;
                    enabled = false;
                    gameObject.SetActive(false);
                }
            }
            if (lookAtTarget)
            {
                transform.LookAt(target);
            }
            transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
        }
    }
}
