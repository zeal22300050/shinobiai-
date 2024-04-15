using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrihimeController : AllCharacterController
{
    /// <summary>
    /// �D�P�̍s���X�e�[�g
    /// </summary>
    private enum OrihimeState
    {
        /// <summary> �ҋ@ </summary>
        stay,
        /// <summary> ���� </summary>
        win,
        /// <summary> �s�k </summary>
        lose,
        /// <summary> �ޏ� </summary>
        exit
    };

    /// <summary>
    /// �D�P�̍s���X�e�[�g
    /// </summary>
    private OrihimeState orihimeState;
    
    // �D�P�̍s�����Ƃ̏���������z��
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(OrihimeState)).Length];

    // ���݂̈ʒu
    private Vector3 currentPosition;

    private Vector3 nextPosition;

    // ���݂̌���
    private Vector3 currentRotation;

    // ���̌���
    private Vector3 nextRotation;

    // �A�j���[�^�[�擾
    private Animator animator;

    [SerializeField]
    private PlayerController playerController;

    private int oldState;

    private bool moveFlag;

    // Start is called before the first frame update
    void Start()
    {
        // �s���X�e�[�g��ҋ@�ɂ���
        orihimeState = OrihimeState.stay;

        // �A�j���[�^�[�R���|�[�l���g���擾����
        animator = GetComponent<Animator>();

        // ������
        SetOrihimeStateFunctions();

        // ���݂̌���
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(0.0f, 180.0f, 0.0f);

        oldState = (int)playerController.playerState;

        moveFlag = false;
    }

    private void FixedUpdate()
    {
        if (oldState != ChangeAction())
        {
            // �D�P�̍s���Ăяo��
            ActionFunc(orihimeState);
        }
    }

    /// <summary>
    /// ��ԂɑΉ������֐���ݒ�
    /// </summary>
    private void SetOrihimeStateFunctions()
    {
        stateFunc[(int)OrihimeState.stay] = StayAciton;
        stateFunc[(int)OrihimeState.win] = WinAction;
        stateFunc[(int)OrihimeState.lose] = LoseAction;
        stateFunc[(int)OrihimeState.exit] = ExitAction;
    }

    /// <summary>
    /// �D�P�̏�ԂɑΉ������֐����Ăяo��
    /// </summary>
    /// <param name="state">
    /// �D�P�̏��
    /// </param>
    private void ActionFunc(OrihimeState state)
    {
        stateFunc[(int)state]();
    }

    /// <summary>
    /// �ҋ@���̏���
    /// </summary>
    private void StayAciton()
    {
    }

    /// <summary>
    /// �������̏���
    /// </summary>
    private void WinAction()
    {
        CharacterRotate(currentRotation, nextRotation);
        animator.SetBool("Win", true);
    }

    /// <summary>
    /// �s�k���̏���
    /// </summary>
    private void LoseAction()
    {
        animator.SetBool("Lose", true);
    }

    /// <summary>
    /// �ޏꎞ�̏���
    /// </summary>
    private void ExitAction()
    {
        if (!moveFlag)
        {
            MoveParamReset();
            animator.SetBool("Win", false);

            currentRotation = transform.rotation.eulerAngles;
            nextRotation = new Vector3(0.0f, 90.0f, 0.0f);

            currentPosition = transform.position;
            nextPosition = transform.position + new Vector3(3.0f * 3.0f, 0.0f, 0.0f);

            moveFlag = true;
        }

        if(rotateParsent <= 1.0f)
        {
            CharacterRotate(currentRotation, nextRotation);
            return;
        }

        CharacterMove(currentPosition, nextPosition, MoveSpeed);
    }

    /// <summary>
    /// �s����ς��邽�߂̏���
    /// </summary>
    private int ChangeAction()
    {
        switch (playerController.playerState)
        {
            case PlayerController.PlayerState.lose:
                orihimeState = OrihimeState.lose;
                break;

            case PlayerController.PlayerState.win:
                orihimeState = OrihimeState.win;
                break; ;

            case PlayerController.PlayerState.exit:
                orihimeState = OrihimeState.exit;
                break;
        }

        return (int)playerController.playerState;
    }
}
