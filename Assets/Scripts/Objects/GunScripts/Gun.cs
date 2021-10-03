﻿using System.Collections;
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

    FireType.FireData fireData;
    BulletType.BulletData bulletData;
    #endregion

    protected override void Initialize()
    {
        base.Initialize();

        muzzleFlash.transform.localScale *= fireEventScale;

        bulletType.Initialize(GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>());

        fireData = fireType.InitializeFireData(muzzleFlash, GetComponent<AudioSource>(), ammoUI);
        bulletData = bulletType.InitializeBulletData(gameObject, frontBarrel.transform.position);
    }

    private void Update()
    {
        if (hand && hand.isPaused)
            return;

        fireType.GunUpdate(fireData, frontBarrel.transform.position, Forward, bulletType, bulletData);
        bulletType.BulletUpdate(bulletData, frontBarrel.transform.position);
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
        fireData.firing = false;
        fireData.reloading = false;

        hand.input["Fire"].performed -= AttemptFire;
        hand.input["Fire"].canceled -= AttemptUnFire;

        hand.input["Reload"].performed -= AttemptReload;
        hand.input["Reload"].canceled -= AttemptUnReload;

        base.Release();
    }
    public override void SwapHands(Hand newHand)
    {
        fireData.firing = false;
        fireData.reloading = false;

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
        if (fireData.CanFire)
            fireData.firing = true;
    }
    void AttemptUnFire(InputAction.CallbackContext context)
    {
        if (fireData.firing)
            fireType.UnFire(fireData);
    }

    void AttemptReload(InputAction.CallbackContext context)
    {
        if (fireData.CanReload)
        {
            fireData.reloading = true;
            fireType.UnFire(fireData);
        }
    }
    void AttemptUnReload(InputAction.CallbackContext context)
    {
        if (fireData.reloading)
            fireType.StopReload(fireData);
    }
}
