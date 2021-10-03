﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Recode this to minimize duplicate SOs
[CreateAssetMenu(menuName = "Gun/Bullet/SingleBullet")]
public class SingleBullet : BulletType
{
    [Header("Bullet Effect")]
    [Tooltip("How long the bullet line appears in the air")]
    [SerializeField] Material bulletMaterial = null;
    [SerializeField] float bulletWidth = 0.01f;

    public override BulletData InitializeBulletData(GameObject gun, Vector3 frontBarrel)
    {
        SingleBulletData toReturn = new SingleBulletData();

        if (gun.GetComponent<LineRenderer>() == null)
            toReturn.bulletLine = gun.AddComponent<LineRenderer>();

        Vector3[] initLaserPositions = new Vector3[2] { frontBarrel, frontBarrel };
        toReturn.bulletLine.SetPositions(initLaserPositions);
        toReturn.bulletLine.material = bulletMaterial;
        toReturn.bulletLine.startWidth = bulletWidth;
        toReturn.bulletLine.endWidth = bulletWidth;
        toReturn.bulletLine.enabled = false;

        toReturn.bulletInactive = true;

        return toReturn;
    }

    public override void SetVelocity(BulletData dataHold, Vector3 frontBarrel, Vector3 forward)
    {
        if (!(dataHold is SingleBulletData data))
            return;

        data.direction = frontBarrel + (forward * range);

        if (spread > 0)
            data.direction += Random.insideUnitSphere * spread;

        CastEvent(dataHold, frontBarrel);
    }

    public override void CastEvent(BulletData dataHold, Vector3 startPoint)
    {
        if (!(dataHold is SingleBulletData data))
            return;

        data.bulletInactive = false;
        Vector3 endPoint;
        RaycastHit hit;

        if (Physics.Raycast(startPoint, (data.direction - startPoint).normalized, out hit, range, 
            data.bulletMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;
            HandleCollision(hit);
        }
        else
            endPoint = data.direction;

        data.bulletLine.enabled = true;
        data.bulletLine.SetPosition(0, startPoint);
        data.bulletLine.SetPosition(1, endPoint);
    }

    public override void BulletUpdate(BulletData dataHold, Vector3 frontBarrel)
    {
        if (!(dataHold is SingleBulletData data))
            return;

        if (data.bulletInactive) 
            return;

        if (data.bulletLifespan.Check())
        {
            data.bulletLine.enabled = false;

            data.bulletInactive = true;
            return;
        }

        data.bulletLine.SetPosition(0, frontBarrel);
    }

    public class SingleBulletData : BulletData
    {
        public LineRenderer bulletLine = null;
        public Vector3 direction = new Vector3();
    }
}

