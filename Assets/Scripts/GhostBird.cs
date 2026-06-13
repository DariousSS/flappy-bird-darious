using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBird : MonoBehaviour
{
    private Transform playerTransform;
    private float xOffset;
    private float[] yHistory;
    private int historySize;
    private int index = 0;

    private AudioSource audioSource;

    public void Init(Transform target, float offsetX, int delayFrames = 10)
    {
        playerTransform = target;
        xOffset = offsetX;
        historySize = delayFrames;
        yHistory = new float[historySize];
        for (int i = 0; i < historySize; i++)
            yHistory[i] = transform.position.y;

        // 复制 Player 的碰撞器尺寸，用于检测 Scoring
        Player player = target.GetComponent<Player>();
        if (player != null)
        {
            CircleCollider2D playerCol = player.GetComponent<CircleCollider2D>();
            if (playerCol != null)
            {
                CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
                col.radius = playerCol.radius;
                col.isTrigger = true;
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 拿到 Player 的音效列表
        if (player != null)
            _noteClips = player.noteClips;
    }

    private AudioClip[] _noteClips;

    void Update()
    {
        if (playerTransform == null) return;

        yHistory[index] = playerTransform.position.y;
        index = (index + 1) % historySize;

        transform.position = new Vector3(
            playerTransform.position.x + xOffset,
            yHistory[index],
            transform.position.z
        );
        transform.eulerAngles = playerTransform.eulerAngles;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Scoring"))
        {
            if (_noteClips == null || _noteClips.Length == 0) return;
            audioSource.PlayOneShot(_noteClips[Player.noteIndex % _noteClips.Length]);
            Player.noteIndex++;
        }
    }
}
