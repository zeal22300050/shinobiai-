using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    /// <summary>
    /// �X�v���C�g�X�V�p
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// �v���C���[�̏�Ԏ擾�p
    /// </summary>
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private GameObject zonePattern1;
    [SerializeField]
    private GameObject zonePattern2;

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g�̎擾
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // ���݂̃^�[�����������Ȃ�
        if (playerController.GetTurnNumber() % 2 == 0)
        {
            // �p�^�[��1�𖳌����A�p�^�[��2��L����
            zonePattern1.SetActive(false);
            zonePattern2.SetActive(true);

            spriteRenderer.flipX = true; // X������ɂ��Ĕ��]����
        }
        else // ��Ȃ�
        {
            // �p�^�[��1��L�����A�p�^�[��2�𖳌���
            zonePattern1.SetActive(true);
            zonePattern2.SetActive(false);

            spriteRenderer.flipX = false; // ���̂܂�
        }
    }
}
