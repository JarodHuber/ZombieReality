using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    enum GameState { START, PLAY, PAUSE, END }

    [SerializeField] bool IsPaused = true;
    [SerializeField] GameState currentState = GameState.START;
    [SerializeField] InputActionMap input = new InputActionMap();
    [SerializeField] PauseMenu pauseMenu = null;

    [Space(10)]
    [SerializeField] Player player = null;
    [SerializeField] EnemyManager enemyManager = null;
    [SerializeField] Transform spawnObjHolder = null;
    [SerializeField] float areaRadius = 200.0f;
    [SerializeField] 

    float playerTime = 0;

    bool hasPaused = false;

    private void Start()
    {
        SpawnObjects();

        input["Pause"].performed += Pause;
    }

    private void LateUpdate()
    {
        player.WristUpdate(enemyManager.currentNumberOfEnemies, enemyManager.waveNum);

        switch (currentState)
        {
            case GameState.START:
                if (IsPaused && !hasPaused)
                {
                    IsPaused = false;
                    StartCoroutine(PauseWait());
                    hasPaused = true;
                }
                break;
            case GameState.PLAY:
                Play();
                break;
            case GameState.PAUSE:
                break;
            case GameState.END:
                // End the game

                break;
        }
    }

    void Play()
    {
        playerTime += Time.deltaTime;
    }

    private void Pause(InputAction.CallbackContext obj)
    {
        TogglePause();
    }

    IEnumerator PauseWait()
    {
        while (player.Height == 0.0f)
            yield return null;

        TogglePause(!IsPaused);
    }

    public void TogglePause()
    {
        player.TogglePause();
        enemyManager.TogglePause();

        pauseMenu.gameObject.SetActive(!IsPaused);

        if (!IsPaused)
        {
            IsPaused = true;
            currentState = GameState.PAUSE;

            pauseMenu.Initialize(player.Height / 2.0f, 
                -new Vector3(player.head.forward.x, 0.0f, player.head.forward.z), 
                player.NavPos);

            return;
        }

        IsPaused = false;
        currentState = GameState.PLAY;
    }
    public void TogglePause(bool isPaused)
    {
        player.TogglePause(isPaused);
        enemyManager.TogglePause(isPaused);

        pauseMenu.gameObject.SetActive(isPaused);

        if (isPaused)
        {
            IsPaused = true;
            currentState = GameState.PAUSE;

            pauseMenu.Initialize(player.Height / 2.0f,
                -new Vector3(player.head.forward.x, 0.0f, player.head.forward.z),
                player.NavPos);

            return;
        }

        IsPaused = false;
        currentState = GameState.PLAY;
    }

    void SpawnObjects()
    {
        for(int x = 0; x < spawnObjHolder.childCount; ++x)
        {
            NavMesh.SamplePosition(Random.insideUnitSphere * areaRadius, out NavMeshHit hit, areaRadius, NavMesh.AllAreas & ~(1 << NavMesh.GetAreaFromName("EnemyOnly")));
            spawnObjHolder.GetChild(x).position = hit.position;
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("WavesCompleted", enemyManager.waveNum);
        PlayerPrefs.SetInt("ZombiesKilled", enemyManager.totalEnemiesKilled);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void OnDrawGizmos()
    {
        if(Selection.activeGameObject == gameObject)
            Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}