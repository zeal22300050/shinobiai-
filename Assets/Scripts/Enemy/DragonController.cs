using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̃h���S���Ɋւ���s��
/// </summary>
public class DragonController : AllCharacterController
{
    private enum DragonMoveState
    {
        stay,
        rotate
    };

    private DragonMoveState dragonMove;

    // ����������G�͈�
    [SerializeField]
    private GameObject searchArea;

    private Animator animator;

    private enum DragonAnimationState
    {
        /// <summary> �ҋ@ </summary>
        idel,
        /// <summary> ������ς��� </summary>
        turn,
        /// <summary> �u���X��f�� </summary>
        breath,
    };

    private DragonAnimationState dragonAnimaState;

    [SerializeField]
    // 1��̉�]��
    private float angle = 90.0f;

    [SerializeField]
    private GameObject breathEffect;

    // ���̉�]��
    private float nextAngle;

    // ���̉�]�̊p�x
    private Vector3 nextRotation;

    // ���݂̊p�x��ێ�
    private Vector3 currentRotation;

    // �p�����[�^���Đݒ肷�邩�̃t���O
    private bool isChange = false;

    private string turnDirection;

    // �{�b�N�X�R���C�_�[
    private BoxCollider boxCollider;

    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {  
        // �ŏ��̊p�x��ݒ肷��
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(currentRotation.x, currentRotation.y + angle, currentRotation.z);
        nextAngle = currentRotation.y + angle;

        // �p�x��������
        transform.rotation = Quaternion.Euler(currentRotation);

        // �E��肩����肩���ʂ���
        if(angle > 0.0f)
        {
            turnDirection = "turnRight";
        }
        else if(angle < 0.0f)
        {
            turnDirection = "turnLeft";
        }

        dragonMove = DragonMoveState.stay;
        dragonAnimaState = DragonAnimationState.idel;

        // �T�[�`�G���A�̐���
        searchArea = Instantiate(searchArea, new Vector3(transform.position.x, 0.0f, transform.position.z), Quaternion.Euler(nextRotation));
        animator = GetComponent<Animator>();

        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        bool endTerms = player.playerState == PlayerController.PlayerState.lose || player.playerState == PlayerController.PlayerState.win;

        if (endTerms && game.isChengeTurn)
        {
            Destroy(searchArea);
        }

        DragonAnimation();

        if (endTerms)
        {
            animator.SetBool("State_Stay", true);
            return;
        }

        dragonMove = ChangeDragonMoveState();

        DragonMove();

        ChangeAnimaState();
    }

    /// <summary>
    /// �h���S���̃A�j���[�V����
    /// </summary>
    private void DragonAnimation()
    {
        switch (dragonAnimaState)
        {
            // �h���S���̏�Ԃ��ҋ@�̏ꍇ
            case DragonAnimationState.idel:
                animator.SetBool("State_TurnR", false);
                animator.SetBool("State_TurnL", false);
                break;

            case DragonAnimationState.turn:
                switch (turnDirection)
                {
                    case "turnRight":
                        animator.SetBool("State_TurnR", true);
                        breathEffect.SetActive(false);
                        break;

                    case "turnLeft":
                        animator.SetBool("State_TurnL", true);
                        breathEffect.SetActive(false);
                        break;
                }
                break;

            case DragonAnimationState.breath:
                animator.SetBool("State_Breath", true);
                breathEffect.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// �A�j���[�V�����̑J�ڏ���
    /// </summary>
    private void ChangeAnimaState()
    {
        if (rotateParsent != 0.0f)
        {
            // �A�j���[�V������������ς���ɂ���
            dragonAnimaState = DragonAnimationState.turn;
        }
        if (rotateParsent == 0.0f)
        {
            // �A�j���[�V����������X����ɂ���
            dragonAnimaState = DragonAnimationState.idel;
        }

        if(isHit)
        {
            dragonAnimaState = DragonAnimationState.breath;
        }
    }

    /// <summary>
    /// ���̉�]�ɕK�v�ȃp�����[�^��ݒ肷��֐�
    /// </summary>
    private void NextRotationParam()
    {
        // ���݂̊p�x���X�V����
        currentRotation = nextRotation;

        // ���̊p�x���X�V����
        nextAngle += angle;

        // ���̉�]�̊p�x��ݒ肷��
        nextRotation.y = nextAngle;

        searchArea.transform.rotation = Quaternion.Euler(nextRotation);

        // ���ɉ�]����ʂ�180�x�����̊p�x��360�ȏゾ������c
        if (nextAngle >= 360.0f)
        {
            // ���ꂼ��̃p�����[�^����-360����
            nextAngle -= 360.0f;
            nextRotation.y -= 360.0f;
            currentRotation.y -= 360.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isHit = true;
        }
    }

    private DragonMoveState ChangeDragonMoveState()
    {
        if(game.isChengeTurn)
        {
            return DragonMoveState.rotate;
        }
        else
        {
            return DragonMoveState.stay;
        }
    }

    /// <summary>
    /// �h���S���̍s������
    /// </summary>
    private void DragonMove()
    {
        switch (dragonMove)
        {
            case DragonMoveState.stay:
                boxCollider.enabled = true;
                // �t���O��true��������c
                if (isChange)
                {
                    MoveParamReset();
                    // �p�����[�^���Đݒ肷��
                    NextRotationParam();
                    // �t���O��false�ɂ���
                    isChange = false;
                }
                break;

            case DragonMoveState.rotate:
                boxCollider.enabled = false;
                // �h���S������]������
                CharacterRotate(currentRotation, nextRotation);
                // �t���O��true�ɂ���
                isChange = true;
                break;
        }
    }
    public void IdelBreathStart()
    {
        breathEffect.SetActive(true);
    }

    public void IdelBreathEnd()
    {
        breathEffect.SetActive(false);
    }
}
