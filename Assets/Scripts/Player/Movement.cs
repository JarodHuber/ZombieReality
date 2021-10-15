using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] InputActionMap controls = new InputActionMap();

    [Space(10)]
    [SerializeField] KinematicPlayerMotor motor = null;
    [SerializeField] float rotSpeed = 15.0f;

    [Space(10)]
    [SerializeField] Transform head = null;
    [SerializeField] Transform colliderPos = null;

    BoxCollider col = null;
    Rigidbody rb = null;

    public bool isPaused = false;

    public float Height { get => (head.position.y - transform.position.y) / transform.localScale.y; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = colliderPos.GetComponent<BoxCollider>();

        CalcCollider();
    }

    private void Update()
    {
        CalcCollider();
    }

    private void FixedUpdate()
    {
        // Rotate the Player

        float rotInput = controls["Turn"].ReadValue<Vector2>().x;
        if (rotInput != 0)
        {
            Vector3 pos = head.position;
            rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(rotInput * rotSpeed * Time.deltaTime, Vector3.up));
            pos -= head.position;
            transform.position += pos;
        }

        if (isPaused)
            return;

        // Move the Player
        Vector3 input = controls["Move"].ReadValue<Vector2>();
        input.z = input.y;
        input.y = 0.0f;

        input = head.rotation * input;
        input.y = 0.0f;

        motor.MoveInput(input); // TODO: Recode for Kinematic movement
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void CalcCollider()
    {
        Vector3 colDataHolder = colliderPos.localPosition;

        colDataHolder.y = Height / 2.0f;
        colliderPos.localPosition = colDataHolder;

        colDataHolder = head.position;
        colDataHolder.y = colliderPos.position.y;
        colliderPos.position = colDataHolder;

        colDataHolder = col.size;
        colDataHolder.y = (Height > 0) ? Height : 0;

        col.size = colDataHolder;
    }
}
