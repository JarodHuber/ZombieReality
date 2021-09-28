using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Fire/SemiFire")]
public class SemiFire : FireType
{
    public override void Fire(Vector3 frontBarrel, Vector3 forward, BulletType bulletType)
    {
        magazine.Count();
        delayTimer.Reset();

        bulletType.SetVelocity(frontBarrel, forward);

        muzzle.Play();
        src.PlayOneShot(clip);

        UnFire();
    }
}
