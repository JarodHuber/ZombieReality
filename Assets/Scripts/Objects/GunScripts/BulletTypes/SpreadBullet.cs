using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Bullet/SpreadBullet")]
public class SpreadBullet : BulletType
{
    [Header("Spread Variables")]
    [Tooltip("How many bullets the gun shoots per fire")]
    [SerializeField] int bulletCount = 10;

    [Header("Bullet Effect")]
    [Tooltip("How long the bullet line appears in the air")]
    [SerializeField] Timer fireHold = new Timer(.05f);
    [SerializeField] Material bulletMaterial = null;
    [SerializeField] float bulletWidth = 0.01f;

    bool lineLock = false;
    Vector3[] directions = new Vector3[0];
    LineRenderer[] bulletLines = new LineRenderer[0];

    public override void Initialize(GameObject gun, Vector3 frontBarrel, EnemyManager manager)
    {
        base.Initialize(gun, frontBarrel, manager);

        directions = new Vector3[bulletCount];

        bulletLines = new LineRenderer[bulletCount];


        for (int x = 0; x < bulletCount; x++)
        {
            if (gun.GetComponent<LineRenderer>() == null)
                bulletLines[x] = gun.AddComponent<LineRenderer>();
            else
            {
                GameObject renderChild = new GameObject("lineRenderer");
                renderChild.transform.position = frontBarrel;
                renderChild.transform.SetParent(gun.transform);

                bulletLines[x] = renderChild.AddComponent<LineRenderer>();
            }

            Vector3[] initLaserPositions = new Vector3[2] { frontBarrel, frontBarrel };
            bulletLines[x].SetPositions(initLaserPositions);
            bulletLines[x].material = bulletMaterial;
            bulletLines[x].startWidth = bulletWidth;
            bulletLines[x].endWidth = bulletWidth;
            bulletLines[x].enabled = false;
        }
    }

    public override void SetVelocity(Vector3 frontBarrel, Vector3 forward)
    {
        for (int x = 0; x < bulletCount; ++x)
        {
            directions[x] = frontBarrel + ((forward * range) + (Random.insideUnitSphere * spread));
        }
        CastEvent(frontBarrel);
    }

    public override void CastEvent(Vector3 startPoint)
    {
        lineLock = false;
        Vector3 endPoint;
        RaycastHit hit;

        for (int x = 0; x < bulletCount; x++)
        {
            if (Physics.Raycast(startPoint, (directions[x] - startPoint).normalized, out hit, range))
            {
                endPoint = hit.point;
                HandleCollision(hit);
            }
            else
                endPoint = directions[x];

            bulletLines[x].enabled = true;
            bulletLines[x].SetPosition(0, startPoint);
            bulletLines[x].SetPosition(1, endPoint);
        }
    }

    public override void BulletUpdate(Vector3 frontBarrel)
    {
        if (lineLock)
            return;

        if (fireHold.Check())
        {
            for (int x = 0; x < bulletCount; ++x)
                bulletLines[x].enabled = false;

            lineLock = true;
            return;
        }

        for (int x = 0; x < bulletCount; ++x)
            bulletLines[x].SetPosition(0, frontBarrel);
    }
}
