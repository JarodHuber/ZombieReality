using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObj : MonoBehaviour
{
    public enum ObjectType
    {
        DEFAULT = 0,
        GUN = 1,
        GRENADE = 1 << 1
    }

    [SerializeField] ObjectType type = ObjectType.DEFAULT;

    protected Hand hand = null;
    protected Rigidbody rb = null;
    protected FixedJoint joint = null;

    public bool IsGrabbed { get => hand; }

    ThrowStamp stamp = new ThrowStamp();
    Holster hoveredHolster = null;

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (hand)
        {
            stamp = new ThrowStamp(transform.position, stamp, Time.deltaTime);
            return;
        }
    }
    private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }

    /// <summary>
    /// Sets up the Object to be grabbed by the players hand
    /// </summary>
    /// <param name="handGrabbing">The relevant hand that will be grabbing the object</param>
    public virtual void Grab(Hand handGrabbing)
    {
        hand = handGrabbing;

        //joint = gameObject.AddComponent<FixedJoint>();
        //joint.connectedBody = handGrabbing.Rigidbody;

        if (hoveredHolster && hoveredHolster.isHolstering)
            hoveredHolster.isHolstering = false;
        else
            rb.isKinematic = true;

        transform.SetParent(handGrabbing.transform);

        stamp = new ThrowStamp(transform.position);
    }

    /// <summary>
    /// Sets up the object to be released by the player
    /// </summary>
    public virtual void Release()
    {
        hand = null;

        if (hoveredHolster && !hoveredHolster.isHolstering)
        {
            hoveredHolster.HolsterContains(type);
            transform.SetParent(hoveredHolster.transform);
            return;
        }

        //Destroy(joint);

        transform.SetParent(null);
        rb.isKinematic = false;
        stamp.print();
        rb.AddForce(stamp.Velocity, ForceMode.Impulse);
    }

    /// <summary>
    /// Sets up the object to be swapped to the player's other hand
    /// </summary>
    /// <param name="newHand">The relevant hand that will be grabbing the object</param>
    public virtual void SwapHands(Hand newHand)
    {
        hand.SwapHands();

        hand = newHand;
        transform.SetParent(hand.transform);
        //joint.connectedBody = hand.Rigidbody;
        hand.SwapHands();

        stamp = new ThrowStamp(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Store the holster data for grabbing and releasing
        if (other.CompareTag("Holster"))
            hoveredHolster = other.GetComponent<Holster>();
    }
    private void OnTriggerExit(Collider other)
    {
        // Disable the holster data
        if (hoveredHolster && other.transform == hoveredHolster.transform)
            hoveredHolster = null;
    }

    /// <summary>
    /// Velocity data for throwing the object on release
    /// </summary>
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
            {
                Debug.Log("new dir");
                velDir += prevStamp.velDir;
            }
        }

        public void print()
        {
            Debug.Log(pos + " : " + velDir + " * " + velForce + " = " + Velocity);
        }
    }
}
