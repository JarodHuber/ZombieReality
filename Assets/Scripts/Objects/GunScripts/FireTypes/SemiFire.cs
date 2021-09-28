using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Fire/SemiFire")]
public class SemiFire : FireType
{
    public override void Fire(Vector3 frontBarrel, Vector3 forward, BulletType bulletType)
    {
        bulletType.SetVelocity(frontBarrel, forward);

        base.Fire(frontBarrel, forward, bulletType);

        UnFire();
    }
}
