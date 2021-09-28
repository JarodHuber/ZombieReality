using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PushableButton : MonoBehaviour
{
    [SerializeField] Vector3 startPos = new Vector3();
    [SerializeField] Vector3 targetPos = new Vector3();
    [SerializeField] float accelForce = 10.0f;
    [SerializeField] bool isHold = false;
    [Range(0.1f, 1.0f)]
    [SerializeField] float buttonSensitivity = 0.9f;
    [SerializeField] Rigidbody rb = null;
    [SerializeField] bool initializeOnStart = true;
    [SerializeField] UnityEvent onPress = new UnityEvent();

    bool isPressed = false;
    bool hasPressed = false;

    bool active = false;

    Vector3 TargetPosition { get => startPos + targetPos; }

    private void Start()
    {
        if(initializeOnStart)
            Initialize();
    }

    private void Update()
    {
        if (isPressed && (isHold || !hasPressed))
        {
            onPress.Invoke();
            hasPressed = true;
        }

        if (!isPressed)
            hasPressed = false;
    }

    private void FixedUpdate()
    {
        help();

        if(rb.position != startPos)
            rb.AddForce((startPos - TargetPosition).normalized * accelForce * Time.deltaTime, ForceMode.Acceleration);
    }

    public void Initialize()
    {
        startPos = transform.position;
        active = true;
    }

    void help()
    {
        if (!active)
            return;

        float u = Vector3.Dot(startPos - transform.position, startPos - TargetPosition);
        u /= Vector3.Dot(startPos - TargetPosition, startPos - TargetPosition);

        u = Mathf.Clamp(u, 0.0f, 1.0f);

        isPressed = u > buttonSensitivity;

        rb.MovePosition(startPos + (u * (TargetPosition - startPos)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(TargetPosition, 0.05f);
    }
}
