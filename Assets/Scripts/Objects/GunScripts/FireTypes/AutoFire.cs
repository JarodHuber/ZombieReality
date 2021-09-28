using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Fire/AutoFire")]
public class AutoFire : FireType
{
    public override void Fire(Vector3 frontBarrel, Vector3 forward, BulletType bulletType)
    {
        bulletType.SetVelocity(frontBarrel, forward);

        base.Fire(frontBarrel, forward, bulletType);
    }
}
