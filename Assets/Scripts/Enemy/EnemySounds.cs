using System.Linq;
using UnityEngine;

static class EnemySounds
{
    static AudioClip[] clips = new AudioClip[0];
    static float[] clipLock = new float[0];

    public static AudioClip Clip(int index)
    {
        if (index >= clips.Length)
            return null;

        return clips[index];
    }
    public static int ClipCount()
    {
        return clips.Length;
    }

    public static void Initialize(AudioClip[] enemySounds)
    {
        clips = enemySounds;

        clipLock = new float[enemySounds.Length];
        for(int x = 0; x <clipLock.Length; ++x)
            clipLock[x] = 1;
    }

    public static AudioClip GetGeneralSound()
    {
        if (!clipLock.Contains(1))
        {
            for (int x = 0; x < clipLock.Length; x++)
                clipLock[x] = 1;
        }

        int val = Utils.SkewedNum(clipLock);
        clipLock[val] = 0;

        return clips[val];
    }
}
