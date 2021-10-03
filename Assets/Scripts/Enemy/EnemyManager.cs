using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { BASE }
public class EnemyManager : MonoBehaviour
{
    public enum SpawnStage { WAITFORWAVEEND, PREPAREWAVE, SPAWNENEMIES }

    #region Variables
    bool isPaused = false;

    public Player player = null;

    [Space(10)]
    [Tooltip("Things you want the enemies to recognise in-between the player")]
    [SerializeField] LayerMask playerMask = new LayerMask();
    [Tooltip("List of general sounds the enemies make")]
    [SerializeField] EnemySounds enemySounds = null;

    [Space(10, order = 0)]
    [Header("Spawning Variables", order = 1)]
    [Tooltip("locations the enemies can spawn in... is auto-set on start")]
    [SerializeField] SpawnBounds[] spawnBounds = new SpawnBounds[0];
    [Tooltip("Base objects for the enemies, the enemy prefabs")]
    [SerializeField] GameObject[] enemyFabs = new GameObject[0];
    [Tooltip("Different types of enemy, the variables for them")]
    [SerializeField] Enemy[] enemyTypes = new Enemy[0];

    [Space(10)]
    [Tooltip("the minimum time in-between waves")]
    [SerializeField] float Delay = 10f;

    [Space(10)]
    [Tooltip("Explosion marker for new wave")]
    [SerializeField] ParticleSystem explosionEffect = null;
    [SerializeField] AudioSource explosionSource = null;

    [Header("Debug values, not to edit")]
    public SpawnStage stage = SpawnStage.WAITFORWAVEEND;
    public int waveNum, totalEnemiesForWave, currentNumberOfEnemies, totalEnemiesKilled;

    List<SubWave> Wave = new List<SubWave>();

    float pauseTime = 0;
    Timer waitTimer;

    bool enemiesDisabled = true;
    List<Enemy> enemies = new List<Enemy>();

    public Enemy this[int index]
    {
        get => enemies[index];
    }
    #endregion

    void Start()
    {
        waitTimer = new Timer(Delay);
        enemySounds.Initialize();
    }

    void Update()
    {
        if (isPaused)
            return;

        if (stage == SpawnStage.WAITFORWAVEEND)
            Wait();
        else if (stage == SpawnStage.PREPAREWAVE)
            GenerateWave();
        else if (stage == SpawnStage.SPAWNENEMIES)
            SpawnWave();

        for (int x = 0; x < enemies.Count; ++x)
            EnemyUpdate(enemies[x]);
    }

    public void TogglePause(bool pauseSet)
    {
        if (isPaused == pauseSet)
            return;

        // Pause
        if (!isPaused)
        {
            if (!enemiesDisabled)
            {
                for (int x = 0; x < enemies.Count; ++x)
                    enemies[x].Agent.StopAI = true;

                pauseTime = Time.time;

                enemiesDisabled = true;
            }

            isPaused = true;
            return;
        }

        // Un-Pause
        if (enemiesDisabled)
        {
            for (int x = 0; x < enemies.Count; ++x)
                enemies[x].Agent.StopAI = false;

            pauseTime = Time.time - pauseTime;

            if (waveNum > 0)
            {
                for (int x = 0; x < Wave.Count; ++x)
                    Wave[x].timestamp += pauseTime;
            }

            enemiesDisabled = false;
        }

        isPaused = false;
    }
    public void TogglePause()
    {
        // Pause
        if (!isPaused)
        {
            if (!enemiesDisabled)
            {
                for (int x = 0; x < enemies.Count; ++x)
                    enemies[x].Agent.StopAI = true;

                pauseTime = Time.time;

                enemiesDisabled = true;
            }

            isPaused = true;
            return;
        }

        // Un-Pause
        if (enemiesDisabled)
        {
            for (int x = 0; x < enemies.Count; ++x)
                enemies[x].Agent.StopAI = false;

            pauseTime = Time.time - pauseTime;

            if (waveNum > 0)
            {
                for (int x = 0; x < Wave.Count; ++x)
                    Wave[x].timestamp += pauseTime;
            }

            enemiesDisabled = false;
        }

        isPaused = false;
    }

    #region Spawning Methods
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

        for (int x = 0; x < Wave.Count; ++x)
        {
            Wave[x].ResetTimestamp(); //We need this so that the timestamps are correct
            totalEnemiesForWave += Wave[x].amount;
        }

        currentNumberOfEnemies = totalEnemiesForWave;
        stage = SpawnStage.SPAWNENEMIES;

        explosionEffect.Play();
        explosionSource.Play();
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
        SetEnemy(Instantiate(enemyFabs[(int)(((int)Wave[index].enemyType < enemyFabs.Length) ? Wave[index].enemyType : EnemyType.BASE)], spawnLocation,
            Quaternion.LookRotation(new Vector3(player.transform.position.x, spawnLocation.y, player.transform.position.z) - spawnLocation, Vector3.up)),
            ((int)Wave[index].enemyType < enemyFabs.Length) ? Wave[index].enemyType : EnemyType.BASE); // TODO: Replace instantiate with a pre-made pool
    }

    void CycleSpawnBounds()
    {
        SpawnBounds spawnTemp = spawnBounds[0];

        for (int i = 1; i < spawnBounds.Length; ++i)
            spawnBounds[(i - 1) % spawnBounds.Length] = spawnBounds[i];

        spawnBounds[spawnBounds.Length - 1] = spawnTemp;
    }
    #endregion

    #region Enemy Methods
    /// <summary>
    /// handles each enemy and updates what it's doing
    /// </summary>
    /// <param name="enemy">enemy to update</param>
    void EnemyUpdate(Enemy enemy)
    {
        if (!enemy.IsAlive)
        {
            KillEnemy(enemy);
            return;
        }
        enemy.PlaySound();

        Vector3 target = enemy.Agent.target;

        if (player.NavPos != target)
            target = player.NavPos;

        float distToPlayer = Vector3.Distance(enemy.Agent.NavPos, player.NavPos);
        bool rayCast = Physics.Raycast(enemy.Agent.transform.position, (player.transform.position - enemy.Agent.NavPos).normalized, 
            out RaycastHit hitInfo, distToPlayer, playerMask, QueryTriggerInteraction.Ignore);

        //Debug.DrawRay(enemy.Transform.position, (player.transform.position - enemy.Transform.position));

        if (!rayCast || hitInfo.transform.CompareTag("Enemy"))
        {
            enemy.Agent.speed = enemy.Speed;

            if (!rayCast && enemy.InReach(distToPlayer, out float distanceToBeAt))
            {
                if (distToPlayer < distanceToBeAt)
                {
                    target = player.NavPos + ((enemy.Agent.NavPos - player.NavPos).normalized * distanceToBeAt);

                    if (NavMesh.Raycast(enemy.Agent.NavPos, target, out NavMeshHit hit, playerMask))
                        target = hit.position;
                }

                if (distToPlayer == distanceToBeAt)
                    enemy.Agent.StopAI = true;
                else if (distToPlayer != distanceToBeAt)
                    enemy.Agent.StopAI = false;

                enemy.AttemptAttack(player);
            }
        }
        else
            enemy.Agent.speed = enemy.Speed / 2;

        if (target != enemy.Agent.target)
            enemy.Agent.SetDestination(target);
    }

    /// <summary>
    /// adds the enemy to the enemies list allowing it to function
    /// </summary>
    public void SetEnemy(GameObject e, EnemyType enemyType)
    {
        int enemyIndex = enemies.Count;
        enemies.Add(new Enemy(e.transform, e.GetComponent<AI>(), e.GetComponent<AudioSource>(), enemyTypes[(int)enemyType])); // TODO: Add enemy presets later
        enemies[enemyIndex].enemySounds = enemySounds;
    }

    void KillEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        --currentNumberOfEnemies;
        ++totalEnemiesKilled;
        Destroy(enemy.Transform.gameObject);
    }

    public Enemy GetEnemy(Transform reference)
    {
        for(int x = 0; x < enemies.Count; ++x)
        {
            if (enemies[x].Transform == reference)
                return enemies[x];
        }

        return null;
    }
    #endregion
}
