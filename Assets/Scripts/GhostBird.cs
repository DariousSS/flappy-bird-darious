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

    public void Init(Transform target, float offsetX, int delayFrames = 10)
    {
        playerTransform = target;
        xOffset = offsetX;
        historySize = delayFrames;
        yHistory = new float[historySize];
        for (int i = 0; i < historySize; i++)
            yHistory[i] = transform.position.y;
    }

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
}
