using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class FadeManager : MonoBehaviour
{
    private AudioSource audioSource;

    public Image fadeInPanel;

    public Image fadeOutPanel;

    public float fadeOutElapsedTime;

    public float fadeInElapsedTime;

    public float fadeStartTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        fadeOutPanel.enabled = false;
    }

    // Start is called before the first frame update
    void Update()
    {
        //if(fadeInPanel)
        FadeIn();
        //Aボタン(Selectボタン)を押したら
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) && !fadeOutPanel.enabled)
        {
            audioSource.Play();
            StartCoroutine(FadeOutAndLoadScene());

            fadeOutPanel.enabled = true;
        }
    }

    public void FadeIn()
    {
        float elasedTime = 0.0f;
        Color startColor = fadeInPanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);

        elasedTime += Time.deltaTime;
        float alfa = Mathf.Clamp01(elasedTime / fadeInElapsedTime);
        fadeInPanel.color = Color.Lerp(startColor, endColor, alfa);
    }

    //フェードアウトとシーンの切り替えを行う
    public IEnumerator FadeOutAndLoadScene()
    {
        float elasedTime = 0.0f;
        Color startColor = fadeOutPanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f);

        yield return new WaitForSeconds(fadeStartTime);

        while (elasedTime < fadeOutElapsedTime)
        {
            elasedTime += Time.deltaTime;
            float alfa = Mathf.Clamp01(elasedTime / fadeOutElapsedTime);
            fadeOutPanel.color = Color.Lerp(startColor, endColor, alfa);
            yield return null;
        }
        fadeOutPanel.color = endColor;
        SceneManager.LoadScene("TutorialGameScene");
    }

}
