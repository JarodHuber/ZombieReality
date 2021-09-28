using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Transform head = null;
    [SerializeField] Counter health = new Counter(5.0f);
    [SerializeField] Image damageOverlay = null;
    [SerializeField] Hand leftHand = null, rightHand = null;
    [SerializeField] TMP_Text leftWrist = null, rightWrist = null;
    [SerializeField] float wristAngle = 80.0f;
    [SerializeField] Timer healthRegenDelay = new Timer(1.0f);
    [SerializeField] Timer healthRegenCycle = new Timer(0.1f);

    float[] attackSoundLock = new float[0];
    AudioSource src = null;
    Movement mover = null;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        mover = GetComponent<Movement>();
    }

    public Vector3 NavPos 
    { 
        get
        {
            Vector3 pos = head.position;
            pos.y = transform.position.y;
            return pos;
        } 
    }
    
    public float Height { get => head.position.y - transform.position.y; }

    private void Update()
    {
        if(damageOverlay.color.a != health.PercentComplete / 2.0f)
        {
            Color color = damageOverlay.color;
            color.a = health.PercentComplete / 2.0f;
            damageOverlay.color = color;
        }

        if (health.Cur > 0 && healthRegenDelay.Check(false))
        {
            if (healthRegenCycle.Check())
                health.Count(-1);

            if (health.Cur == 0)
                healthRegenDelay.Reset();
        }
    }

    public void TakeDamage(int damage)
    {
        print("player ow");
        healthRegenDelay.Reset();
        if (health.PreCheck(damage, false))
        {
            // TODO: Die
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
        }

        //PlayAttackSound();
    }

    void PlayAttackSound()
    {
        if (!attackSoundLock.Contains(1))
        {
            for (int x = 0; x < attackSoundLock.Length; x++)
                attackSoundLock[x] = 1;
        }

        int val = Utils.SkewedNum(attackSoundLock);
        attackSoundLock[val] = 0;

        // TODO: Implement getting hit sounds
    }

    public void WristUpdate(int enemyNum, int waveNum)
    {
        if (Vector3.Angle(leftWrist.transform.forward, head.forward) < wristAngle)
        {
            leftWrist.gameObject.SetActive(true);
            leftWrist.text = enemyNum + "\nenemies nearby";
        }
        else
            leftWrist.gameObject.SetActive(false);

        if (Vector3.Angle(rightWrist.transform.forward, head.forward) < wristAngle)
        {
            rightWrist.gameObject.SetActive(true);
            rightWrist.text = "wave " + waveNum;
        }
        else
            rightWrist.gameObject.SetActive(false);
    }

    public void TogglePause(bool pauseSet)
    {
        mover.isPaused = pauseSet;
        leftHand.isPaused = pauseSet;
        rightHand.isPaused = pauseSet;
    }
    public void TogglePause()
    {
        mover.isPaused = !mover.isPaused;
        leftHand.isPaused = !leftHand.isPaused;
        rightHand.isPaused = !rightHand.isPaused;
    }
}
