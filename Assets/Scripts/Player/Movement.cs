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

    RaycastHit holder = new RaycastHit();

    public float Height { get => (head.position.y - transform.position.y) / transform.localScale.y; }

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

        rb.isKinematic = isPaused;
        if (isPaused)
            return;

        // Move the Player
        Vector2 moveInput = controls["Move"].ReadValue<Vector2>() * moveSpeed;

        Vector3 vel = new Vector3(moveInput.x, 0, moveInput.y);
        vel = head.rotation * vel;
        vel.y = rb.velocity.y;

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
        grounded = Physics.SphereCast(head.position, col.radius / 2.0f, 
            Vector3.down, out holder, Height);
    }

    void CalcCollider()
    {
        Vector3 colPos = colliderPos.localPosition;
        colPos.y = Height / 2.0f;
        colliderPos.localPosition = colPos;
        colPos.x = head.position.x;
        colPos.y = colliderPos.position.y;
        colPos.z = head.position.z;
        colliderPos.position = colPos;

        float colHeight = Height - col.radius;
        col.height = (colHeight > 0) ? colHeight : 0;
    }
}
