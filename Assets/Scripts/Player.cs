using UnityEngine;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float gravity = -30.123f;
    public float tilt = 5f;
    public float maxLift = 4f;

    public int maxHealth = 3;
    public int currentHealth;

    // 遮住光传感器多少秒回1血
    public float healTime = 1.5f;
    // 光线高于这个值算"手电筒照上了"
    public int brightThreshold = 300;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    public float topLimit = 5f;
    public float bottomLimit = -5f;

    private float healTimer = 0f;
    private float invincibleTimer = 0f;
    public float invincibleDuration = 2f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
        currentHealth = maxHealth;
        healTimer = 0f;
        invincibleTimer = 0f;
    }

    private void Update()
    {

        // 飞行控制
        float pitch = SerialController.soundLevel;
        float mapped = Mathf.InverseLerp(200f, 1000f, pitch);
        float force = mapped * maxLift;

        //Debug.Log("Sound: " + pitch);
        direction.y += force * Time.deltaTime;
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;

        // 边界死亡
        if (transform.position.y > topLimit || transform.position.y < bottomLimit)
            GameManager.Instance.GameOver();

        // 无敌时间倒计时
        if (invincibleTimer > 0f)
            invincibleTimer -= Time.deltaTime;

        // 光线回血
        //Debug.Log("Light: " + SerialController.lightLevel);
        if (SerialController.lightLevel > brightThreshold)
        {
            healTimer += Time.deltaTime;
            if (healTimer >= healTime && currentHealth < maxHealth)
            {
                currentHealth++;
                healTimer = 0f;
                Debug.Log("Healed! HP: " + currentHealth);
            }
        }
        else
        {
            healTimer = 0f;
        }
    }

    private void AnimateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Length) spriteIndex = 0;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (invincibleTimer > 0f) return;

            currentHealth--;
            invincibleTimer = invincibleDuration;
            Debug.Log("Hit! HP: " + currentHealth);

            // 弹回：速度反向+向上推
            direction.y = -direction.y * 0.5f;

            if (currentHealth <= 0)
                GameManager.Instance.GameOver();
        }
        else if (other.gameObject.CompareTag("Scoring"))
        {
            GameManager.Instance.IncreaseScore();
        }
    }
}