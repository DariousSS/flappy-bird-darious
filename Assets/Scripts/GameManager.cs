using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;

    public int score { get; private set; } = 0;


    private bool isGameOver = false;
    private float reviveTimer = 0f;
    public float reviveTime = 2f;       // 照多久复活
    public int brightThreshold = 1000;   // 手电筒阈值
    private int ghostCount = 0;
    public int reviveCount = 0;

    private void Update()
    {
        if (!isGameOver) return;

        Debug.Log("GameOver state - Light: " + SerialController.lightLevel); // 加这行


        if (SerialController.lightLevel > brightThreshold)
        {
            reviveTimer += Time.unscaledDeltaTime; // 用unscaled因为游戏是暂停的
            if (reviveTimer >= reviveTime)
            {
                reviveTimer = 0f;
                isGameOver = false;
                Play();
            }
        }
        else
        {
            reviveTimer = 0f;
        }
    }


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
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
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
        // 创建幽灵鸟
        ghostCount++;
        GameObject ghost = new GameObject("Ghost_" + ghostCount);
        ghost.transform.position = player.transform.position;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = player.GetComponent<SpriteRenderer>().sprite;
        sr.color = new Color(1f, 1f, 1f, 0.5f); // 半透明

        GhostBird gb = ghost.AddComponent<GhostBird>();
        gb.Init(player.transform, -ghostCount * 1.5f, 10 * ghostCount);

        //playButton.SetActive(true);
        gameOver.SetActive(true);
        isGameOver = true;
        reviveTimer = 0f;
        Pause();

        //提高间隔
        reviveCount++;
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

}
