using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �v���C���[�Ɋւ���s��
/// </summary>
public class PlayerController : AllCharacterController
{
    private float ClearMotionTime { get { return 3.8f; } }
    private float StandingPosRatio { get { return 0.8f; } }
    private float RayPosY { get { return -0.3f; } }
    private Vector3 AfterExitPosition { get { return new Vector3(StageSpace * 3.0f, 0.0f, 0.5f); } }

    /// <summary>
    /// �v���C���[�̏��
    /// </summary>
    [HideInInspector]
    public enum PlayerState
    {
        /// <summary> �o�� </summary>
        appear,
        /// <summary> �ҋ@ </summary>
        stay,
        /// <summary> �ړ� </summary>
        move,
        /// <summary> �s�k </summary>
        lose,
        /// <summary> ���� </summary>
        win,
        /// <summary> �ޏ� </summary>
        exit,
    };

    public PlayerState playerState { get; private set; }

    /// <summary>
    /// ��ԏ����̊֐�������z��
    /// </summary>
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(PlayerState)).Length];

    private Animator animator;

    private AudioSource audioSource;

    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private Camera mainCameraObject;

    [SerializeField]
    private GameObject startObject;

    [SerializeField]
    private PlayerChangeMaterial changeMaterial;

    private Vector3 currentPosition;

    private Vector3 nextPosition;

    private Vector3 currentRotation;

    private Vector3 nextRotation;

    private Vector3 moveDistance;

    public Vector3 lookDirection { get; private set; }

    // �ڐG�����I�u�W�F�N�g�̈ʒu
    private Vector3 targetPosition;

    private Vector3 cameraPosition;

    public int currentTurnNum { get; private set; }

    private bool isHit;

    private bool oldKeyDown;

    private bool gameDPad;
    private bool gameStick;

    private string hitObjName;

    // Start is called before the first frame update
    private void Start()
    {
        // �v���C���[��o���Ԃɂ���
        playerState = PlayerState.appear;

        // �����ʒu��������
        currentPosition = transform.position;
        nextPosition = startObject.transform.position;

        // �p�����[�^�̏�����
        MoveParamReset();
        SetStateFunction();

        // �J�v�Z���R���C�_�[���擾����
        capsuleCollider = GetComponent<CapsuleCollider>();
        // �A�j���[�^�[���擾����
        animator = GetComponent<Animator>();
        // �I�[�f�B�I�\�[�X���擾����
        audioSource = GetComponent<AudioSource>();

        hitObjName = null;

        // �t���O��false�ɂ���
        isHit = false;

        gameDPad = false;
        gameStick = false;
    }

    private void FixedUpdate()
    {
        // �v���C���[�̏�Ԃ��Ƃ̏���
        StateProcess(playerState);

        oldKeyDown = ArrowKeyDown();
    }

    /// <summary>
    /// ���ꂼ��̏�ԂɑΉ������֐���ݒ�
    /// </summary>
    private void SetStateFunction()
    {
        stateFunc[(int)PlayerState.appear] = AppearProcess;
        stateFunc[(int)PlayerState.stay] = StayProcess;
        stateFunc[(int)PlayerState.move] = MoveProcess;
        stateFunc[(int)PlayerState.lose] = LoseProcess;
        stateFunc[(int)PlayerState.win] = WinProcess;
        stateFunc[(int)PlayerState.exit] = ExitProcess;
    }

    /// <summary>
    /// �󂯎������ԂɑΉ������֐����Ăяo��
    /// </summary>
    /// <param name="state"> �v���C���[�̏�� </param>
    private void StateProcess(PlayerState state)
    {
        stateFunc[(int)state]();
    }

    /// <summary>
    /// �o�ꎞ����
    /// </summary>
    private void AppearProcess()
    {
        // �����ē���
        animator.SetBool("State_Walk", true);
        CharacterMove(currentPosition, nextPosition, moveSpeed);

        // �ړ����I�������
        if (EndMove())
        {
            game.SetGameProgress(GameController.GameProgress.playing);
            // �v���C���[�̏�Ԃ�ҋ@�ɂ���
            playerState = PlayerState.stay;
            // ���݂̈ʒu��ێ�����
            currentPosition = transform.position;
            // ���`��Ԃ̃p�����[�^������������
            MoveParamReset();
        }
    }

    /// <summary>
    /// �ҋ@������
    /// </summary>
    private void StayProcess()
    {     
        // �����E�s�k����
        WinOrLose(isHit);

        capsuleCollider.enabled = true;
        // �ҋ@���[�V�����ɂȂ�
        animator.SetBool("State_Walk", false);

        // �Q�[���I������
        if (game.GameEnd())
        {
            return;
        }

        // �O�t���[���ňړ��{�^���������ꂸ�A���t���[���ŉ����ꂽ��c
        if (!oldKeyDown && ArrowKeyDown())
        {
            StayToMoveState(); // ��Ԃ�move�ɕύX
        }
    }

    /// <summary>
    /// �ړ�������
    /// </summary>
    private void MoveProcess()
    {
        // �Q�[���I������
        if (game.GameEnd())
        {
            return;
        }

        // �ړ��A�j���[�V�����Đ�
        animator.SetBool("State_Walk", true);

        // �����E�s�k����
        WinOrLose(isHit);

        // �ړ�����
        CharacterMove(currentPosition, nextPosition, moveSpeed);

        // �i�s�����ɉ�]������
        lookDirection = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // �ړ����I�������
        if (EndMove())
        {
            audioSource.Stop();
            // �v���C���[�̏�Ԃ�ҋ@�ɂ���
            playerState = PlayerState.stay;
            // ���݂̈ʒu��ێ�����
            currentPosition = transform.position;
            // ���`��Ԃ̃p�����[�^������������
            MoveParamReset();
            game.TurnEnd();
        }
    }

    /// <summary>
    /// �s�k������
    /// </summary>
    private void LoseProcess()
    {
        ChangeHitRotate(transform.rotation, targetPosition, transform.position); // ���������G�̕�������
        audioSource.Stop(); // BGM���X�g�b�v
        if (rotateSpeed >= 1.0f)
        {
            // �v���C���[�̕\���ύX
            if (changeMaterial.GetChangeMaterials() != PlayerChangeMaterial.ChangeMaterials.lose)
            {
                changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.surprise);
            }
            animator.SetBool("State_Findout", true); // �����������̃A�j���[�V�����ɕύX

            switch (hitObjName)
            {
                case "Rabit":
                    animator.SetBool("State_RabitFind", true); // �������Ɍ��������Ƃ��̃A�j���[�V����
                    break;

                case "Dragon":
                    animator.SetBool("State_DragonFind", true); // ���Ɍ��������Ƃ��̃A�j���[�V����
                    break;
            }
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void WinProcess()
    {
        // �v���C���[�̕\����Ί��
        changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.smile);
        audioSource.Stop(); // BGM���X�g�b�v

        cameraPosition = mainCameraObject.transform.position;
        ChangeHitRotate(transform.rotation, cameraPosition, transform.position); // ���������G�̕�������

        // ���s���[�V�������~
        animator.SetBool("State_Walk", false);

        // �X�e�[�W�N���A���[�V�������J�n
        animator.SetBool("State_StageClear", true);

        // �X�e�[�W�N���A���[�V��������莞�ԍs������
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > ClearMotionTime)
        {
            animator.SetBool("State_StageClear", false); // �X�e�[�W�N���A���[�V�������I��
            changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.normal); // �v���C���[�̕\������ɖ߂�
            
            // �ޏꎞ�Ɏg�p������̐ݒ�
            currentPosition = transform.position;
            nextPosition = transform.position + AfterExitPosition;
            currentRotation = transform.rotation.eulerAngles;
            nextRotation = new Vector3(0.0f, 90.0f, 0.0f);
            MoveParamReset();
            game.SetGameProgress(GameController.GameProgress.result);
            playerState = PlayerState.exit;
        }
    }

    /// <summary>
    /// �ޏꎞ�̏���
    /// </summary>
    private void ExitProcess()
    {
        // ���s���[�V�������J�n
        animator.SetBool("State_Walk", true);

        // �i�s�����������������I����Ă��Ȃ�������
        if (rotateParsent <= 1.0f)
        {
            CharacterRotate(currentRotation, nextRotation); // �v���C���[����]������
            return;
        }

        CharacterMove(currentPosition, nextPosition, MoveSpeed); // �v���C���[���ړ�������
    }

    /// <summary>
    /// �ҋ@��Ԃ���ړ���ԂɈڍs
    /// </summary>
    private void StayToMoveState()
    {
        if (gameDPad)
        {
            // �Q�[���p�b�h�̏\���L�[�̓��͂��擾����
            moveDistance = new Vector3((int)Input.GetAxisRaw("D_Pad_H"), 0.0f, (int)Input.GetAxisRaw("D_Pad_V"));
        }
        else if (gameStick)
        {
            // �Q�[���p�b�h�̃X�e�B�b�N���͂��擾����
            moveDistance = new Vector3((int)Input.GetAxisRaw("L_Stick_H"), 0.0f, (int)Input.GetAxisRaw("L_Stick_V"));
        }
        // ���̈ړ��ʒu���X�V����
        nextPosition = currentPosition + (moveDistance * StageSpace);

        // �ړ���ɃX�e�[�W�I�u�W�F�N�g����������c
        if (PlayerMoveLimit())
        {
            currentTurnNum++;
            // �^�[���o�߂�m�点��
            game.TurnStart();

            // �v���C���[�̏�Ԃ��ړ��ɂ���
            playerState = PlayerState.move;
            audioSource.Play();
        }
    }

    /// <summary>
    /// �������s�k�����肷��
    /// </summary>
    /// <param name="hitFlag">
    /// �I�u�W�F�N�g�ɓ����������ǂ���
    /// </param>
    private void WinOrLose(bool hitFlag)
    {
        // hit�t���O��false�Ȃ�
        if (!hitFlag)
        {
            return; // �������Ȃ�
        }
        // �A�j���[�V������؂�ւ���
        animator.SetBool("State_Walk", false);

        // ���������I�u�W�F�N�g�̖��O��
        switch (hitObjName)
        {
            // �e�̏ꍇ
            case "Rabit":
                // �v���C���[�̏�Ԃ�s�k�ɂ���
                playerState = PlayerState.lose;
                break;

            // �h���S���̏ꍇ
            case "Dragon":
                if (NotMoving())
                {
                    // �v���C���[�̏�Ԃ�s�k�ɂ���
                    playerState = PlayerState.lose;
                }
                break;

            // �S�[���̏ꍇ
            case "Goal":
                if (!(positionRatio < StandingPosRatio))
                {
                    // �v���C���[�̏�Ԃ������ɂ���
                    playerState = PlayerState.win;
                }
                break;
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
        bool joyStickInput = (int)Input.GetAxisRaw("L_Stick_H") != 0 || (int)Input.GetAxisRaw("L_Stick_V") != 0;
        bool directionKeyInput = (int)Input.GetAxisRaw("D_Pad_H") != 0 || (int)Input.GetAxisRaw("D_Pad_V") != 0;
        // �W���C�X�e�B�b�N�܂��͏\���L�[�̓��͂����m������
        if (joyStickInput)
        {
            gameDPad = false;
            gameStick = true;
            // �^��Ԃ�
            return true;
        }
        else if(directionKeyInput)
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
        // Y���W�������ɃZ�b�g����
        searchPosition.y = RayPosY;

        // ���C�L���X�g�ŏՓ˂��������ׂ�
        return Physics.Raycast(searchPosition, moveDistance, StageSpace);
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g�ɓ����������𔻒肷��
    /// </summary>
    /// <param name="other">
    /// �I�u�W�F�N�g���
    /// </param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "StageSE")
        {
            return;
        }

        targetPosition = other.transform.position;

        // �Ԃ������I�u�W�F�N�g�̃^�O���擾����
        hitObjName = other.tag;
        // �ڐG���������true�ɂ���
        isHit = true;
    }

    public bool PlayerStop()
    {
        return playerState == PlayerState.lose || playerState == PlayerState.win || playerState == PlayerState.exit;
    }

    public void EndDownAnimation()
    {
        changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.lose);
        game.SetGameProgress(GameController.GameProgress.gameover);
    }

    public void EndHappyAnimation()
    {
        // �Q�[���̐i�s��Ԃ��Q�[���N���A���ᖳ��������c
        if (game.gameProgress != GameController.GameProgress.goal)
        {
            // �Q�[���̐i�s��Ԃ��Q�[���N���A�ɂ���
            game.SetGameProgress(GameController.GameProgress.goal);
        }
    }

    public void EndCloakAnimation()
    {
        game.TurnEnd();
        animator.SetBool("State_Item", false);
        playerState = PlayerState.stay;
    }
}
