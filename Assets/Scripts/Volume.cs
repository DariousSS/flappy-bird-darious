using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public float minSound = 200f;
    public float maxSound = 1000f;

    private Image fillImage;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    private void Update()
    {
        float level = SerialController.soundLevel;
        float filled = Mathf.InverseLerp(minSound, maxSound, level);
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, filled, Time.deltaTime * 10f);
    }
}
