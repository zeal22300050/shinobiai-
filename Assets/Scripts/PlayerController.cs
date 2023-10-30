using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    private enum PlayerCondition
    {
        Wait,
        Move
    }
    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    private PlayerCondition playerCondition;

    /// <summary>
    /// 比較結果
    /// </summary>
    private enum CompareResult
    {
        Defferrent,
        Same
    }
    /// <summary>
    /// マップグリッドの大きさ取得用
    /// </summary>
    [SerializeField]
    private Grid mapGrid;
    /// <summary>
    /// 残り手数表示用
    /// </summary>
    [SerializeField]
    private Slider slider;
    /// <summary>
    /// ステージ情報取得用
    /// </summary>
    [SerializeField]
    private GameController gameController; 
    /// <summary>
    /// 足場表示用スプライトマスク
    /// </summary>
    [SerializeField]
    private GameObject spriteMask;

    private const float DefaultSliderValue = 1.0f; // スライダーの初期量

    private readonly List<GameObject> maskObject = new(); // スプライトマスクを保存しておく配列

    private Vector3 defaultPosition; // 初期位置
    private Vector2 moveDistance; // 一度に移動する距離
    private Vector3 arrowKeyInput; // 矢印キーの入力を取得する変数

    private int moveCount; // 移動した回数を保存する変数
    private float decreaseGauge; // 一回の移動で減るゲージの量

    private readonly List<Vector3> oldPosition = new(); // 過去の位置を保存する配列
    private int oldPositionIndex; // 任意の保存済み位置にアクセスするための変数
    private bool oldFrameKeyInput; // 前フレームのキー入力を保存するフラグ

    private int turnNumber = 1; // ターン数(初期値は1)

    // Start is called before the first frame update
    private void Start()
    {
        slider.value = DefaultSliderValue;// 初期状態ではゲージを満タンにしておく
        decreaseGauge = DefaultSliderValue / (float)gameController.GetMoveLimit(StageName.Stage1); // 一回の移動で減るゲージ量を計算
        defaultPosition = transform.position; // 初期位置の設定(後で位置取得などに変えたい)
        // 一回の移動距離をセルサイズに合わせる
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    private void Update()
    {
        // ゴールしたら
        if (gameController.GetGameProgress() == GameProgress.Goal)
        {
            return; // これ以降の処理を行わない
        }

        // リセットボタンが押されたら
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProcess(); // リセット処理を行う
        }
    }

    private void FixedUpdate()
    {
        // ゴールしたら
        if (gameController.GetGameProgress() == GameProgress.Goal)
        {
            return; // これ以降の処理を行わない
        }

        // 移動回数が移動上限を超えていなければ
        if (moveCount < gameController.GetMoveLimit(StageName.Stage1))
        {
            // プレイヤーの移動処理を行う
            PlayerMoveProcess();
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMoveProcess()
    {
        // 矢印キーの入力を取得
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // 前フレームでキーが押されていないかつ今フレームでキーが押されていたら
        if (oldFrameKeyInput == false && ArrowKeyInput())
        {
            playerCondition = PlayerCondition.Move; // 移動状態に移行

            oldPosition.Add(transform.position); // 現在の位置を保存する
            transform.position += arrowKeyInput; // 移動する

            turnNumber++; // ターン数を増やす

            // 現在位置と保存済み位置が一致しなかったなら
            if (ComparePosition() == CompareResult.Defferrent)
            {
                moveCount++; // 移動回数を増やす
                slider.value = DefaultSliderValue - decreaseGauge * moveCount; // ゲージを減らす
                maskObject.Add(Instantiate(spriteMask, transform.position, Quaternion.identity)); // 足場を表示する
            }
        }
        else
        {
            // なにもないときは待機状態にする
            playerCondition = PlayerCondition.Wait;
        }
        oldFrameKeyInput = ArrowKeyInput(); // 今フレームのキー入力情報を保存

    }

    /// <summary>
    /// 矢印キーの入力を確認する関数
    /// </summary>
    /// <returns> 
    /// 入力中 : true 
    /// 未入力 : false
    /// </returns>
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

    /// <summary>
    /// 現在の位置と保存済みの位置を比べて一致か不一致かを返す関数
    /// </summary>
    /// <returns>
    /// 何も保存されていないか保存済み位置と現在位置が不一致だった : Defferrent
    /// 保存済み位置と現在位置が一致した : Same
    /// </returns>
    private CompareResult ComparePosition()
    {
        // oldPositionに何も保存されていないなら
        if (oldPosition == null)
        {
            // Defferrentを返す
            return CompareResult.Defferrent;
        }

        // oldPositionの配列サイズまでループする
        for (oldPositionIndex = 0; oldPositionIndex < oldPosition.Count; oldPositionIndex++)
        {
            // 現在位置が保存済の位置と一致していたら
            if (transform.position == oldPosition[oldPositionIndex])
            {
                // Sameを返す
                return CompareResult.Same;
            }
        }       

        // 上記の条件に当たらなければDefferrentを返す
        return CompareResult.Defferrent;
    } 

    /// <summary>
    /// リセット時の処理
    /// </summary>
    private void ResetProcess()
    {
        // 位置の初期化
        transform.position = defaultPosition;
        // 移動回数の初期化
        moveCount = 0;
        // ゲージの初期化
        slider.value = DefaultSliderValue;
        // 配列がNULLならこれ以降の処理を行わない
        if (oldPosition == null || maskObject == null)
        {
            return;
        }
        // maskオブジェクトをすべて削除する
        for (int i = 0; i < maskObject.Count; i++)
        {
            Destroy(maskObject[i]);
        }
        // 保存された位置を削除する
        oldPosition.Clear();
        // 保存されたオブジェクト情報を削除する
        maskObject.Clear();
        // ゲーム進行状況をゲームスタート時に戻す
        gameController.StartInGame();
    }

    // 衝突応答処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 一手前に保存された位置に戻る
        transform.position = oldPosition[--oldPositionIndex];

        // 壁に当たって動けなかったときは移動回数とターン数を増やさない
        moveCount--;
        turnNumber--;

        // ゲージに変更を適用する
        slider.value = DefaultSliderValue - decreaseGauge * moveCount; 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            // ゴール関数を呼び出す
            gameController.Goal();
        }
    }

    /// <summary>
    /// 現在の移動回数を返す関数
    /// </summary>
    /// <returns>
    /// 移動回数
    /// </returns>
    public int GetCurrentMoveCount()
    {
        return moveCount;
    }

    /// <summary>
    /// プレイヤーの状態を返す関数
    /// </summary>
    /// <returns>
    /// 現在のプレイヤーの状態
    /// </returns>
    public int GetPlayerCondition()
    {
        return (int)playerCondition;
    }

    /// <summary>
    /// 現在のターン数を返す関数
    /// </summary>
    /// <returns>現在のターン数</returns>
    public int GetTurnNumber()
    {
        return turnNumber;
    }
}
