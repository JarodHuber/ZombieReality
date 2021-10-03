using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Fire/AutoFire")]
public class AutoFire : FireType
{
    public override void Fire(FireData data, Vector3 frontBarrel, Vector3 forward, BulletType bulletType, BulletType.BulletData bulletData)
    {
        bulletType.SetVelocity(bulletData, frontBarrel, forward);

        base.Fire(data, frontBarrel, forward, bulletType, bulletData);
    }
}
