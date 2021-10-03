using System.Linq;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    Transform trnfrm = null;
    AI mov = null;
    AudioSource src = null;
    [SerializeField] Timer atkT = new Timer(1), sndT = new Timer(1);
    [SerializeField] Counter hp = new Counter(30);
    [SerializeField] int dmg = 1;
    [SerializeField] float spd = 7, rch = 3.0f;

    [HideInInspector]
    public EnemySounds enemySounds = null;

    public bool IsAlive { get => !hp.IsComplete(false); }
    public Transform Transform { get => trnfrm; }
    public AI Agent { get => mov; }
    public float Speed { get => spd; }

    public Enemy(Timer attackTimer, Timer soundTimer, Counter health, 
        int damage, float speed, float reach)
    {
        trnfrm = null;
        mov = null;
        src = null;
        atkT = attackTimer;
        sndT = soundTimer;
        hp = health;
        dmg = damage;
        spd = speed;
        rch = reach;
    }

    public Enemy(Transform transform, AI agent, AudioSource audioSource, Enemy enemyFab)
    {
        trnfrm = transform;
        mov = agent;
        src = audioSource;
        atkT = new Timer(enemyFab.atkT.Max);
        sndT = new Timer(enemyFab.sndT.Max);
        hp = new Counter(enemyFab.hp.Max);
        dmg = enemyFab.dmg;
        spd = enemyFab.spd;
        rch = enemyFab.rch;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("ow");
        hp.Count(damage);
    }

    public bool InReach(float dist, out float distToBeAt)
    {
        if(dist < rch)
        {
            distToBeAt = Mathf.Max(1.25f, rch / 2.0f); // TODO: replace 1.25f with a const variable
            return true;
        }

        if (atkT.Cur > 0.0f)
            atkT.Reset();

        distToBeAt = 0.0f;
        return false;
    }

    public void AttemptAttack(Player player)
    {
        if (atkT.Check())
        {
            PlaySound(true);
            player.TakeDamage(dmg);
        }
    }

    public void PlaySound(bool attackSound = false)
    {
        if (enemySounds.ClipCount(attackSound) == 0)
            return;

        if (!attackSound)
        {
            if (src.isPlaying)
            {
                if (sndT.Cur > 0.0f)
                    sndT.Reset();

                return;
            }

            if (!sndT.Check())
                return;
        }

        src.PlayOneShot(enemySounds.GetSound(attackSound));
    }
}
