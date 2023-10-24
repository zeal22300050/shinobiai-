using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �v���C���[�̏��
    private enum PlayerCondition
    {
        Wait,
        Move
    }
    private PlayerCondition playerCondition;

    [SerializeField]
    private Grid mapGrid; // �}�b�v�O���b�h
    [SerializeField]
    private GameDifficulityController difficulityController; // �ړ��񐔏���擾�p

    private int moveCount; // �ړ������񐔂�ۑ�����ϐ�

    private Vector2 moveDistance; // ��x�Ɉړ����鋗��
    private Vector3 arrowKeyInput; // ���L�[�̓��͂��擾����ϐ�

    private Vector3 oldPosition; // �O�t���[���̈ʒu��ۑ�����ϐ�
    private bool oldFrameKeyInput; // �O�t���[���̃L�[���͂�ۑ�����t���O

    // Start is called before the first frame update
    private void Start()
    {
        // �ŏ��͏����ʒu��ۑ�
        oldPosition = transform.position;
        // ���̈ړ��������Z���T�C�Y�ɍ��킹��
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    private void Update()
    {
        // �Ȃɂ��Ȃ��Ƃ��͑ҋ@��Ԃɂ���
        playerCondition = PlayerCondition.Wait;
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

    // �v���C���[�̈ړ�����
    private void PlayerMoveProcess()
    {
        // ���L�[�̓��͂��擾
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // �O�t���[���ŃL�[��������Ă��Ȃ������t���[���ŃL�[��������Ă�����
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            oldPosition = transform.position; // ���݂̈ʒu��ۑ�����
            playerCondition = PlayerCondition.Move; // �ړ���ԂɈڍs
            transform.position += arrowKeyInput; // �ړ�����
            moveCount++; // �ړ��񐔂𑝂₷
        }
        oldFrameKeyInput = ArrowKeyInput(); // ���t���[���̃L�[���͏���ۑ�
    }

    // ���L�[��������Ă��邩�ǂ�����Ԃ��֐�
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

    // �Փˉ�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �R���W�������Փ˂�����A�ۑ����ꂽ�ʒu�ɖ߂�
        transform.position = oldPosition;

        // �ǂɓ������ē����Ȃ������Ƃ��͈ړ��񐔂𑝂₳�Ȃ�
        moveCount--;
    }

    // ���݂̈ړ��񐔂�Ԃ��֐�
    public int GetCurrentMoveCount()
    {
        return moveCount;
    }

    // �v���C���[�̏�Ԃ�Ԃ��֐�
    public int GetPlayerCondition()
    {
        return (int)playerCondition;
    }
}
