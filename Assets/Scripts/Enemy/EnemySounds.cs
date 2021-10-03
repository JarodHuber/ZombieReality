using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemySounds
{
    [SerializeField] AudioClip[] generalClips = new AudioClip[0];
    [SerializeField] AudioClip[] attackClips = new AudioClip[0];
    
    float[] generalClipLock = new float[0];
    float[] attackClipLock = new float[0];

    public AudioClip Clip(int index)
    {
        if (index >= generalClips.Length)
            return null;

        return generalClips[index];
    }
    public int ClipCount(bool attackSound)
    {
        return (attackSound) ? attackClips.Length : generalClips.Length;
    }

    public void Initialize()
    {
        generalClipLock = new float[generalClips.Length];

        attackClipLock = new float[attackClips.Length];
    }
    public void Initialize(AudioClip[] enemySounds, AudioClip[] attackSounds)
    {
        generalClips = enemySounds;
        generalClipLock = new float[enemySounds.Length];

        attackClips = attackSounds;
        attackClipLock = new float[attackSounds.Length];
    }

    public AudioClip GetSound(bool attack)
    {
        int val = 0;

        if (attack)
        {
            if (!attackClipLock.Contains(1))
            {
                for (int x = 0; x < attackClipLock.Length; x++)
                    attackClipLock[x] = 1;
            }

            val = Utils.SkewedNum(attackClipLock);
            attackClipLock[val] = 0;

            return attackClips[val];
        }

        if (!generalClipLock.Contains(1))
        {
            for (int x = 0; x < generalClipLock.Length; x++)
                generalClipLock[x] = 1;
        }

        val = Utils.SkewedNum(generalClipLock);
        generalClipLock[val] = 0;

        return generalClips[val];
    }
}
