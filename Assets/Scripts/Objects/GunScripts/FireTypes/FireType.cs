using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class FireType : ScriptableObject
{
    [HideInInspector]
    public bool firing = false, reloading = false;
    [Tooltip("How many bullets the gun can fire per reload")]
    public Counter magazine = new Counter(10);

    [Space(10)]
    [Tooltip("the amount of time between each fire")]
    [SerializeField] protected Timer delayTimer = new Timer(0);
    [Tooltip("Time required to reload")]
    [SerializeField] protected Timer reloadTimer = new Timer(1f);

    public bool CanFire { get => delayTimer.IsComplete(false) && !reloading && !magazine.IsComplete(false); }
    public bool CanReload { get => magazine.AmountRemaining < magazine.Max; }

    protected ParticleSystem muzzle = null;
    protected AudioSource src = null;
    protected TMP_Text ui = null;

    public virtual void Initialize(ParticleSystem muzzleFlash, AudioSource source, TMP_Text ammoUI)
    {
        muzzle = muzzleFlash;
        src = source;
        ui = ammoUI;
        delayTimer.Reset(delayTimer.Delay);
        magazine.Reset();
    }

    public virtual void Fire(Vector3 frontBarrel, Vector3 forward, BulletType bulletType)
    {
        magazine.Count();
        delayTimer.Reset();
        muzzle.Play();
        src.Play();
    }
    public virtual void UnFire()
    {
        firing = false;
        //delayTimer.Reset(delayTimer.Delay);
    }
    /// <summary>
    /// reload the gun
    /// </summary>
    public virtual void Reload()
    {
        reloading = true;
        reloadTimer.Count();

        magazine.InverseLerp(reloadTimer.PercentComplete);
    }

    /// <summary>
    /// end reload
    /// </summary>
    public virtual void StopReload()
    {
        reloading = false;
        reloadTimer.Reset();
    }

    public virtual void GunUpdate(Vector3 frontBarrel, Vector3 forward, BulletType bulletType)
    {
        if (firing)
        {
            if (CanFire)
            {
                Fire(frontBarrel, forward, bulletType);
                ui.text = (int)magazine.AmountRemaining + "/" + magazine.Max;
            }
        }
        if (reloading)
        {
            if (CanReload)
            {
                Reload();
                ui.text = (int)magazine.AmountRemaining + "/" + magazine.Max;
            }
        }
        else
            delayTimer.Count();
    }
}
