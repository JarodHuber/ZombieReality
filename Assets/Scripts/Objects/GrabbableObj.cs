using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObj : MonoBehaviour
{
    [SerializeField] protected float gravity = 20;

    protected Hand hand = null;
    Rigidbody rb = null;
    ThrowStamp stamp = new ThrowStamp();

    protected FixedJoint joint = null;
    public bool IsGrabbed { get => hand; }

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (hand)
        {
            stamp = new ThrowStamp(transform.position, stamp, Time.deltaTime);
            return;
        }

        rb.AddForce(0, -gravity, 0);
    }

    public virtual void Grab(Hand handGrabbing)
    {
        hand = handGrabbing;

        //joint = gameObject.AddComponent<FixedJoint>();
        //joint.connectedBody = handGrabbing.Rigidbody;

        rb.isKinematic = true;
        transform.SetParent(handGrabbing.transform);

        stamp = new ThrowStamp(transform.position);
    }

    public virtual void Release()
    {
        //Destroy(joint);
        hand = null;

        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(stamp.Velocity, ForceMode.Impulse);
    }

    public virtual void SwapHands(Hand newHand)
    {
        hand.SwapHands();

        hand = newHand;
        transform.SetParent(hand.transform);
        //joint.connectedBody = hand.Rigidbody;
        hand.SwapHands();

        stamp = new ThrowStamp(transform.position);
    }

    struct ThrowStamp
    {
        private Vector3 pos;
        private Vector3 velDir;
        private float velForce;

        public Vector3 Velocity { get => velDir.normalized * velForce; }

        public ThrowStamp(Vector3 position)
        {
            pos = position;
            velDir = new Vector3();
            velForce = 0.0f;

        }
        public ThrowStamp(Vector3 position, ThrowStamp prevStamp, float deltaTime)
        {
            pos = position;

            Vector3 curVel = (pos - prevStamp.pos) / deltaTime;

            velForce = curVel.magnitude;
            velDir = curVel;

            if(Vector3.Angle(prevStamp.velDir, curVel) < 45.0f)
                velDir += prevStamp.velDir;
        }
    }
}
