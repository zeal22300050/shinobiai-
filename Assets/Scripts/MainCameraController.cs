using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private const float ZoomSpeed = 5.0f;
    private Vector3 DefaultZoomOffset { get { return new Vector3(0f, 2f, -5f); } }
    private Vector3 MoveMaxLimit { get { return new Vector3(13.5f, 0.0f, 2.0f); } }
    private Vector3 MoveMinLimit { get { return new Vector3(7.5f, 0.0f, -4.0f); } }

    private Transform trans;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GameObject player;
    private PlayerController plController;

    private Vector3 playerPosition;

    private Vector3 initalPositionOffset;

    private Quaternion initalRotation;
    private Quaternion zoomInRotation;

    private float distance;
    private float moveTime;
    private float zoomInRate;

    // Start is called before the first frame update
    void Start()
    {
        // �������̐ݒ�
        plController = player.GetComponent<PlayerController>();

        trans = transform;
        initalPositionOffset = trans.position - player.transform.position;

        initalRotation = trans.localRotation;
        zoomInRotation = new Quaternion(0, 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;
        Vector3 pos;

        if (plController.PlayerStop())
        {
            ZoomIn(); // �Y�[���C��
        }
        else
        {
            pos = playerPosition + initalPositionOffset; 
            pos.x = Mathf.Clamp(pos.x, MoveMinLimit.x, MoveMaxLimit.x);
            pos.z = Mathf.Clamp(pos.z, MoveMinLimit.z, MoveMaxLimit.z);
            trans.position = pos; // �v���C���[�ɒǏ]����
        }
    }

    /// <summary>
    /// �Y�[���C������
    /// </summary>
    private void ZoomIn()
    {
        // �Y�[���C�����I������珈������߂�
        if (zoomInRate > 1.0f)
        {
            return;
        }
        // �f���^�^�C���𑫂�������
        moveTime += Time.deltaTime;

        // �J�������ړ�������
        CameraTransLerp();
    }

    /// <summary>
    /// �J��������`��Ԃňړ��E��]������֐�
    /// </summary>
    private void CameraTransLerp()
    {
        // ���݂̈ʒu�ƃY�[����̈ړ��ʒu�̋��������߂�
        distance = Vector3.Distance(initalPositionOffset, playerPosition + DefaultZoomOffset);
        // �ړ��̊��������߂�
        zoomInRate = (moveTime * ZoomSpeed) / distance;

        // �ʒu�Ɖ�]���Y�[���C����̏�Ԃɕω�������
        transform.position = Vector3.Lerp(trans.position, playerPosition + DefaultZoomOffset, zoomInRate);
        transform.rotation = Quaternion.Lerp(initalRotation, zoomInRotation, zoomInRate);
    }
}
