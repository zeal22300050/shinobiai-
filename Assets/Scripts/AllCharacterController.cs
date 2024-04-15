using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����N�^�[�̈ړ��E��]���Ǘ�����X�N���v�g
/// </summary>

public class AllCharacterController : MonoBehaviour
{
    protected float StageSpace { get { return 3.0f; } }

    protected float MoveSpeed { get { return 5.0f; } }

    [SerializeField]
    protected GameController game;
   
    [SerializeField]
    protected PlayerController player;

    /// <summary>
    /// ���݂̈ʒu�Ǝ��̈ړ��ʒu�̋���
    /// </summary>
    protected float distance;
    /// <summary>
    /// ���݂̈ʒu�̊���
    /// </summary>
    protected float positionRatio;

    protected float moveTime;

    protected float rotateTime;
    protected float rotateSpeed;
    protected float rotateParsent;

    protected float moveSpeed = 5.0f;

    /// <summary>
    /// �L�����N�^�[�̈ړ�����������֐�
    /// </summary>
    /// <param name="currentPos"> ���݂̈ʒu </param>
    /// <param name="nextPos"> �ړ���̈ʒu </param>
    /// <param name="speed"> �ړ����x </param>
    protected void CharacterMove(Vector3 currentPos, Vector3 nextPos,float speed)
    {
        // ���݂̈ʒu�Ǝ��̈ړ��ʒu�̋��������߂�
        distance = Vector3.Distance(currentPos, nextPos);
        // �f���^�^�C���𑫂�������
        moveTime += Time.deltaTime;
        // �ړ��̊��������߂�
        positionRatio = moveTime * speed / distance;

        // �ʒu���X�V����
        transform.position = Vector3.Lerp(currentPos, nextPos, positionRatio);
    }

    /// <summary>
    /// �L�����N�^�[�̉�]����������֐�
    /// </summary>
    /// <param name="currentRot"> ���݂̌��� </param>
    /// <param name="nextRot"> ���̌��� </param>
    /// <param name="par"> ��]�̊��� </param>
    protected void CharacterRotate(Vector3 currentRot, Vector3 nextRot)
    {
        distance = Vector3.Distance(new Vector3(0.0f,0.0f,0.0f), new Vector3(3.0f,0.0f,0.0f));

        rotateTime += Time.deltaTime;

        rotateParsent = (rotateTime * 5.0f) / distance;

        // ��]������
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(currentRot), Quaternion.Euler(nextRot), rotateParsent);
    }

    /// <summary>
    /// ���L�����Ɠ����������̉�]����
    /// </summary>
    /// <param name="rotation"> ���������u�Ԃ̊p�x </param>
    /// <param name="targetPos"> ���L�����̈ʒu </param>
    /// <param name="myPos"> �����̈ʒu </param>
    protected void ChangeHitRotate(Quaternion rotation, Vector3 targetPos, Vector3 myPos)
    {
        // ��]���鑬��
        rotateSpeed += Time.deltaTime / 2.0f;

        Quaternion currentRotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(targetPos - myPos), rotateSpeed);
        currentRotation.x = 0.0f;
        currentRotation.z = 0.0f;

        transform.rotation = currentRotation;
    }


    /// <summary>
    /// �ړ��Ɋւ���p�����[�^�����Z�b�g����֐�
    /// </summary>
    protected void MoveParamReset()
    {
        distance = 0.0f;
        rotateParsent = 0.0f;
        positionRatio = 0.0f;
        moveTime = 0.0f;
        rotateTime = 0.0f;
    }

    protected bool NotMoving()
    {
        return positionRatio <= 0.0f;
    }

    protected bool EndMove()
    {
        return positionRatio > 1.0f;
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns>
    /// �v���C���[������or��������true��Ԃ�
    /// </returns>
    protected bool GameEnd()
    {
        return player.playerState == PlayerController.PlayerState.lose || player.playerState == PlayerController.PlayerState.win;
    }
}
