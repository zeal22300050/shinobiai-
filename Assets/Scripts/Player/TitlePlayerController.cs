using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TitlePlayerController : AllCharacterController
{
    [SerializeField]
    protected FadeManager fade;

    private Animator animator;

    /// <summary>
    /// �v���C���[�̏��
    /// </summary>
    [HideInInspector]
    public enum PlayerState
    {
        /// <summary> �ҋ@ </summary>
        stay,
        /// <summary> �ړ� </summary>
        move,
        /// <summary> ���� </summary>
        win,
    };

    /// <summary>
    /// �v���C���[�̏��
    /// </summary>
    private PlayerState playerState;

    /// <summary>
    /// ��ԏ����̊֐�������z��
    /// </summary>
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(PlayerState)).Length];

    //�ړ����鑬��
    private float speed = 20.0f;

    // ���݂̈ʒu
    private Vector3 currentPosition;

    // ���̈ړ��ʒu
    private Vector3 nextPosition;

    //��x�Ɉړ����鋗��
    private Vector3 moveDistance;

    //���̃}�X�܂ł̋���
    private float titleDistance = 20.0f;

    // ��]���y���W
    private float angle = 120.0f;

    // ���݂̊p�x��ێ�
    private Vector3 currentRotation;

    // ���̉�]�̊p�x
    private Vector3 nextRotation;

    //���̉�]�ʒu
    private float nowRotation = 0.0f;

    // �v���C���[�̌������
    private Vector3 lookDirection;

    //�\���L�[��������Ă��邩�̔���
    private bool gameDPad = false;

    //�X�e�B�b�N�ɓ��͂����邩�̔���
    private bool gameStick = false;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(currentRotation.x, angle, currentRotation.z);
        // �v���C���[�̏�Ԃ�ҋ@�ɂ���
        playerState = PlayerState.stay;

        SetStateFunction();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerAnimation();
    }

    private void FixedUpdate()
    {
        // �v���C���[�̏�Ԃ��Ƃ̏���
        StateProcess(playerState);
        //Debug.Log(prsentLocation);
    }

    /// <summary>
    /// ���ꂼ��̏�ԂɑΉ������֐���ݒ�
    /// </summary>
    private void SetStateFunction()
    {
        stateFunc[(int)PlayerState.stay] = StayProcess;
        stateFunc[(int)PlayerState.move] = MoveProcess;
        stateFunc[(int)PlayerState.win] = WinProcess;
    }

    /// <summary>
    /// �󂯎������ԂɑΉ������֐����Ăяo��
    /// </summary>
    /// <param name="state"> �v���C���[�̏�� </param>
    private void StateProcess(PlayerState state)
    {
        stateFunc[(int)state]();
    }

    private void PlayerAnimation()
    {
        switch (playerState)
        {
            // �v���C���[�̏�Ԃ��ҋ@�̏ꍇ
            case PlayerState.stay:
                // �A�j���[�V������؂�ւ���
                animator.SetBool("State_Walk", false);
                break;

            // �v���C���[�̏�Ԃ��ړ��̏ꍇ
            case PlayerState.move:
                // �A�j���[�V������؂�ւ���
                animator.SetBool("State_Walk", true);
                break;

            case PlayerState.win:
                // �A�j���[�V������؂�ւ���
                animator.SetBool("State_StageClear", true);
                //animator.SetBool("State_Walk", false);
                break;
        }
    }

    private void StayProcess()
    {
        if (ArrowKeyDown())
        {
            StayToMoveState();
        }
    }

    public void MoveProcess()
    {
        CharacterMove(currentPosition, nextPosition,speed);

        lookDirection = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        if (positionRatio > 1)
        {
            playerState = PlayerState.stay;
            currentPosition = transform.position;
            CharacterRotate(currentRotation,new Vector3(currentRotation.x,180.0f,currentRotation.z));
            MoveParamReset();
        }
    }

    private void WinProcess()
    {
        Invoke(nameof(PlayerMove), 3.0f);
    }

    private void StayToMoveState()
    {
        if (gameDPad)
        {
            // �Q�[���p�b�h�̏\���L�[�̓��͂��擾����
            moveDistance = new Vector3((int)Input.GetAxisRaw("D_Pad_H"), 0.0f, 0.0f);
            Debug.Log("pad");
        }
        else if (gameStick)
        {
            // �Q�[���p�b�h�̏\���L�[�̓��͂��擾����
            moveDistance = new Vector3((int)Input.GetAxisRaw("L_Stick_H"), 0.0f, 0.0f);
            Debug.Log("stick");
        }
        // ���̈ړ��ʒu���X�V����
        nextPosition = currentPosition + (moveDistance * titleDistance);

        if (PlayerMoveLimit())
        {
            // �v���C���[�̏�Ԃ��ړ��ɂ���
            playerState = PlayerState.move;
        }
    }

    protected void PlayerRotate()
    {
        // ��]������
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(currentRotation), Quaternion.Euler(nextRotation), nowRotation);
    }

    private void PlayerMove()
    {
        playerState = PlayerState.move;
    }

    /// <summary>
    /// �v���C���[�̈ړ������̐ݒ�
    /// </summary>
    /// <returns>
    /// �������� : true
    /// �������ĂȂ� : false
    /// </returns>
    private bool PlayerMoveLimit()
    {
        // �T������ʒu�����߂�
        Vector3 searchPosition = currentPosition;
        // Y���W����������
        searchPosition.y = currentPosition.y - 0.5f;

        // ���C�L���X�g�����̃R���C�_�[�ɓ���������c
        if (Physics.Raycast(searchPosition, moveDistance, titleDistance))
        {
            Debug.Log("MoveTrue");
            // �^��Ԃ�
            return true;
        }
        else // ������Ȃ���΁c
        {
            Debug.Log("MoveFalse");
            // �U��Ԃ�
            return false;
        }
    }

    /// <summary>
    /// �\���L�[�̓��͂��m�F����֐�
    /// </summary>
    /// <returns>
    /// ���͒� : true
    /// ������ : false
    /// </returns>
    private bool ArrowKeyDown()
    {
        bool directionKeyInput = (int)Input.GetAxisRaw("D_Pad_H") != 0;//|| (int)Input.GetAxisRaw("D_Pad_V") != 0;
        bool joyStickInput = (int)Input.GetAxisRaw("L_Stick_H") != 0;//|| (int)Input.GetAxisRaw("L_Stick_V") != 0;
        // �W���C�X�e�B�b�N�܂��͏\���L�[�̓��͂����m������
        if (joyStickInput)
        {
            gameDPad = false;
            gameStick = true;
            // �^��Ԃ�
            return true;
        }
        else if (directionKeyInput)
        {
            gameDPad = true;
            gameStick = false;
            // �^��Ԃ�
            return true;
        }
        else // �����łȂ����
        {
            gameDPad = false;
            gameStick = false;
            // �U��Ԃ�
            return false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Start")
        {
            Debug.Log("Start");
            playerState = PlayerState.stay;
            nextPosition = new Vector3(60.0f, -17.5f, 40.0f);
            StartCoroutine(fade.FadeOutAndLoadScene());
            fade.fadeOutPanel.enabled = true;

        }
    }

}
