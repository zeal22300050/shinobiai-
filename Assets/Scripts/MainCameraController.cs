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
        // 初期情報の設定
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
            ZoomIn(); // ズームイン
        }
        else
        {
            pos = playerPosition + initalPositionOffset; 
            pos.x = Mathf.Clamp(pos.x, MoveMinLimit.x, MoveMaxLimit.x);
            pos.z = Mathf.Clamp(pos.z, MoveMinLimit.z, MoveMaxLimit.z);
            trans.position = pos; // プレイヤーに追従する
        }
    }

    /// <summary>
    /// ズームイン処理
    /// </summary>
    private void ZoomIn()
    {
        // ズームインが終わったら処理をやめる
        if (zoomInRate > 1.0f)
        {
            return;
        }
        // デルタタイムを足し続ける
        moveTime += Time.deltaTime;

        // カメラを移動させる
        CameraTransLerp();
    }

    /// <summary>
    /// カメラを線形補間で移動・回転させる関数
    /// </summary>
    private void CameraTransLerp()
    {
        // 現在の位置とズーム後の移動位置の距離を求める
        distance = Vector3.Distance(initalPositionOffset, playerPosition + DefaultZoomOffset);
        // 移動の割合を求める
        zoomInRate = (moveTime * ZoomSpeed) / distance;

        // 位置と回転をズームイン後の状態に変化させる
        transform.position = Vector3.Lerp(trans.position, playerPosition + DefaultZoomOffset, zoomInRate);
        transform.rotation = Quaternion.Lerp(initalRotation, zoomInRotation, zoomInRate);
    }
}
