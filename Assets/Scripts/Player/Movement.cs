using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] InputActionMap controls = new InputActionMap();

    [Space(10)]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotSpeed = 15.0f;

    [Space(10)]
    [SerializeField] Transform head = null;
    [SerializeField] Transform colliderPos = null;

    CapsuleCollider col = null;
    Rigidbody rb = null;

    //[HideInInspector]
    public bool isPaused = false;

    bool grounded = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = colliderPos.GetComponent<CapsuleCollider>();

        CalcCollider();
    }

    private void Update()
    {
        CalcCollider();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        // Rotate the Player
        Vector3 pos = head.position;
        transform.Rotate(Vector3.up, controls["Turn"].ReadValue<Vector2>().x * rotSpeed * Time.deltaTime, Space.Self);
        pos -= head.position;
        transform.position += pos;

        if (isPaused)
            return;

        // Move the Player
        Vector2 moveInput = controls["Move"].ReadValue<Vector2>() * moveSpeed;

        Vector3 vel = new Vector3(moveInput.x, 0, moveInput.y);
        vel = head.rotation * vel;
        vel.y = rb.velocity.y;

        if (!grounded)
            vel.y += Physics.gravity.y * Time.deltaTime;

        rb.velocity = vel; // TODO: Recode for Kinematic movement
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void GroundCheck()
    {
        float height = (((head.position.y - transform.position.y) / transform.localScale.y) / 2) - col.radius;

        grounded = Physics.SphereCast(colliderPos.position, col.radius - 0.05f, Vector3.down, out RaycastHit hit, height + 0.06f);
    }

    void CalcCollider()
    {
        float height = (head.position.y - transform.position.y) / transform.localScale.y;

        Vector3 colPos = colliderPos.localPosition;
        colPos.y = height / 2;
        colliderPos.localPosition = colPos;
        colPos.x = head.position.x;
        colPos.y = colliderPos.position.y;
        colPos.z = head.position.z;
        colliderPos.position = colPos;

        height -= col.radius;
        col.height = (height > 0) ? height : 0;
    }
}
