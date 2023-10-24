using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    // �X���C�_�[�R���|�[�l���g
    [SerializeField]
    private Slider slider; 

    // �v���C���[�R���g���[���[�R���|�[�l���g
    [SerializeField]
    private PlayerController playerController; 

    private int moveLimitCount; // �ړ��񐔏��
    private float decreaseGauge; // ���̈ړ��Ō���Q�[�W�̗�

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1; // ������Ԃł̓Q�[�W�𖞃^���ɂ��Ă���
        moveLimitCount = 10; // ��U10��ړ������瓮���Ȃ�������(��X�ύX)
        decreaseGauge = 1 / (float)moveLimitCount; // ���̈ړ��Ō���Q�[�W�̗ʂ�ݒ�
    }

    // Update is called once per frame
    void Update()   
    {
        // �v���C���[�̏�Ԃ�1(�ړ����)�Ȃ�
        if (playerController.GetPlayerCondition() == 1)
        {
            slider.value -= decreaseGauge; // �Q�[�W(�o�����[)�����炷
        }
    }
}
