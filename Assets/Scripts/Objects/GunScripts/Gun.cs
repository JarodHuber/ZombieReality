using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : SnappedGrabbableObj
{
    #region Variables
    [Header("Gun Variables")]
    [Tooltip("Set to an empty GameObject at the tip of the gun")]
    [SerializeField] GameObject frontBarrel = null;
    [Tooltip("Set to an empty GameObject at the other end of the gun")]
    [SerializeField] GameObject backBarrel = null;

    [Space(10)]
    [Tooltip("Prefab of the muzzle-flash particle system")]
    [SerializeField] ParticleSystem muzzleFlash = null;

    [Space(10)]
    [Tooltip("The force of the vibration in the controller")]
    [SerializeField] float fireEventScale = .6f;

    [Space(10)]
    [SerializeField] TMP_Text ammoUI = null;

    [Space(10)]
    [SerializeField] BulletType bulletType = null;
    [SerializeField] FireType fireType = null;

    Vector3 Forward { get => (frontBarrel.transform.position - backBarrel.transform.position).normalized; }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();

        muzzleFlash.transform.localScale *= fireEventScale;

        fireType.Initialize(muzzleFlash, GetComponent<AudioSource>(), ammoUI);
        bulletType.Initialize(gameObject, frontBarrel.transform.position, GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>());
    }

    private void Update()
    {
        if (hand && hand.isPaused)
            return;

        fireType.GunUpdate(frontBarrel.transform.position, Forward, bulletType);
        bulletType.BulletUpdate(frontBarrel.transform.position);
    }

    public override void Grab(Hand handGrabbing)
    {
        base.Grab(handGrabbing);

        hand.input["Fire"].performed += AttemptFire;
        hand.input["Fire"].canceled += AttemptUnFire;

        hand.input["Reload"].performed += AttemptReload;
        hand.input["Reload"].canceled += AttemptUnReload;
    }
    public override void Release()
    {
        fireType.firing = false;
        fireType.reloading = false;

        hand.input["Fire"].performed -= AttemptFire;
        hand.input["Fire"].canceled -= AttemptUnFire;

        hand.input["Reload"].performed -= AttemptReload;
        hand.input["Reload"].canceled -= AttemptUnReload;

        base.Release();
    }
    public override void SwapHands(Hand newHand)
    {
        fireType.firing = false;
        fireType.reloading = false;

        hand.input["Fire"].performed -= AttemptFire;
        hand.input["Fire"].canceled -= AttemptUnFire;

        hand.input["Reload"].performed -= AttemptReload;
        hand.input["Reload"].canceled -= AttemptUnReload;

        base.SwapHands(newHand);

        hand.input["Fire"].performed += AttemptFire;
        hand.input["Fire"].canceled += AttemptUnFire;

        hand.input["Reload"].performed += AttemptReload;
        hand.input["Reload"].canceled += AttemptUnReload;
    }

    void AttemptFire(InputAction.CallbackContext context)
    {
        if (fireType.CanFire)
            fireType.firing = true;
    }
    void AttemptUnFire(InputAction.CallbackContext context)
    {
        if (fireType.firing)
            fireType.UnFire();
    }

    void AttemptReload(InputAction.CallbackContext context)
    {
        if (fireType.CanReload)
        {
            fireType.reloading = true;
            fireType.UnFire();
        }
    }
    void AttemptUnReload(InputAction.CallbackContext context)
    {
        if (fireType.reloading)
            fireType.StopReload();
    }
}
