using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grenade : GrabbableObj
{
    [Tooltip("The damage this grenade will do to each enemy")]
    [SerializeField] float damage = 50.0f;
    [Tooltip("The radius of the explosion")]
    [SerializeField] float radius = 5.0f;
    [Tooltip("This is multiplied against the damage to determine explosion force")]
    [SerializeField] float explosionMod = 10.0f;
    [Tooltip("How long before the grenade explodes")]
    [SerializeField] Timer fuse = new Timer(3.0f);
    [Tooltip("what layers the grenade can hit")]
    [SerializeField] LayerMask grenadeMask = new LayerMask();

    [Space(10)]
    [Tooltip("Holder for the basic grenade object")]
    [SerializeField] GameObject shell = null;
    [Tooltip("The particle effect for the explosion")]
    [SerializeField] ParticleSystem explosionEffect = null;
    [Tooltip("The Sound effect for the grenade")]
    [SerializeField] AudioSource explosionSound = null;

    EnemyManager manager = null;

    bool primed = false;
    bool armed = false;
    bool blown = false;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>();
    }

    private void Update()
    {
        // If not armed or fuse has not finished
        if (!armed || !fuse.Check(false))
            return;

        // If the Grenade has not yet exploded
        if (!blown)
        {
            // If the grenade is still being held drop it
            if (IsGrabbed)
                hand.UnGrab();

            rb.isKinematic = true;

            // Start visual effect
            shell.SetActive(false);
            explosionEffect.Play();
            explosionSound.Play();

            foreach (Collider col in Physics.OverlapSphere(transform.position, radius, grenadeMask, QueryTriggerInteraction.Ignore))
            {
                switch (col.gameObject.tag)
                {
                    case "Enemy":
                        manager.DamageEnemy(col.transform, damage);
                        break;
                    case "GrabbableObject":
                        col.attachedRigidbody.AddExplosionForce(damage * explosionMod, transform.position, radius);
                        break;
                    case "Player":
                        col.transform.root.GetComponent<Player>().NearKill();
                        break;
                }
            }

            blown = true;
            return;
        }

        // If the explosion has finished
        if (explosionEffect.isStopped)
            DestroySelf();
    }

    public override void Grab(Hand handGrabbing)
    {
        // Do basic object grabbing
        base.Grab(handGrabbing);

        // Add relevent methods to the InputActionMap
        hand.input["Fire"].performed += PullPin;
        hand.input["Fire"].canceled += ReleaseLever;
    }

    public override void Release()
    {
        // Remove relevent methods to the InputActionMap
        hand.input["Fire"].performed -= PullPin;
        hand.input["Fire"].canceled -= ReleaseLever;

        // Do basic object releasing
        base.Release();
    }
    public override void SwapHands(Hand newHand)
    {
        // Remove relevent methods to the InputActionMap
        hand.input["Fire"].performed -= PullPin;
        hand.input["Fire"].canceled -= ReleaseLever;
        
        //Do basic object swapping
        base.SwapHands(newHand);

        // Add relevent methods to the InputActionMap
        hand.input["Fire"].performed += PullPin;
        hand.input["Fire"].canceled += ReleaseLever;
    }

    void PullPin(InputAction.CallbackContext context)
    {
        primed = true;
    }
    void ReleaseLever(InputAction.CallbackContext context)
    {
        if(primed)
            armed = true;
    }

    private void DestroySelf()
    {
        Destroy(gameObject); // TODO: replace this with object cycler
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
