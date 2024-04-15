using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public enum GameProgress
    {
        start,
        playing,
        goal,
        gameover,
        result,
    };

    public GameProgress gameProgress { get; private set; }

    //フェードアウト用のimageの取得
    public Image fadeOutPanel;

    //フェードにかかる時間
    public float fadeOutElapsedTime;

    //フェードが始まるまでにかかる時間
    public float fadeStartTime;

    public bool fadeStart = false;

    public bool isChengeTurn { get; private set; }

    public bool isBackTitle { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // ゲームの進行状態をstartにする
        gameProgress = GameProgress.start;
        isChengeTurn = false;
        isBackTitle = false;

        fadeOutPanel.enabled = false;
    }

    private void FixedUpdate()
    {
        GameProgressState();
    }

    private void GameProgressState()
    {
        switch (gameProgress)
        {
            case GameProgress.start:
                gameProgress = GameProgress.playing;
                break;

            case GameProgress.playing:
                break;

            case GameProgress.goal:
                if(SceneManager.GetActiveScene().name == "GameScene" && !fadeStart)
                {
                    StartCoroutine(FadeOutAndLoadScene());
                    fadeStart = true;
                    fadeOutPanel.enabled = true;
                }
                break;

            case GameProgress.gameover:
                break;

            case GameProgress.result:
                break;
        }
    }

    /// <summary>
    /// ゲームの進行状態を変更する関数
    /// </summary>
    public void SetGameProgress(GameProgress progress)
    {
        gameProgress = progress;
    }

    /// <summary>
    /// ゲームが終わったかどうか確認する関数
    /// </summary>
    public bool GameEnd()
    {
        // ゲームの進行状態がクリアorゲームオーバーならtrue
        return
        gameProgress == GameProgress.goal ||
        gameProgress == GameProgress.gameover;
    }

    public void TurnStart()
    {
        isChengeTurn = true;
    }

    public void TurnEnd()
    {
        isChengeTurn = false;
    }

    /// <summary>
    /// タイトルボタンが押されたら実行する関数
    /// </summary>
    public void PushTitleButton()
    {
        if (gameProgress == GameProgress.gameover || gameProgress == GameProgress.goal || gameProgress == GameProgress.result)
        {
            // タイトルシーンを読み込む
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// リトライボタンが押されたら実行する関数
    /// </summary>
    public void PushRetryButton()
    {
        if (gameProgress == GameProgress.gameover)
        {
            // ゲームシーンを読み込む
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    private void BackTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    //呼ばれたらフェードアウトとシーンの切り替えをする関数
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
        SceneManager.LoadScene("TitleScene");
    }

}
