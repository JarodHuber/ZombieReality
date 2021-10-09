using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class AI : MonoBehaviour
{
    enum State { Stop, NavMesh }

    [Tooltip("This is the current type of movement the AI is using")]
    [SerializeField] State currentState = State.Stop;

    [Space(10)]
    [Tooltip("Target of the AI")]
    public Vector3 target = new Vector3();
    [Tooltip("How fast the AI moves")]
    public float speed = 5.0f;
    [Tooltip("Turning speed of the AI")]
    [SerializeField] float turningSpeed = 30.0f;

    [Space(10)]
    [Tooltip("Physical radius of the AI")]
    [SerializeField] float radius = 0.5f;
    [Tooltip("Height of the AI")]
    [SerializeField] float height = 2.0f;
    [Tooltip("How far the AI should stop before the target")]
    [SerializeField] float stoppingDist = 1.0f;
    [Tooltip("How much time to spend in-between path recalculation")]
    [SerializeField] Timer calcPathCallBackRate = new Timer(3.0f);
    [SerializeField] float cornerDistBuffer = 1.0f;
    [SerializeField] bool ignoreDistToTarget = true;

    [Space(10)]
    [Tooltip("This is the maximum angle before the AI moves forward")]
    [SerializeField] float maxMoveAngle = 90.0f;
    [Tooltip("This is the maximum angle before the AI moves at max speed")]
    [SerializeField] float minMoveAngle = 20.0f;

    [Space(10)]
    [Tooltip("This is how much pull the AI gets to travel towards it's node")]
    public float seekStr = 10.0f;

    [Space(10)]
    [SerializeField] Rigidbody rb = null;

    //[Space(10)]
    //[Tooltip("This is the distance the AI needs to be from obstacles to avoid them")]
    //public float fleeDist = 10.0f;

    // The path the AI is taking towards the target
    Vector3[] path = new Vector3[0];

    State stateBeforePause = State.NavMesh;

    [SerializeField] bool active = true, stop = false;
    bool isGrounded = false;


    int cornerIndex = 1;

    // Vector3 the AI is currently moving towards from the path
    Vector3 CurrentCorner { get => (path.Length != 0) ? path[(cornerIndex >= path.Length) ? path.Length - 1 : cornerIndex] : transform.position; }
    // Check to avoid errors
    bool PathEmpty { get => path.Length < 2; }
    public Vector3 NavPos
    {
        get
        {
            NavMesh.SamplePosition(transform.position, out NavMeshHit hit, height, NavMesh.AllAreas);
            return hit.position;
        }
    }

    public bool StopAI { get => stop; set => stop = value; }
    bool PauseAI { get => !active || stop; }

    private void Start()
    {
        CalculateNavPath();
    }

    private void Update()
    {
        // Update variables to set the AI into a paused state
        if (PauseAI && currentState != State.Stop)
        {
            stateBeforePause = currentState;
            rb.velocity = new Vector3();
            currentState = State.Stop;
        }

        GroundCheck();

        switch (currentState)
        {
            case State.Stop:    // AI not moving
                if (!PauseAI)
                {
                    currentState = stateBeforePause;

                    return;
                }
                else if (!ignoreDistToTarget && !active)
                    active = Vector3.Distance(target, NavPos) > stoppingDist;

                return;
            case State.NavMesh: // AI traveling along nav mesh
                if (calcPathCallBackRate.Check())
                    CalculateNavPath();
                return;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Stop:
                Stand();
                break;
            case State.NavMesh:
                Move();
                break;
        }
    }

    private void Stand()
    {
        Vector3 velocity = new Vector3();
        velocity.y = rb.velocity.y;

        // Make the AI fall
        if (!isGrounded)
            velocity.y += Physics.gravity.y * Time.deltaTime;

        // Assign velocity
        rb.velocity = velocity; // TODO: Replace this with AddForce
    }
    private void Move()
    {
        // If the AI is close enough to targetNode to move on to the next
        if (!ignoreDistToTarget && Vector3.Distance(target, NavPos) < stoppingDist)
        {
            active = false;
            return;
        }

        if (Vector3.Distance(NavPos, CurrentCorner) < cornerDistBuffer)
        {
            if (++cornerIndex >= path.Length)
                cornerIndex = path.Length - 1;
        }

        // Determine the angle between the AI's forward direction and create a nonsigned version
        float angle = Vector3.SignedAngle(transform.forward, GetDesiredDirection(), transform.up);
        float uAngle = Mathf.Abs(angle);

        // Reduce speed the larger the angle is
        float angleSpeedMod = Mathf.InverseLerp(maxMoveAngle, minMoveAngle, uAngle);

        // How much the AI will rotate towrds desiredDir
        float rotation = turningSpeed * ((angle != 0) ? (angle > 0) ? 1 : -1 : 0) * Time.deltaTime;

        //Rotate the AI
        transform.Rotate(transform.up, (Mathf.Abs(rotation) > uAngle) ? angle : rotation);

        // Determine the velocity the AI should go
        Vector3 velocity = transform.forward * speed * angleSpeedMod;
        velocity.y = rb.velocity.y;

        // Make the AI fall
        if (!isGrounded)
            velocity.y += Physics.gravity.y * Time.deltaTime;

        // Assign velocity
        rb.velocity = velocity; // TODO: Replace this with AddForce
    }

    void AddForce(Vector3 force)
    {
        if(rb.velocity.sqrMagnitude < speed * speed)
        {
            Vector3 assumedVelocity = rb.velocity + (force * Time.fixedDeltaTime);
            if (assumedVelocity.sqrMagnitude > speed * speed) 
            {
                force = Vector3.ClampMagnitude(force, force.magnitude - (assumedVelocity.magnitude - speed));
            }

            rb.AddForce(force * Time.deltaTime);
        }
    }

    /// <summary>
    /// Determines the direction the AI wants to go
    /// </summary>
    /// <returns>Returns a Vector3 direction for the Ai to travel</returns>
    Vector3 GetDesiredDirection()
    {
        // Retunr if there is nothing to try and travel towards
        //if (PathEmpty)
        //    return Vector3.zero;

        // Seek steer towards target
        Vector3 toReturn = (CurrentCorner - rb.position).normalized * seekStr;

        #region flee code
        // Flee steer away from nearby objectsToFleeFrom
        //Vector3 fleeForce = Vector3.zero;
        //for (int x = 0; x < objectsToFleeFrom.Length; ++x)
        //{
        //    if (objectsToFleeFrom[x] == racer.transform)
        //        continue;

        //    float dist = Vector3.Distance(objectsToFleeFrom[x].position, racer.transform.position) / racer.fleeDist;

        //    if (dist > 1)
        //        continue;

        //    fleeForce += (racer.transform.position - objectsToFleeFrom[x].position).normalized * (1 - dist);
        //}
        //toReturn += fleeForce.normalized * racer.fleeStr;
        #endregion

        toReturn.y = 0;

        return toReturn.normalized;
    }
    /// <summary>
    /// Calculate navigation path utilizing nav mesh
    /// </summary>
    /// <returns>returns an array of Vector3s that the</returns>
    void CalculateNavPath()
    {
        cornerIndex = 1;
        NavMeshPath navPath = new NavMeshPath();

        NavMesh.CalculatePath(rb.position, target, NavMesh.AllAreas, navPath);

        //for (int x = 0; x < navPath.corners.Length; ++x)
        //    navPath.corners[x].y += height / 2;

        path = navPath.corners;
    }

    /// <summary>
    /// Check to see if the AI is touching the ground
    /// </summary>
    void GroundCheck()
    {
        float height = (((rb.position.y - transform.position.y) / transform.localScale.y) / 2) - radius;

        isGrounded = Physics.SphereCast(rb.position, radius - 0.05f, Vector3.down, out RaycastHit hit, height + 0.06f);
    }

    /// <summary>
    /// Update the target then recalculate the new path
    /// </summary>
    /// <param name="destination">the new destination for the AI to travel towards</param>
    public void SetDestination(Vector3 destination)
    {
        target = destination;
        calcPathCallBackRate.Reset();
        CalculateNavPath();
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        for(int x = cornerIndex; x < path.Length; ++x)
        {
            Gizmos.DrawLine(pos, path[x]);
            pos = path[x];
        }
    }
}
