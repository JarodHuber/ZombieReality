using UnityEngine;

public class SubWave
{
    public int wave = 0;
    public EnemyType enemyType = EnemyType.BASE;

    public float timestamp = 0.0f;

    public int amount = 0;

    private float time = 0.0f;
    private float spacing = 0.0f;
    private bool isStarted = false;

    public SubWave(int wave, float time, float spacing, int amount, EnemyType enemyType)
    {
        this.wave = wave;
        this.time = time;
        this.spacing = spacing;
        this.amount = amount;
        this.enemyType = enemyType;

        this.timestamp = Time.time;
    }

    /// <summary>
    /// Spawn the next enemy
    /// </summary>
    /// <returns>Returns the enemy to spawn</returns>
    public bool Spawn()
    {
        if (amount == 0)
            return false;

        if (timestamp + (spacing) < Time.time)
        {
            timestamp = Time.time;
            --amount;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tell when it's time to spawn the next Subwave
    /// </summary>
    /// <returns>Returns true when next Subwave is ready to start spawning</returns>
    public bool IsTime()
    {
        if (isStarted)
            return true;

        if (timestamp + (time) < Time.time)
        {
            isStarted = true;
            timestamp = Time.time - spacing; //Minus spacing so they spawn right away instead of with a delay
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tell when Subwave is done spawning
    /// </summary>
    /// <returns>Returns true when there are no more enemies to spawn in the Subwave</returns>
    public bool IsDone()
    {
        return amount <= 0;
    }

    /// <summary>
    /// resets the enemy timestamp
    /// </summary>
    public void ResetTimestamp()
    {
        timestamp = Time.time;
    }
}
