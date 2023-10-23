using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Grid mapGrid; // �Z���T�C�Y�擾�p

    private Vector2 moveDistance; // ��x�Ɉړ����鋗��
    private Vector3 arrowKeyInput; // ���L�[�̓��͂��擾����ϐ�

    // Start is called before the first frame update
    void Start()
    {
        // ���̈ړ��������Z���T�C�Y�ɍ��킹��
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        // ���L�[�̓��͂��擾
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // �L�[�������ꂽ�u�ԂȂ�
        if (ArrowKeyDown())
        {
            transform.position += arrowKeyInput; // �ړ�����
        }
    }

    // ���L�[�������ꂽ�u�Ԃ��ǂ�����Ԃ��֐�
    bool ArrowKeyDown()
    {
        // ���L�[�̂����ꂩ�������ꂽ�u�ԂȂ�
        if (Input.GetKeyDown(KeyCode.RightArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            return true; // true
        }
        else // �����łȂ����
        {
            return false; // false
        }
    }
}
