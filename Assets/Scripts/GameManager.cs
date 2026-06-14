using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject instruction;   // 说明文字
    [SerializeField] private Text countdownText;       // 倒计时数字

    public int score { get; private set; } = 0;

    private bool isGameOver = false;
    private float reviveTimer = 0f;
    public float reviveTime = 2f;
    public int brightThreshold = 1000;
    private int ghostCount = 0;
    public int reviveCount = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        gameOver.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        Pause();
    }

    private void Update()
    {
        if (!isGameOver) return;

        Debug.Log("GameOver state - Light: " + SerialController.lightLevel);

        if (SerialController.lightLevel > brightThreshold)
        {
            reviveTimer += Time.unscaledDeltaTime;
            if (reviveTimer >= reviveTime)
            {
                reviveTimer = 0f;
                isGameOver = false;
                StartCoroutine(CountdownThenPlay(false));
            }
        }
        else
        {
            reviveTimer = 0f;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    // 点击Play按钮时调用
    public void OnPlayButton()
    {
        playButton.SetActive(false);
        StartCoroutine(CountdownThenPlay(true));
    }

    private IEnumerator CountdownThenPlay(bool showInstruction = false)
    {
        // 只有第一次开始才显示说明文字
        if (instruction != null) instruction.SetActive(showInstruction);
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);

        int count = 3;
        while (count > 0)
        {
            if (countdownText != null) countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        // 隐藏说明和倒计时，显示分数，开始游戏
        if (instruction != null) instruction.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        Play();
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();
        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void GameOver()
    {
        ghostCount++;
        GameObject ghost = new GameObject("Ghost_" + ghostCount);
        ghost.transform.position = player.transform.position;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = player.GetComponent<SpriteRenderer>().sprite;
        sr.color = new Color(1f, 1f, 1f, 0.5f);

        GhostBird gb = ghost.AddComponent<GhostBird>();
        gb.Init(player.transform, -ghostCount * 1.5f, 10 * ghostCount);

        gameOver.SetActive(true);
        isGameOver = true;
        reviveTimer = 0f;
        Pause();

        reviveCount++;
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
