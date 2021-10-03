using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Fire/SemiFire")]
public class SemiFire : FireType
{
    public override void Fire(FireData data, Vector3 frontBarrel, Vector3 forward, BulletType bulletType, BulletType.BulletData bulletData)
    {
        bulletType.SetVelocity(bulletData, frontBarrel, forward);

        base.Fire(data, frontBarrel, forward, bulletType, bulletData);

        UnFire(data);
    }
}
