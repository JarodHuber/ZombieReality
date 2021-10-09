using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class FireType : ScriptableObject
{
    [Tooltip("How many bullets the gun can fire per reload")]
    [SerializeField] int magazine = 10;

    [Space(10)]
    [Tooltip("the amount of time between each fire")]
    [SerializeField] float delayTimer = 0.0f;
    [Tooltip("Time required to reload")]
    [SerializeField] float reloadTimer = 1.0f;

    public virtual FireData InitializeFireData(ParticleSystem muzzleFlash, AudioSource source, TMP_Text ammoUI)
    {
        FireData toReturn = new FireData();

        toReturn.muzzle = muzzleFlash;
        toReturn.src = source;
        toReturn.ui = ammoUI;

        toReturn.delayTimer = new Timer(delayTimer);
        toReturn.magazine = new Counter(magazine);
        toReturn.reloadTimer = new Timer(reloadTimer);

        return toReturn;
    }

    public virtual void Fire(FireData data, Vector3 frontBarrel, Vector3 forward, BulletType bulletType, BulletType.BulletData bulletData)
    {
        data.magazine.Count();
        data.delayTimer.Reset();
        data.muzzle.Play();
        data.src.Play();
    }
    public virtual void UnFire(FireData data)
    {
        data.firing = false;
        //delayTimer.Reset(delayTimer.Delay);
    }
    /// <summary>
    /// reload the gun
    /// </summary>
    public virtual void Reload(FireData data)
    {
        data.reloading = true;
        data.reloadTimer.Count();

        data.magazine.InverseLerp(data.reloadTimer.PercentComplete);
    }

    /// <summary>
    /// end reload
    /// </summary>
    public virtual void StopReload(FireData data)
    {
        data.reloading = false;
        data.reloadTimer.Reset();
    }

    public virtual void GunUpdate(FireData data, Vector3 frontBarrel, Vector3 forward, BulletType bulletType, BulletType.BulletData bulletData)
    {
        if (data.firing)
        {
            if (data.CanFire)
            {
                Fire(data, frontBarrel, forward, bulletType, bulletData);
                data.ui.text = (int)data.magazine.AmountRemaining + "/" + data.magazine.Max;
            }
        }
        if (data.reloading)
        {
            if (data.CanReload)
            {
                Reload(data);
                data.ui.text = (int)data.magazine.AmountRemaining + "/" + data.magazine.Max;
            }
        }
        else
            data.delayTimer.Count();
    }
    public class FireData
    {
        public bool firing, reloading;
        public Counter magazine;

        public Timer delayTimer;
        public Timer reloadTimer;

        public ParticleSystem muzzle;
        public AudioSource src;
        public TMP_Text ui;

        public bool CanFire { get => delayTimer.IsComplete(false) && !reloading && !magazine.IsComplete(false); }
        public bool CanReload => magazine.AmountRemaining < magazine.Max;
    }
}
