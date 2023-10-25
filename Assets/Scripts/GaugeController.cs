using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    // �e��R���|�[�l���g
    [SerializeField]
    private Slider slider; 
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameDifficulityController difficulityController;

    // �X���C�_�[�̏����̑傫��
    private const float DefaultSliderValue = 1;

    private int moveLimitCount; // �ړ��񐔏��
    private float decreaseGauge; // ���̈ړ��Ō���Q�[�W�̗�

    // Start is called before the first frame update
    void Start()
    {
        slider.value = DefaultSliderValue; // ������Ԃł̓Q�[�W�𖞃^���ɂ��Ă���
        moveLimitCount = difficulityController.GetMoveLimit(0); // �ړ��񐔂̏�����擾����
        decreaseGauge = DefaultSliderValue / (float)moveLimitCount; // ���̈ړ��Ō���Q�[�W�̗ʂ�ݒ�
    }

    // Update is called once per frame
    void Update()   
    {
        // �v���C���[�̏�Ԃ�1(�ړ����)�Ȃ�
        if (playerController.GetPlayerCondition() == 1)
        {
            slider.value = DefaultSliderValue - decreaseGauge * playerController.GetCurrentMoveCount(); // �Q�[�W(�o�����[)�����炷
        }
    }
}
