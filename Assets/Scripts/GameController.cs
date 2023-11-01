using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// �X�e�[�W��
    /// </summary>
    public enum StageName
    {
        Stage1,
        Stage2,
        Stage3,
    }
    /// <summary>
    /// �Q�[���̐i�s��
    /// </summary>
    public enum GameProgress
    {
        Start,
        InGame,
        Goal,
        GameOver
    }
    /// <summary>
    /// �X�e�[�W��
    /// </summary>
    private StageName stageName;

    /// <summary>
    /// �Q�[���̐i�s��
    /// </summary>
    private GameProgress gameProgress;

    /// <summary>
    /// �X�e�[�W���Ƃ̈ړ��񐔏����ۑ�����ϐ�
    /// </summary>
    private readonly Dictionary<StageName, int> moveLimit = new(){
        {StageName.Stage1, 10},
        {StageName.Stage2, 8},
        {StageName.Stage3, 6},
    };

    private void Start()
    {
        // �ŏ��̓Q�[�������
        gameProgress = GameProgress.Start;
    }

    /// <summary>
    /// �i�s�󋵂��Q�[�����ɂ��鎞�ɌĂ΂��֐�
    /// </summary>
    public void StartInGame()
    {
        // �i�s�󋵂��Q�[�����ɖ߂�
        gameProgress = GameProgress.InGame;
    }

    /// <summary>
    ///  �v���C���[���S�[���������ɌĂяo�����֐�
    /// </summary>
    public void Goal()
    {
        //�Q�[���̐i�s�󋵂��S�[���ɂ���
        gameProgress = GameProgress.Goal;
    }

    public void GameOver()
    {
        gameProgress = GameProgress.GameOver;
    }

    /// <summary>
    /// �Q�[���̐i�s�󋵂��擾����֐�
    /// </summary>
    /// <returns> �Q�[���i�s�� </returns>
    public GameProgress GetGameProgress()
    {
        return gameProgress;
    }

    // �X�e�[�W�����擾����֐�
    public StageName GetStageName()
    {
        return stageName;
    }

    /// <summary>
    /// �󂯎�����ԍ��ɑΉ������X�e�[�W�̈ړ��񐔏�����擾����
    /// </summary>
    /// <param name="stageNames"> �X�e�[�W�� </param>
    /// <returns> �ړ��񐔏�� </returns>
    public int GetMoveLimit(StageName stageNames)
    {
        return moveLimit[stageNames]; // �ړ��񐔏����Ԃ�
    }
}
