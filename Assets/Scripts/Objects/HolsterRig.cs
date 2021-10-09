using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HolsterRig : MonoBehaviour
{
    public List<Holster> holsters { get; private set; }
    [SerializeField] Transform head;
    [SerializeField] float rotSmooth = 0.3f;
    [SerializeField] float angleBeforeRot = 30.0f;


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

        if (rotating || Vector3.Angle(transform.forward, forward) > angleBeforeRot)
        {
            rotating = true;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(forward), rotSmooth * Time.deltaTime);
        }
        else if (rotating && Vector3.Angle(transform.forward, forward) == 0.0f)
            rotating = false;
    }
}
