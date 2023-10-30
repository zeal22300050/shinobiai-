using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

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
    /// <summary>
    /// �v���C���[�̏��
    /// </summary>
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
    /// �c��萔�\���p
    /// </summary>
    [SerializeField]
    private Slider slider;
    /// <summary>
    /// �X�e�[�W���擾�p
    /// </summary>
    [SerializeField]
    private GameController gameController; 
    /// <summary>
    /// ����\���p�X�v���C�g�}�X�N
    /// </summary>
    [SerializeField]
    private GameObject spriteMask;

    private const float DefaultSliderValue = 1.0f; // �X���C�_�[�̏�����

    private readonly List<GameObject> maskObject = new(); // �X�v���C�g�}�X�N��ۑ����Ă����z��

    private Vector3 defaultPosition; // �����ʒu
    private Vector2 moveDistance; // ��x�Ɉړ����鋗��
    private Vector3 arrowKeyInput; // ���L�[�̓��͂��擾����ϐ�

    private int moveCount; // �ړ������񐔂�ۑ�����ϐ�
    private float decreaseGauge; // ���̈ړ��Ō���Q�[�W�̗�

    private readonly List<Vector3> oldPosition = new(); // �ߋ��̈ʒu��ۑ�����z��
    private int oldPositionIndex; // �C�ӂ̕ۑ��ς݈ʒu�ɃA�N�Z�X���邽�߂̕ϐ�
    private bool oldFrameKeyInput; // �O�t���[���̃L�[���͂�ۑ�����t���O

    private int turnNumber = 1; // �^�[����(�����l��1)

    // Start is called before the first frame update
    private void Start()
    {
        slider.value = DefaultSliderValue;// ������Ԃł̓Q�[�W�𖞃^���ɂ��Ă���
        decreaseGauge = DefaultSliderValue / (float)gameController.GetMoveLimit(StageName.Stage1); // ���̈ړ��Ō���Q�[�W�ʂ��v�Z
        defaultPosition = transform.position; // �����ʒu�̐ݒ�(��ňʒu�擾�Ȃǂɕς�����)
        // ���̈ړ��������Z���T�C�Y�ɍ��킹��
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    private void Update()
    {
        // �S�[��������
        if (gameController.GetGameProgress() == GameProgress.Goal)
        {
            return; // ����ȍ~�̏������s��Ȃ�
        }

        // ���Z�b�g�{�^���������ꂽ��
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProcess(); // ���Z�b�g�������s��
        }
    }

    private void FixedUpdate()
    {
        // �S�[��������
        if (gameController.GetGameProgress() == GameProgress.Goal)
        {
            return; // ����ȍ~�̏������s��Ȃ�
        }

        // �ړ��񐔂��ړ�����𒴂��Ă��Ȃ����
        if (moveCount < gameController.GetMoveLimit(StageName.Stage1))
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

            turnNumber++; // �^�[�����𑝂₷

            // ���݈ʒu�ƕۑ��ς݈ʒu����v���Ȃ������Ȃ�
            if (ComparePosition() == CompareResult.Defferrent)
            {
                moveCount++; // �ړ��񐔂𑝂₷
                slider.value = DefaultSliderValue - decreaseGauge * moveCount; // �Q�[�W�����炷
                maskObject.Add(Instantiate(spriteMask, transform.position, Quaternion.identity)); // �����\������
            }
        }
        else
        {
            // �Ȃɂ��Ȃ��Ƃ��͑ҋ@��Ԃɂ���
            playerCondition = PlayerCondition.Wait;
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

    /// <summary>
    /// ���Z�b�g���̏���
    /// </summary>
    private void ResetProcess()
    {
        // �ʒu�̏�����
        transform.position = defaultPosition;
        // �ړ��񐔂̏�����
        moveCount = 0;
        // �Q�[�W�̏�����
        slider.value = DefaultSliderValue;
        // �z��NULL�Ȃ炱��ȍ~�̏������s��Ȃ�
        if (oldPosition == null || maskObject == null)
        {
            return;
        }
        // mask�I�u�W�F�N�g�����ׂč폜����
        for (int i = 0; i < maskObject.Count; i++)
        {
            Destroy(maskObject[i]);
        }
        // �ۑ����ꂽ�ʒu���폜����
        oldPosition.Clear();
        // �ۑ����ꂽ�I�u�W�F�N�g�����폜����
        maskObject.Clear();
        // �Q�[���i�s�󋵂��Q�[���X�^�[�g���ɖ߂�
        gameController.StartInGame();
    }

    // �Փˉ�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���O�ɕۑ����ꂽ�ʒu�ɖ߂�
        transform.position = oldPosition[--oldPositionIndex];

        // �ǂɓ������ē����Ȃ������Ƃ��͈ړ��񐔂ƃ^�[�����𑝂₳�Ȃ�
        moveCount--;
        turnNumber--;

        // �Q�[�W�ɕύX��K�p����
        slider.value = DefaultSliderValue - decreaseGauge * moveCount; 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            // �S�[���֐����Ăяo��
            gameController.Goal();
        }
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

    /// <summary>
    /// ���݂̃^�[������Ԃ��֐�
    /// </summary>
    /// <returns>���݂̃^�[����</returns>
    public int GetTurnNumber()
    {
        return turnNumber;
    }
}
