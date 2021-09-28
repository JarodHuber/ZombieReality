using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Recode this to minimize duplicate SOs
[CreateAssetMenu(menuName = "Gun/Bullet/SingleBullet")]
public class SingleBullet : BulletType
{
    [Header("Bullet Effect")]
    [Tooltip("How long the bullet line appears in the air")]
    [SerializeField] Timer fireHold = new Timer(.05f);
    [SerializeField] Material bulletMaterial = null;
    [SerializeField] float bulletWidth = 0.01f;
    [SerializeField] LayerMask bulletMask = new LayerMask();

    protected bool lineLock = false;
    LineRenderer bulletLine = null;

    protected Vector3 direction = new Vector3();

    public override void Initialize(GameObject gun, Vector3 frontBarrel, EnemyManager manager)
    {
        base.Initialize(gun, frontBarrel, manager);

        if (gun.GetComponent<LineRenderer>() == null)
            bulletLine = gun.AddComponent<LineRenderer>();

        Vector3[] initLaserPositions = new Vector3[2] { frontBarrel, frontBarrel };
        bulletLine.SetPositions(initLaserPositions);
        bulletLine.material = bulletMaterial;
        bulletLine.startWidth = bulletWidth;
        bulletLine.endWidth = bulletWidth;
        bulletLine.enabled = false;

        lineLock = true;
    }

    public override void SetVelocity(Vector3 frontBarrel, Vector3 forward)
    {
        direction = frontBarrel + (forward * range);

        if (spread > 0)
            direction += Random.insideUnitSphere * spread;

        CastEvent(frontBarrel);
    }

    public override void CastEvent(Vector3 startPoint)
    {
        lineLock = false;
        Vector3 endPoint;
        RaycastHit hit;

        if (Physics.Raycast(startPoint, (direction - startPoint).normalized, out hit, range, bulletMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;
            HandleCollision(hit);
        }
        else
            endPoint = direction;

        bulletLine.enabled = true;
        bulletLine.SetPosition(0, startPoint);
        bulletLine.SetPosition(1, endPoint);
    }

    public override void BulletUpdate(Vector3 frontBarrel)
    {
        if (lineLock) 
            return;

        if (fireHold.Check())
        {
            bulletLine.enabled = false;

            lineLock = true;
            return;
        }

        bulletLine.SetPosition(0, frontBarrel);
    }
}
