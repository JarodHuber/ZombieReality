using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class HolsterRig : MonoBehaviour
{
    public List<Holster> holsters { get; private set; }
    [SerializeField] Transform head;
    [SerializeField] float rotSmooth = 0.3f;
    [SerializeField] float angleBeforeRot = 30.0f;
    [SerializeField] InputAction moveStick = null;


    bool rotating = false;

    private void Start()
    {
        holsters = transform.GetComponentsInChildren<Holster>().ToList();
    }

    private void FixedUpdate()
    {
        transform.position = head.position;

        Vector3 forward = head.forward;
        forward.y = transform.forward.y;

        Vector2 movement = moveStick.ReadValue<Vector2>();

        if (rotating || (Mathf.Abs(movement.y) > Mathf.Abs(movement.x)) || Vector3.Angle(transform.forward, forward) > angleBeforeRot)
        {
            rotating = true;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(forward), rotSmooth * Time.deltaTime);
        }
        else if (rotating && Vector3.Angle(transform.forward, forward) == 0.0f)
            rotating = false;
    }
}
