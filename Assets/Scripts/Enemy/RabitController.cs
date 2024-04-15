using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̃E�T�M�Ɋւ���s��
/// </summary>
public class RabitController : AllCharacterController
{
    /// <summary>
    /// �E�T�M���ړ��������
    /// </summary>
    private enum RabitMoveDirection
    {
        /// <summary> �O���� </summary>
        up,
        /// <summary> ����� </summary>
        down,
        /// <summary> �E���� </summary>
        right,
        /// <summary> ������ </summary>
        left
    };

    /// <summary>
    /// �E�T�M���ړ��������
    /// </summary>
    [SerializeField]
    private RabitMoveDirection rabitMove;

    /// <summary>
    /// �E�T�M�̃A�j���[�V������
    /// </summary>
    private enum RabitAnimaState
    {
        /// <summary> �ҋ@ </summary>
        idel,
        /// <summary> �ړ� </summary>
        jump,
        /// <summary> ���˂� </summary>
        attack,
    };

    /// <summary>
    /// �E�T�M�̃A�j���[�V������
    /// </summary>
    private RabitAnimaState rabitAnima;

    private Animator animator;

    private Transform trans;

    [SerializeField]
    private GameObject searchArea;

    /// <summary>
    /// �Փ˂��郌�C���[�̖��O
    /// </summary>
    [SerializeField]
    private LayerMask layerMask;

    /// <summary>
    /// �i�s�����ɑ��ꂪ���邩�T������ʒu
    /// </summary>
    private Vector3 searchPosition;

    // ���݂̈ʒu
    private Vector3 currentPosition;

    // ���̈ړ��ʒu
    private Vector3 nextPosition;

    // ���̍��G�ʒu
    private Vector3 nextSearchPosition;

    private Vector3 localAngle;

    /// <summary>
    /// �X�e�[�W�������Ă��邩���ׂ����
    /// </summary>
    private Vector3[] distances =
    {
        new Vector3(0.0f,0.0f,+1.0f), // ��
        new Vector3(0.0f,0.0f,-1.0f), // �O
        new Vector3(+1.0f,0.0f,0.0f), // �E
        new Vector3(-1.0f,0.0f,0.0f), // ��
    };

    // ���̌���
    private float nextRotation;

    private bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        // ���݂̈ʒu���擾����
        currentPosition = trans.position;

        NextPositionParam();

        localAngle = trans.localEulerAngles;

        // �T�[�`�G���A�̐���
        searchArea = Instantiate(searchArea, nextSearchPosition, Quaternion.identity);

        rabitAnima = RabitAnimaState.idel;

        animator = GetComponent<Animator>();

        isHit = false;
    }

    private void FixedUpdate()
    {
        RabitAnimation(); // �A�j���[�V��������

        // �I�����Ă��邩�^�[�����I����Ă��Ȃ��������͓����Ă��Ȃ�������
        if (GameEnd() && (game.isChengeTurn || NotMoving()))
        {
            Destroy(searchArea); // ���G�͈͂��폜����
            ChangeHitRotate(trans.rotation, player.transform.position, currentPosition); // �v���C���[�̕�������

            if (!isHit)
            {
                rabitAnima = RabitAnimaState.idel;
            }
            return;
        }

        MoveProcess();
    }

    /// <summary>
    /// �A�j���[�V�����̕ύX
    /// </summary>
    private void RabitAnimation()
    {
        switch(rabitAnima)
        {
            case RabitAnimaState.idel:
                animator.SetBool("State_Jump", false);
                animator.SetBool("State_Headbutt", false);
                break;

            case RabitAnimaState.jump:
                animator.SetBool("State_Jump", true);
                break;

            case RabitAnimaState.attack:
                animator.SetBool("State_Headbutt", true);
                break;
        }
    }

    private void MoveProcess()
    {
        // �U����ԂłȂ��Ȃ�
        if (rabitAnima != RabitAnimaState.attack)
        {
            MoveLimit(); // �ړ���������
            RabitAngle(); // ��]����
        }

        // �v���C���[�̃^�[�����Ȃ�
        if (game.isChengeTurn)
        {
            rabitAnima = RabitAnimaState.jump; // �W�����v��ԂɑJ��
            CharacterMove(currentPosition, nextPosition, MoveSpeed); // �ړ�����

            // �ړ����I�������
            if (EndMove())
            {
                rabitAnima = RabitAnimaState.idel; // �ҋ@��ԂɑJ��
            }

        }
        else // �����łȂ��Ȃ�
        {
            NextPositionParam(); // ���̈ړ�����W��ݒ�

            // �ړ����I�������
            if (EndMove())
            {
                MoveParamReset(); // �p�����[�^�����Z�b�g
            }

            rabitAnima = RabitAnimaState.idel; // �ҋ@��ԂɑJ��
        }

    }

    private void RabitAngle()
    {
        switch (rabitMove)
        {
            // �e�̈ړ��������O�̏ꍇ
            case RabitMoveDirection.up:
                nextRotation = 0.0f;
                break;

            // �e�̈ړ���������̏ꍇ
            case RabitMoveDirection.down:
                nextRotation = 180.0f;
                break;

            // �e�̈ړ��������E�̏ꍇ
            case RabitMoveDirection.right:
                nextRotation = 90.0f;
                break;

            // �e�̈ړ����������̏ꍇ
            case RabitMoveDirection.left:
                nextRotation = -90.0f;
                break;
        }
        localAngle.y = nextRotation;
        trans.localEulerAngles = localAngle;

    }

    /// <summary>
    /// ���ݒn���X�V���邽�߂̊֐�
    /// </summary>
    private void NextPositionParam()
    {
        // ���݂̈ʒu���擾����
        currentPosition = trans.position;

        // ���̈ʒu�����݂̈ʒu�ɐݒ肷��
        nextPosition = currentPosition;

        switch(rabitMove)
        {
            // �e�̈ړ���������̏ꍇ
            case RabitMoveDirection.up:
                nextPosition.z += StageSpace;
                break;

            // �e�̈ړ����������̏ꍇ
            case RabitMoveDirection.down:
                nextPosition.z -= StageSpace;
                break;

            // �e�̈ړ��������E�̏ꍇ
            case RabitMoveDirection.right:
                nextPosition.x += StageSpace;
                break;

            // �e�̈ړ����������̏ꍇ
            case RabitMoveDirection.left:
                nextPosition.x -= StageSpace;
                break;
        }

        nextSearchPosition = new Vector3(nextPosition.x, 0.0f, nextPosition.z);

        if (searchArea)
        {
            searchArea.transform.position = nextSearchPosition;
        }
    }

    private void MoveLimit()
    {
        // �T�����J�n����ʒu��ݒ肷��
        searchPosition = currentPosition;
        // �T������Y���W����������
        searchPosition.y = -0.3f;

        int a = (int)rabitMove;

        if (!Physics.Raycast(searchPosition, distances[a], StageSpace, layerMask))
        {
            switch (rabitMove)
            {
                // �e�̈ړ��������O�̏ꍇ
                case RabitMoveDirection.up:
                    rabitMove = RabitMoveDirection.down;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;

                // �e�̈ړ���������̏ꍇ
                case RabitMoveDirection.down:
                    rabitMove = RabitMoveDirection.up;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;

                // �e�̈ړ��������E�̏ꍇ
                case RabitMoveDirection.right:
                    rabitMove = RabitMoveDirection.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;

                // �e�̈ړ����������̏ꍇ
                case RabitMoveDirection.left:
                    rabitMove = RabitMoveDirection.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rabitAnima = RabitAnimaState.attack;
            isHit = true;
        }
    }
}
