using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum SpawnStage { WAITFORWAVEEND, PREPAREWAVE, SPAWNENEMIES }

    [SerializeField] EnemyManager enemyManager;
    public bool Paused = false;

    [Space(10)]
    [Tooltip("locations the enemies can spawn in... is auto-set on start")]
    [SerializeField] SpawnBounds[] spawnBounds = new SpawnBounds[0];
    [Tooltip("types of enemies that can be spawned, enemy prefabs are added here")]
    [SerializeField] GameObject[] enemyTypes = new GameObject[0];
    [Tooltip("the minimum time in-between waves")]
    [SerializeField] float Delay = 10f;

    [Space(10)]
    [Tooltip("Explosion marker for new wave")]
    [SerializeField] GameObject explosionFab;
    [Tooltip("where explosion for new wave marker is spawned")]
    [SerializeField] Transform explosionPoint;

    [Space(10)]
    public SpawnStage stage = SpawnStage.WAITFORWAVEEND;
    public int waveNum, totalEnemiesForWave, currentNumberOfEnemies;

    List<SubWave> Wave = new List<SubWave>();

    AudioSource audioSource;
    float pauseTime = 0;
    bool lastVal = false;
    Timer waitTimer;

    float playerTime = 0;

    private void Start()
    {
        waitTimer = new Timer(Delay);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PauseCheck()) 
            return;

        playerTime += Time.deltaTime;

        if (stage == SpawnStage.WAITFORWAVEEND)
            Wait();
        else if (stage == SpawnStage.PREPAREWAVE)
            GenerateWave();
        else if (stage == SpawnStage.SPAWNENEMIES)
            SpawnWave();
    }

    /// <summary>
    /// Wait for the next wave
    /// </summary>
    public void Wait()
    {
        if (waitTimer.Check() && currentNumberOfEnemies <= totalEnemiesForWave / 2)
            stage = SpawnStage.PREPAREWAVE;
    }

    /// <summary>
    /// Prepare the next wave for spawning
    /// </summary>
    public void GenerateWave()
    {
        ++waveNum;
        totalEnemiesForWave = currentNumberOfEnemies;

        Wave.Add(new SubWave(waveNum, 0f, 0.5f, 6 + (int)(2.0f * (waveNum - 1)), EnemyType.BASE));
        if (waveNum > 6)
            for (int i = 0; i < waveNum / 6; i++)
                Wave.Add(new SubWave(waveNum, (i * 3) + 2, 0.75f, 1 + (int)(waveNum / 3), EnemyType.BASE));
        if (waveNum > 12)
            for (int i = 0; i < (waveNum / 6) - 1; i++)
                Wave.Add(new SubWave(waveNum, (i * 3) + 3, 1f, 1 + (int)(waveNum / 3), EnemyType.BASE));
        if (waveNum > 24)
            for (int i = 0; i < (waveNum / 6) - 2; i++)
                Wave.Add(new SubWave(waveNum, (i * 3) + 3, 1f, 1 + (int)(waveNum / 3), EnemyType.BASE));

        for(int x = 0; x < Wave.Count; ++x)
        {
            Wave[x].ResetTimestamp(); //We need this so that the timestamps are correct
            totalEnemiesForWave += Wave[x].amount;
        }

        currentNumberOfEnemies = totalEnemiesForWave;
        stage = SpawnStage.SPAWNENEMIES;
    }

    /// <summary>
    /// Spawn the enemies
    /// </summary>
    public void SpawnWave()
    {
        for (int x = 0; x < Wave.Count; x++)
        {
            if (Wave[x].IsTime())
            {
                if (Wave[x].Spawn())
                {
                    SpawnEnemy(Wave[x].enemyType, x);
                    CycleSpawnBounds();
                }
            }

            if (Wave[x].IsDone()) 
                Wave.Remove(Wave[x]);
        }

        if (Wave.Count == 0)
        {
            print("wave finished"); //Signal the end of the wave
            stage = SpawnStage.WAITFORWAVEEND;
        }
    }

    void SpawnEnemy(EnemyType type, int index)
    {
        Vector3 spawnLocation = spawnBounds[0].SpawnLoc();
        enemyManager.SetEnemy(Instantiate(enemyTypes[(int)type], spawnLocation, 
            Quaternion.LookRotation(new Vector3(enemyManager.player.transform.position.x, spawnLocation.y, enemyManager.player.transform.position.z) - spawnLocation, Vector3.up)), 
            ((int)Wave[index].enemyType < enemyTypes.Length) ? Wave[index].enemyType : EnemyType.BASE); // TODO: Replace instantiate with a pre-made pool
    }

    void CycleSpawnBounds()
    {
        SpawnBounds spawnTemp = spawnBounds[0];

        for (int i = 1; i < spawnBounds.Length; ++i)
            spawnBounds[(i - 1) % spawnBounds.Length] = spawnBounds[i];

        spawnBounds[spawnBounds.Length - 1] = spawnTemp;
    }

    bool PauseCheck()
    {
        if (Paused && !lastVal)
        {
            pauseTime = Time.time;
            lastVal = true;
        }

        if (!Paused && lastVal)
        {
            pauseTime = Time.time - pauseTime;
            lastVal = false;

            if (waveNum > 0)
            {
                for (int x = 0; x < Wave.Count; ++x)
                    Wave[x].timestamp += pauseTime;
            }
        }

        return Paused;
    }
}
