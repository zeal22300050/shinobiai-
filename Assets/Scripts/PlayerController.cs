using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// �v���C���[�̏��
    /// </summary>
    private enum PlayerCondition
    {
        Wait,
        Move
    }
    private PlayerCondition playerCondition;

    /// <summary>
    /// ��r����
    /// </summary>
    private enum CompareResult
    {
        Defferrent,
        Same
    }
    /// <summary>
    /// �}�b�v�O���b�h�̑傫���擾�p
    /// </summary>
    [SerializeField]
    private Grid mapGrid;
    /// <summary>
    /// �X�e�[�W���擾�p
    /// </summary>
    [SerializeField]
    private GameDifficulityController difficulityController; 
    /// <summary>
    /// ����\���p�X�v���C�g�}�X�N
    /// </summary>
    [SerializeField]
    private GameObject spriteMask; 

    private int moveCount; // �ړ������񐔂�ۑ�����ϐ�

    private Vector2 moveDistance; // ��x�Ɉړ����鋗��
    private Vector3 arrowKeyInput; // ���L�[�̓��͂��擾����ϐ�

    private readonly List<Vector3> oldPosition = new(); // �ߋ��̈ʒu��ۑ�����z��
    private int oldPositionIndex; // �C�ӂ̕ۑ��ς݈ʒu�ɃA�N�Z�X���邽�߂̕ϐ�
    private bool oldFrameKeyInput; // �O�t���[���̃L�[���͂�ۑ�����t���O

    // Start is called before the first frame update
    private void Start()
    {
        // ���̈ړ��������Z���T�C�Y�ɍ��킹��
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    private void FixedUpdate()
    {
        // �ړ��񐔂��ړ�����𒴂��Ă��Ȃ��Ȃ�
        if (moveCount < difficulityController.GetMoveLimit(0))
        {
            // �v���C���[�̈ړ��������s��
            PlayerMoveProcess();
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// </summary>
    private void PlayerMoveProcess()
    {
        // ���L�[�̓��͂��擾
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // �O�t���[���ŃL�[��������Ă��Ȃ������t���[���ŃL�[��������Ă�����
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            playerCondition = PlayerCondition.Move; // �ړ���ԂɈڍs
            oldPosition.Add(transform.position); // ���݂̈ʒu��ۑ�����
            transform.position += arrowKeyInput; // �ړ�����
            // ���݈ʒu�ƕۑ��ς݈ʒu����v���Ȃ������Ȃ�
            if (ComparePosition() == CompareResult.Defferrent)
            {
                moveCount++; // �ړ��񐔂𑝂₷
                Instantiate(spriteMask, transform.position, Quaternion.identity); // �����\������
            }
        }
        else
        {
            // �Ȃɂ��Ȃ��Ƃ��͑ҋ@��Ԃɂ���
            //playerCondition = PlayerCondition.Wait;
        }
        oldFrameKeyInput = ArrowKeyInput(); // ���t���[���̃L�[���͏���ۑ�

    }

    /// <summary>
    /// ���L�[�̓��͂��m�F����֐�
    /// </summary>
    /// <returns> 
    /// ���͒� : true 
    /// ������ : false
    /// </returns>
    private bool ArrowKeyInput()
    {
        // ���L�[�̂����ꂩ�������ꂽ�Ȃ�
        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.LeftArrow) || 
            Input.GetKey(KeyCode.UpArrow) || 
            Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        }
        else // �����łȂ����
        {
            return false;
        }
    }

    /// <summary>
    /// ���݂̈ʒu�ƕۑ��ς݂̈ʒu���ׂĈ�v���s��v����Ԃ��֐�
    /// </summary>
    /// <returns>
    /// �����ۑ�����Ă��Ȃ����ۑ��ς݈ʒu�ƌ��݈ʒu���s��v������ : Defferrent
    /// �ۑ��ς݈ʒu�ƌ��݈ʒu����v���� : Same
    /// </returns>
    private CompareResult ComparePosition()
    {
        // oldPosition�ɉ����ۑ�����Ă��Ȃ��Ȃ�
        if (oldPosition == null)
        {
            // Defferrent��Ԃ�
            return CompareResult.Defferrent;
        }

        // oldPosition�̔z��T�C�Y�܂Ń��[�v����
        for (oldPositionIndex = 0; oldPositionIndex < oldPosition.Count; oldPositionIndex++)
        {
            // ���݈ʒu���ۑ��ς̈ʒu�ƈ�v���Ă�����
            if (transform.position == oldPosition[oldPositionIndex])
            {
                // Same��Ԃ�
                return CompareResult.Same;
            }
        }       

        // ��L�̏����ɓ�����Ȃ����Defferrent��Ԃ�
        return CompareResult.Defferrent;
    }

    // �Փˉ�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���O�ɕۑ����ꂽ�ʒu�ɖ߂�
        transform.position = oldPosition[--oldPositionIndex];
        // �ǂɓ������ē����Ȃ������Ƃ��͈ړ��񐔂𑝂₳�Ȃ�
        moveCount--;
    }

    /// <summary>
    /// ���݂̈ړ��񐔂�Ԃ��֐�
    /// </summary>
    /// <returns>
    /// �ړ���
    /// </returns>
    public int GetCurrentMoveCount()
    {
        return moveCount;
    }

    /// <summary>
    /// �v���C���[�̏�Ԃ�Ԃ��֐�
    /// </summary>
    /// <returns>
    /// ���݂̃v���C���[�̏��
    /// </returns>
    public int GetPlayerCondition()
    {
        return (int)playerCondition;
    }
}
