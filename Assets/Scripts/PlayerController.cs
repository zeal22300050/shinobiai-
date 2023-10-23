using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Grid mapGrid; // マップグリッド

    private Vector2 moveDistance; // 一度に移動する距離
    private Vector3 arrowKeyInput; // 矢印キーの入力を取得する変数

    private Vector3 oldPosition; // 前フレームの位置を保存する変数
    private bool oldFrameKeyInput; // 前フレームのキー入力を保存するフラグ

    // Start is called before the first frame update
    void Start()
    {
        // 最初は初期位置を保存
        oldPosition = transform.position;
        // 一回の移動距離をセルサイズに合わせる
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 矢印キーの入力を取得
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // 前フレームでキーが押されていないかつ今フレームでキーが押されていたら
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            oldPosition = transform.position; // 現在の位置を保存する
            transform.position += arrowKeyInput; // 移動する
        }
        oldFrameKeyInput = ArrowKeyInput(); // 今フレームのキー入力を保存
    }

    // 矢印キーが押されているかどうかを返す関数
    bool ArrowKeyInput()
    {
        // 矢印キーのいずれかが押されたなら
        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.LeftArrow) || 
            Input.GetKey(KeyCode.UpArrow) || 
            Input.GetKey(KeyCode.DownArrow))
        {
            return true; // true
        }
        else // そうでなければ
        {
            return false; // false
        }
    }

    // 衝突応答処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // コリジョンが衝突したら、保存された位置に戻る
        transform.position = oldPosition;
    }
}
