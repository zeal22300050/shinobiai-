using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // プレイヤーの状態
    private enum PlayerCondition
    {
        Wait,
        Move
    }
    private PlayerCondition playerCondition;

    [SerializeField]
    private Grid mapGrid; // マップグリッド
    [SerializeField]
    private GameDifficulityController difficulityController; // 移動回数上限取得用

    private int moveCount; // 移動した回数を保存する変数

    private Vector2 moveDistance; // 一度に移動する距離
    private Vector3 arrowKeyInput; // 矢印キーの入力を取得する変数

    private Vector3 oldPosition; // 前フレームの位置を保存する変数
    private bool oldFrameKeyInput; // 前フレームのキー入力を保存するフラグ

    // Start is called before the first frame update
    private void Start()
    {
        // 最初は初期位置を保存
        oldPosition = transform.position;
        // 一回の移動距離をセルサイズに合わせる
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    private void Update()
    {
        // なにもないときは待機状態にする
        playerCondition = PlayerCondition.Wait;
    }

    private void FixedUpdate()
    {
        // 移動回数が移動上限を超えていないなら
        if (moveCount < difficulityController.GetMoveLimit(0))
        {
            // プレイヤーの移動処理を行う
            PlayerMoveProcess();
        }
    }

    // プレイヤーの移動処理
    private void PlayerMoveProcess()
    {
        // 矢印キーの入力を取得
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // 前フレームでキーが押されていないかつ今フレームでキーが押されていたら
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            oldPosition = transform.position; // 現在の位置を保存する
            playerCondition = PlayerCondition.Move; // 移動状態に移行
            transform.position += arrowKeyInput; // 移動する
            moveCount++; // 移動回数を増やす
        }
        oldFrameKeyInput = ArrowKeyInput(); // 今フレームのキー入力情報を保存
    }

    // 矢印キーが押されているかどうかを返す関数
    private bool ArrowKeyInput()
    {
        // 矢印キーのいずれかが押されたなら
        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.LeftArrow) || 
            Input.GetKey(KeyCode.UpArrow) || 
            Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        }
        else // そうでなければ
        {
            return false;
        }
    }

    // 衝突応答処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // コリジョンが衝突したら、保存された位置に戻る
        transform.position = oldPosition;

        // 壁に当たって動けなかったときは移動回数を増やさない
        moveCount--;
    }

    // 現在の移動回数を返す関数
    public int GetCurrentMoveCount()
    {
        return moveCount;
    }

    // プレイヤーの状態を返す関数
    public int GetPlayerCondition()
    {
        return (int)playerCondition;
    }
}
