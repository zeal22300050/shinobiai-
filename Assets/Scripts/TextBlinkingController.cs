using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinkingController : MonoBehaviour
{
    /// <summary>
    /// 点滅するスピード
    /// </summary>
    public float blinkingSpeed = 1.0f;

    private Text text;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
       text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.color = GetAlphaColor(text.color);
    }

    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * blinkingSpeed;
        color.a = Mathf.Pow(Mathf.Sin(time),2);

        return color;
    }
}
