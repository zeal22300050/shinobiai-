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
        stage05, // �o�O�h�~�p
        stage06  // �o�O�h�~�p
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
    /// �l�N�X�g�{�^���������ꂽ����s����֐�
    /// </summary>
    public void PushNextButton()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            activeStage++;

            if ((int)activeStage > 3)
            {
                // �Q�[���V�[���ڍs���ɃX�e�[�W�i�s�󋵂�����������
                activeStage = ActiveStage.stage01;

                // �Q�[���V�[����ǂݍ���
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                // ���݃A�N�e�B�u�ȃV�[����ǂݍ���
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
