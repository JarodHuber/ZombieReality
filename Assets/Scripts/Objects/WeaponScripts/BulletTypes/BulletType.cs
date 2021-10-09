using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletType : ScriptableObject
{
    [Header("Bullet Variables")]
    [Tooltip("Range of the bullet")]
    [SerializeField] protected float range = 50f;
    [Tooltip("Amount of damage the bullet deals")]
    [SerializeField] protected float damage = 50;
    [Tooltip("How much the bullet can deviate from center")]
    [SerializeField] protected float spread = 0.3f;
    [SerializeField] float bulletLifespan = 0.05f;
    [SerializeField] LayerMask bulletMask = new LayerMask();

    EnemyManager enemyManager = null;

    public void Initialize(EnemyManager manager)
    {
        enemyManager = manager;
    }

    public abstract BulletData InitializeBulletData(GameObject gun, Vector3 frontBarrel);

    public virtual void Configure(BulletData data, GameObject gun, Vector3 frontBarrel)
    {
        data.bulletLifespan = new Timer(bulletLifespan);
        data.bulletMask = bulletMask;
    }

    /// <summary>
    /// Sets the direction for the bullet to travel
    /// </summary>
    public abstract void SetVelocity(BulletData data, Vector3 frontBarrel, Vector3 forward);

    /// <summary>
    /// shoot the raycast
    /// </summary>
    public abstract void CastEvent(BulletData data, Vector3 startPoint);

    public abstract void BulletUpdate(BulletData data, Vector3 frontBarrel);

    /// <summary>
    /// what to do if the bullet hits
    /// </summary>
    /// <param name="collision">Bullet hit data</param>
    public void HandleCollision(RaycastHit collision)
    {
        //Debug.LogWarning(collision.collider.gameObject.name);
        if (collision.transform.CompareTag("Enemy"))
        {
            enemyManager.DamageEnemy(collision.transform, damage);
        }
    }

    public class BulletData
    {
        public Timer bulletLifespan = new Timer(.05f);
        public bool bulletInactive = false;

        public LayerMask bulletMask = new LayerMask();
    }
}

