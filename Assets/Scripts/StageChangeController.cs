using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChangeController : MonoBehaviour
{
    private enum ActiveStage
    {
        stage01,
        stage02,
        stage03,
        stage04,
        stage05, // バグ防止用
        stage06  // バグ防止用
    };

    private static ActiveStage activeStage;

    [SerializeField]
    private GameObject[] stageObjects;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < stageObjects.Length; i++)
        {
            stageObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        stageObjects[(int)activeStage].SetActive(true);
    }

    /// <summary>
    /// ネクストボタンが押されたら実行する関数
    /// </summary>
    public void PushNextButton()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            activeStage++;

            if ((int)activeStage > 3)
            {
                // ゲームシーン移行時にステージ進行状況を初期化する
                activeStage = ActiveStage.stage01;

                // ゲームシーンを読み込む
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                // 現在アクティブなシーンを読み込む
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
