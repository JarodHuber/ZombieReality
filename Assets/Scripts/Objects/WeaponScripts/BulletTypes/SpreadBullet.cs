using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Bullet/SpreadBullet")]
public class SpreadBullet : BulletType
{
    [Header("Spread Variables")]
    [Tooltip("How many bullets the gun shoots per fire")]
    [SerializeField] int bulletCount = 10;

    [SerializeField] Material bulletMaterial = null;
    [SerializeField] float bulletWidth = 0.01f;


    public override BulletData InitializeBulletData(GameObject gun, Vector3 frontBarrel)
    {
        SpreadBulletData toReturn = new SpreadBulletData();

        Configure(toReturn, gun, frontBarrel);

        return toReturn;
    }

    public override void Configure(BulletData data, GameObject gun, Vector3 frontBarrel)
    {
        base.Configure(data, gun, frontBarrel);

        SpreadBulletData spreadData = (SpreadBulletData)data;

        spreadData.directions = new Vector3[bulletCount];
        spreadData.bulletLines = new LineRenderer[bulletCount];


        for (int x = 0; x < bulletCount; x++)
        {
            if (gun.GetComponent<LineRenderer>() == null)
                spreadData.bulletLines[x] = gun.AddComponent<LineRenderer>();
            else
            {
                GameObject renderChild = new GameObject("lineRenderer");
                renderChild.transform.position = frontBarrel;
                renderChild.transform.SetParent(gun.transform);

                spreadData.bulletLines[x] = renderChild.AddComponent<LineRenderer>();
            }

            Vector3[] initLaserPositions = new Vector3[2] { frontBarrel, frontBarrel };
            spreadData.bulletLines[x].SetPositions(initLaserPositions);
            spreadData.bulletLines[x].material = bulletMaterial;
            spreadData.bulletLines[x].startWidth = bulletWidth;
            spreadData.bulletLines[x].endWidth = bulletWidth;
            spreadData.bulletLines[x].enabled = false;
        }

        spreadData.bulletInactive = true;
    }

    public override void SetVelocity(BulletData dataHold, Vector3 frontBarrel, Vector3 forward)
    {
        if (!(dataHold is SpreadBulletData data))
            return;

        for (int x = 0; x < bulletCount; ++x)
        {
            data.directions[x] = frontBarrel + ((forward * range) + (Random.insideUnitSphere * spread));
        }
        CastEvent(dataHold, frontBarrel);
    }

    public override void CastEvent(BulletData dataHold, Vector3 startPoint)
    {
        if (!(dataHold is SpreadBulletData data))
            return;

        data.bulletInactive = false;
        Vector3 endPoint;
        RaycastHit hit;

        for (int x = 0; x < bulletCount; x++)
        {
            if (Physics.Raycast(startPoint, (data.directions[x] - startPoint).normalized, out hit, range, 
                data.bulletMask, QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;
                HandleCollision(hit);
            }
            else
                endPoint = data.directions[x];

            data.bulletLines[x].enabled = true;
            data.bulletLines[x].SetPosition(0, startPoint);
            data.bulletLines[x].SetPosition(1, endPoint);
        }
    }

    public override void BulletUpdate(BulletData dataHold, Vector3 frontBarrel)
    {
        if (!(dataHold is SpreadBulletData data))
            return;

        if (data.bulletInactive)
            return;

        if (data.bulletLifespan.Check())
        {
            for (int x = 0; x < bulletCount; ++x)
                data.bulletLines[x].enabled = false;

            data.bulletInactive = true;
            return;
        }

        for (int x = 0; x < bulletCount; ++x)
            data.bulletLines[x].SetPosition(0, frontBarrel);
    }

    public class SpreadBulletData : BulletData
    {
        public Vector3[] directions = new Vector3[0];
        public LineRenderer[] bulletLines = new LineRenderer[0];
    }
}
