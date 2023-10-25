using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficulityController : MonoBehaviour
{
    /// <summary>
    /// �X�e�[�W��
    /// </summary>
    private enum StageName
    {
        Stage1,
        Stage2,
        Stage3,
    }

    /// <summary>
    /// �X�e�[�W���Ƃ̈ړ��񐔏����ۑ�����ϐ�
    /// </summary>
    private readonly Dictionary<StageName, int> moveLimit = new(){
        {StageName.Stage1, 10},
        {StageName.Stage2, 8},
        {StageName.Stage3, 6},
    };

    /// <summary>
    /// �󂯎�����ԍ��ɑΉ������X�e�[�W�̈ړ��񐔏�����擾����
    /// </summary>
    /// <param name="stageNumber"> �X�e�[�W�ԍ� </param>
    /// <returns> �ړ��񐔏�� </returns>
    public int GetMoveLimit(int stageNumber)
    {
        // �Ή������X�e�[�W�ԍ����Ȃ��ꍇ
        if (moveLimit.Count <= stageNumber || 0 > stageNumber)
        {
            return 0; // 0��Ԃ�
        }
        return moveLimit[(StageName)stageNumber]; // �ړ��񐔏����Ԃ�
    }
}
