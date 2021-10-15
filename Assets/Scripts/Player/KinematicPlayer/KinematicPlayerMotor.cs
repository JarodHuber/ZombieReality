using UnityEngine;

/// <summary>
/// Basic kinematic player motor demonstrating how to implement a motor for a KinematicBody
/// </summary>
public class KinematicPlayerMotor : MonoBehaviour, IKinematicMotor
{
    [Header("Body")]
    public KinematicBody body;
    
    [Header("Common Movement Settings")]
    public float moveSpeed = 8.0f;
    public float jumpHeight = 2.0f;
    
    [Header("Ground Movement")]
    public float maxGroundAngle = 75f;
    public LayerMask groundLayers;
    public float maxGroundAdhesionDistance = 0.1f;
    
    public bool Grounded { get; private set; }
    private bool wasGrounded;

    private bool jumpedThisFrame;

    // Input handling
    private Vector3 moveWish;
    private bool jumpWish;

    //
    // Motor API
    //

    public void MoveInput(Vector3 move)
    {
        moveWish = move;
    }

    public void JumpInput()
    {
        jumpWish = true;
    }
    
    //
    // Motor Utilities
    //
    
    public Vector3 ClipVelocity(Vector3 inputVelocity, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(inputVelocity, normal);
    }

    //
    // IKinematicMotor implementation
    //

    public Vector3 UpdateVelocity(Vector3 oldVelocity)
    {
        // If I need jumping I'll re-code this
        //if (jumpWish)
        //{
        //    jumpWish = false;

        //    if(wasGrounded)
        //    {
        //        jumpedThisFrame = true;

        //        velocity.y += Mathf.Sqrt(-2.0f * body.EffectiveGravity.y * jumpHeight);
        //    }
        //}

        return moveWish * moveSpeed;
    }

    public void OnMoveHit(ref Vector3 curPosition, ref Vector3 curVelocity, Collider other, Vector3 direction, float pen)
    {
        print(other.name);

        Vector3 clipped = ClipVelocity(curVelocity, direction);
        
        // floor
        if (groundLayers.Test(other.gameObject.layer) &&  // require ground layer
            direction.y > 0 &&                                      // direction check
            Vector3.Angle(direction, Vector3.up) < maxGroundAngle)  // angle check
        {
            // only change Y-position if bumping into the floor
            curPosition.y += direction.y * (pen);
            curVelocity.y = clipped.y;
            
            Grounded = true;
        }
        // other
        else
        {
            curPosition += direction * (pen);
            curVelocity = clipped;
        }
    }

    public void OnPreMove()
    {
        // reset frame data
        jumpedThisFrame = false;
        Grounded = false;
    }
    public void OnFinishMove(ref Vector3 curPosition, ref Vector3 curVelocity)
    {
        // Ground Adhesion

        // early exit if we're already grounded or jumping
        if (Grounded || jumpedThisFrame || !wasGrounded) return;
        
        var groundCandidates = body.Cast(curPosition, Vector3.down, maxGroundAdhesionDistance, groundLayers);
        Vector3 snapPosition = curPosition;
        foreach (var candidate in groundCandidates)
        {
            // ignore colliders that we start inside of - it's either us or something bad happened
            if(candidate.point == Vector3.zero) { continue; }

            // NOTE: This code assumes that the ground will always be below us
            snapPosition.y = candidate.point.y - body.FootOffset.y - body.contactOffset;
            
            // Snap to the ground - perform any necessary collision and sliding logic
            body.DeferredCollideAndSlide(ref snapPosition, ref curVelocity, candidate.collider);
            //Debug.Assert(snapPosition.y >= candidate.point.y, "Snapping put us underneath the ground?!");
            break;
        }

        curPosition = snapPosition;
    }

    public void OnPostMove()
    {
        // record grounded status for next frame
        wasGrounded = Grounded;
    }
    
    //
    // Unity Messages
    //

    private void Start()
    {
        body.motor = this;
        OnValidate();
    }
    
    private void OnValidate()
    {
        if(body == null ||body.BodyCollider == null) { return; }
    }
}
