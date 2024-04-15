using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public enum Status
    {
        Sleep,
        Rise,
        Find
    }
    private Status status;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private GameObject searchRangeDeco; // ���G�͈͂�������悤�ɂ���I�u�W�F�N�g

    private BoxCollider searchRange;

    private int sleepInterval; // �Q��Ԋu

    // Start is called before the first frame update
    private void Start()
    {
        searchRange = GetComponent<BoxCollider>(); // �R���|�[�l���g�̎擾

        // �e�X�g�p��2����
        sleepInterval = 2;
    }

    // Update is called once per frame
    private void Update()
    {   
        // �^�[�������A�Q�Ԋu�Ŋ�����0�Ȃ�
        if (player.currentTurnNum % sleepInterval == 0) 
        {
            status = Status.Sleep;
            searchRange.enabled = false; // ���G���Ȃ��Ȃ�
            searchRangeDeco.SetActive(false);
        }
        else // ����ȊO�Ȃ�
        {
            status = Status.Rise;
            searchRange.enabled = true; // ���G����
            searchRangeDeco.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�𔭌������ꍇ
        if (other.gameObject.tag == "Player")
        {
            status = Status.Find;
        }
    }

    public Status GetSheepStatus()
    {
        return status;
    }
}
