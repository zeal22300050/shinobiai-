using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayerChangeMaterial : MonoBehaviour
{
    /// <summary>
    /// �v���C���[�̃}�e���A���̎��
    /// </summary>
    [HideInInspector]
    public enum ChangeMaterials
    {
        /// <summary> �ʏ� </summary>
        normal,
        /// <summary> �u�� </summary>
        closeEye,

        item,
    };

    /// <summary>
    /// �v���C���[�̃}�e���A���̎��
    /// </summary>
    private ChangeMaterials changeMaterials;

    // �ύX����}�e���A��
    [SerializeField]
    private Material[] materials;

    // �X�L�����b�V�������_���̎擾
    private SkinnedMeshRenderer smr;

    private int random;

    // Start is called before the first frame update
    void Start()
    {
        /// �v���C���[�̃}�e���A���̎�ނ�ʏ�ɂ���
        changeMaterials = ChangeMaterials.normal;

        // �ŏ��̃}�e���A����ݒ肷��
        smr = GetComponent<SkinnedMeshRenderer>();
        smr.material = materials[(int)changeMaterials];

        random = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // 0 ���� 99 �̊ԂŃ����_���Ȑ���������
        random = Random.Range(0, 100);

        // �}�e���A����ݒ肷��
        smr.material = materials[(int)changeMaterials];

        if (random == 0)
        {
            // �}�e���A����ύX����
            smr.material = materials[(int)ChangeMaterials.closeEye];
        }
    }
}
