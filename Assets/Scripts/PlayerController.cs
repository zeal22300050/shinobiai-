using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Grid mapGrid; // �}�b�v�O���b�h

    private Vector2 moveDistance; // ��x�Ɉړ����鋗��
    private Vector3 arrowKeyInput; // ���L�[�̓��͂��擾����ϐ�

    private Vector3 oldPosition; // �O�t���[���̈ʒu��ۑ�����ϐ�
    private bool oldFrameKeyInput; // �O�t���[���̃L�[���͂�ۑ�����t���O

    // Start is called before the first frame update
    void Start()
    {
        // �ŏ��͏����ʒu��ۑ�
        oldPosition = transform.position;
        // ���̈ړ��������Z���T�C�Y�ɍ��킹��
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���L�[�̓��͂��擾
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // �O�t���[���ŃL�[��������Ă��Ȃ������t���[���ŃL�[��������Ă�����
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            oldPosition = transform.position; // ���݂̈ʒu��ۑ�����
            transform.position += arrowKeyInput; // �ړ�����
        }
        oldFrameKeyInput = ArrowKeyInput(); // ���t���[���̃L�[���͂�ۑ�
    }

    // ���L�[��������Ă��邩�ǂ�����Ԃ��֐�
    bool ArrowKeyInput()
    {
        // ���L�[�̂����ꂩ�������ꂽ�Ȃ�
        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.LeftArrow) || 
            Input.GetKey(KeyCode.UpArrow) || 
            Input.GetKey(KeyCode.DownArrow))
        {
            return true; // true
        }
        else // �����łȂ����
        {
            return false; // false
        }
    }

    // �Փˉ�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �R���W�������Փ˂�����A�ۑ����ꂽ�ʒu�ɖ߂�
        transform.position = oldPosition;
    }
}
