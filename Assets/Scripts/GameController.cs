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

    //�t�F�[�h�A�E�g�p��image�̎擾
    public Image fadeOutPanel;

    //�t�F�[�h�ɂ����鎞��
    public float fadeOutElapsedTime;

    //�t�F�[�h���n�܂�܂łɂ����鎞��
    public float fadeStartTime;

    public bool fadeStart = false;

    public bool isChengeTurn { get; private set; }

    public bool isBackTitle { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���̐i�s��Ԃ�start�ɂ���
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
    /// �Q�[���̐i�s��Ԃ�ύX����֐�
    /// </summary>
    public void SetGameProgress(GameProgress progress)
    {
        gameProgress = progress;
    }

    /// <summary>
    /// �Q�[�����I��������ǂ����m�F����֐�
    /// </summary>
    public bool GameEnd()
    {
        // �Q�[���̐i�s��Ԃ��N���Aor�Q�[���I�[�o�[�Ȃ�true
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
    /// �^�C�g���{�^���������ꂽ����s����֐�
    /// </summary>
    public void PushTitleButton()
    {
        if (gameProgress == GameProgress.gameover || gameProgress == GameProgress.goal || gameProgress == GameProgress.result)
        {
            // �^�C�g���V�[����ǂݍ���
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// ���g���C�{�^���������ꂽ����s����֐�
    /// </summary>
    public void PushRetryButton()
    {
        if (gameProgress == GameProgress.gameover)
        {
            // �Q�[���V�[����ǂݍ���
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    private void BackTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    //�Ă΂ꂽ��t�F�[�h�A�E�g�ƃV�[���̐؂�ւ�������֐�
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
