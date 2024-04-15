using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーに関する行動
/// </summary>
public class PlayerController : AllCharacterController
{
    private float ClearMotionTime { get { return 3.8f; } }
    private float StandingPosRatio { get { return 0.8f; } }
    private float RayPosY { get { return -0.3f; } }
    private Vector3 AfterExitPosition { get { return new Vector3(StageSpace * 3.0f, 0.0f, 0.5f); } }

    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    [HideInInspector]
    public enum PlayerState
    {
        /// <summary> 登場 </summary>
        appear,
        /// <summary> 待機 </summary>
        stay,
        /// <summary> 移動 </summary>
        move,
        /// <summary> 敗北 </summary>
        lose,
        /// <summary> 勝利 </summary>
        win,
        /// <summary> 退場 </summary>
        exit,
    };

    public PlayerState playerState { get; private set; }

    /// <summary>
    /// 状態処理の関数を入れる配列
    /// </summary>
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(PlayerState)).Length];

    private Animator animator;

    private AudioSource audioSource;

    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private Camera mainCameraObject;

    [SerializeField]
    private GameObject startObject;

    [SerializeField]
    private PlayerChangeMaterial changeMaterial;

    private Vector3 currentPosition;

    private Vector3 nextPosition;

    private Vector3 currentRotation;

    private Vector3 nextRotation;

    private Vector3 moveDistance;

    public Vector3 lookDirection { get; private set; }

    // 接触したオブジェクトの位置
    private Vector3 targetPosition;

    private Vector3 cameraPosition;

    public int currentTurnNum { get; private set; }

    private bool isHit;

    private bool oldKeyDown;

    private bool gameDPad;
    private bool gameStick;

    private string hitObjName;

    // Start is called before the first frame update
    private void Start()
    {
        // プレイヤーを登場状態にする
        playerState = PlayerState.appear;

        // 初期位置を代入する
        currentPosition = transform.position;
        nextPosition = startObject.transform.position;

        // パラメータの初期化
        MoveParamReset();
        SetStateFunction();

        // カプセルコライダーを取得する
        capsuleCollider = GetComponent<CapsuleCollider>();
        // アニメーターを取得する
        animator = GetComponent<Animator>();
        // オーディオソースを取得する
        audioSource = GetComponent<AudioSource>();

        hitObjName = null;

        // フラグをfalseにする
        isHit = false;

        gameDPad = false;
        gameStick = false;
    }

    private void FixedUpdate()
    {
        // プレイヤーの状態ごとの処理
        StateProcess(playerState);

        oldKeyDown = ArrowKeyDown();
    }

    /// <summary>
    /// それぞれの状態に対応した関数を設定
    /// </summary>
    private void SetStateFunction()
    {
        stateFunc[(int)PlayerState.appear] = AppearProcess;
        stateFunc[(int)PlayerState.stay] = StayProcess;
        stateFunc[(int)PlayerState.move] = MoveProcess;
        stateFunc[(int)PlayerState.lose] = LoseProcess;
        stateFunc[(int)PlayerState.win] = WinProcess;
        stateFunc[(int)PlayerState.exit] = ExitProcess;
    }

    /// <summary>
    /// 受け取った状態に対応した関数を呼び出す
    /// </summary>
    /// <param name="state"> プレイヤーの状態 </param>
    private void StateProcess(PlayerState state)
    {
        stateFunc[(int)state]();
    }

    /// <summary>
    /// 登場時処理
    /// </summary>
    private void AppearProcess()
    {
        // 歩いて入場
        animator.SetBool("State_Walk", true);
        CharacterMove(currentPosition, nextPosition, moveSpeed);

        // 移動が終わったら
        if (EndMove())
        {
            game.SetGameProgress(GameController.GameProgress.playing);
            // プレイヤーの状態を待機にする
            playerState = PlayerState.stay;
            // 現在の位置を保持する
            currentPosition = transform.position;
            // 線形補間のパラメータを初期化する
            MoveParamReset();
        }
    }

    /// <summary>
    /// 待機時処理
    /// </summary>
    private void StayProcess()
    {     
        // 勝利・敗北判定
        WinOrLose(isHit);

        capsuleCollider.enabled = true;
        // 待機モーションになる
        animator.SetBool("State_Walk", false);

        // ゲーム終了判定
        if (game.GameEnd())
        {
            return;
        }

        // 前フレームで移動ボタンが押されず、現フレームで押されたら…
        if (!oldKeyDown && ArrowKeyDown())
        {
            StayToMoveState(); // 状態をmoveに変更
        }
    }

    /// <summary>
    /// 移動時処理
    /// </summary>
    private void MoveProcess()
    {
        // ゲーム終了判定
        if (game.GameEnd())
        {
            return;
        }

        // 移動アニメーション再生
        animator.SetBool("State_Walk", true);

        // 勝利・敗北判定
        WinOrLose(isHit);

        // 移動処理
        CharacterMove(currentPosition, nextPosition, moveSpeed);

        // 進行方向に回転させる
        lookDirection = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // 移動が終わったら
        if (EndMove())
        {
            audioSource.Stop();
            // プレイヤーの状態を待機にする
            playerState = PlayerState.stay;
            // 現在の位置を保持する
            currentPosition = transform.position;
            // 線形補間のパラメータを初期化する
            MoveParamReset();
            game.TurnEnd();
        }
    }

    /// <summary>
    /// 敗北時処理
    /// </summary>
    private void LoseProcess()
    {
        ChangeHitRotate(transform.rotation, targetPosition, transform.position); // あたった敵の方を向く
        audioSource.Stop(); // BGMをストップ
        if (rotateSpeed >= 1.0f)
        {
            // プレイヤーの表情を変更
            if (changeMaterial.GetChangeMaterials() != PlayerChangeMaterial.ChangeMaterials.lose)
            {
                changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.surprise);
            }
            animator.SetBool("State_Findout", true); // 見つかった時のアニメーションに変更

            switch (hitObjName)
            {
                case "Rabit":
                    animator.SetBool("State_RabitFind", true); // うさぎに見つかったときのアニメーション
                    break;

                case "Dragon":
                    animator.SetBool("State_DragonFind", true); // 龍に見つかったときのアニメーション
                    break;
            }
        }
    }

    /// <summary>
    /// 勝利時処理
    /// </summary>
    private void WinProcess()
    {
        // プレイヤーの表情を笑顔に
        changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.smile);
        audioSource.Stop(); // BGMをストップ

        cameraPosition = mainCameraObject.transform.position;
        ChangeHitRotate(transform.rotation, cameraPosition, transform.position); // あたった敵の方を向く

        // 歩行モーションを停止
        animator.SetBool("State_Walk", false);

        // ステージクリアモーションを開始
        animator.SetBool("State_StageClear", true);

        // ステージクリアモーションを一定時間行ったら
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > ClearMotionTime)
        {
            animator.SetBool("State_StageClear", false); // ステージクリアモーションを終了
            changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.normal); // プレイヤーの表情を元に戻す
            
            // 退場時に使用する情報の設定
            currentPosition = transform.position;
            nextPosition = transform.position + AfterExitPosition;
            currentRotation = transform.rotation.eulerAngles;
            nextRotation = new Vector3(0.0f, 90.0f, 0.0f);
            MoveParamReset();
            game.SetGameProgress(GameController.GameProgress.result);
            playerState = PlayerState.exit;
        }
    }

    /// <summary>
    /// 退場時の処理
    /// </summary>
    private void ExitProcess()
    {
        // 歩行モーションを開始
        animator.SetBool("State_Walk", true);

        // 進行方向を向く処理が終わっていなかったら
        if (rotateParsent <= 1.0f)
        {
            CharacterRotate(currentRotation, nextRotation); // プレイヤーを回転させる
            return;
        }

        CharacterMove(currentPosition, nextPosition, MoveSpeed); // プレイヤーを移動させる
    }

    /// <summary>
    /// 待機状態から移動状態に移行
    /// </summary>
    private void StayToMoveState()
    {
        if (gameDPad)
        {
            // ゲームパッドの十字キーの入力を取得する
            moveDistance = new Vector3((int)Input.GetAxisRaw("D_Pad_H"), 0.0f, (int)Input.GetAxisRaw("D_Pad_V"));
        }
        else if (gameStick)
        {
            // ゲームパッドのスティック入力を取得する
            moveDistance = new Vector3((int)Input.GetAxisRaw("L_Stick_H"), 0.0f, (int)Input.GetAxisRaw("L_Stick_V"));
        }
        // 次の移動位置を更新する
        nextPosition = currentPosition + (moveDistance * StageSpace);

        // 移動先にステージオブジェクトがあったら…
        if (PlayerMoveLimit())
        {
            currentTurnNum++;
            // ターン経過を知らせる
            game.TurnStart();

            // プレイヤーの状態を移動にする
            playerState = PlayerState.move;
            audioSource.Play();
        }
    }

    /// <summary>
    /// 勝利か敗北か判定する
    /// </summary>
    /// <param name="hitFlag">
    /// オブジェクトに当たったかどうか
    /// </param>
    private void WinOrLose(bool hitFlag)
    {
        // hitフラグがfalseなら
        if (!hitFlag)
        {
            return; // 何もしない
        }
        // アニメーションを切り替える
        animator.SetBool("State_Walk", false);

        // 当たったオブジェクトの名前が
        switch (hitObjName)
        {
            // 兎の場合
            case "Rabit":
                // プレイヤーの状態を敗北にする
                playerState = PlayerState.lose;
                break;

            // ドラゴンの場合
            case "Dragon":
                if (NotMoving())
                {
                    // プレイヤーの状態を敗北にする
                    playerState = PlayerState.lose;
                }
                break;

            // ゴールの場合
            case "Goal":
                if (!(positionRatio < StandingPosRatio))
                {
                    // プレイヤーの状態を勝利にする
                    playerState = PlayerState.win;
                }
                break;
        }
    }

    /// <summary>
    /// 十字キーの入力を確認する関数
    /// </summary>
    /// <returns>
    /// 入力中 : true
    /// 未入力 : false
    /// </returns>
    private bool ArrowKeyDown()
    {
        bool joyStickInput = (int)Input.GetAxisRaw("L_Stick_H") != 0 || (int)Input.GetAxisRaw("L_Stick_V") != 0;
        bool directionKeyInput = (int)Input.GetAxisRaw("D_Pad_H") != 0 || (int)Input.GetAxisRaw("D_Pad_V") != 0;
        // ジョイスティックまたは十字キーの入力を検知したら
        if (joyStickInput)
        {
            gameDPad = false;
            gameStick = true;
            // 真を返す
            return true;
        }
        else if(directionKeyInput)
        {
            gameDPad = true;
            gameStick = false;
            // 真を返す
            return true;
        }
        else // そうでなければ
        {
            gameDPad = false;
            gameStick = false;
            // 偽を返す
            return false;
        }
    }

    /// <summary>
    /// プレイヤーの移動制限の設定
    /// </summary>
    /// <returns>
    /// 当たった : true
    /// 当たってない : false
    /// </returns>
    private bool PlayerMoveLimit()
    {
        // 探索する位置を決める
        Vector3 searchPosition = currentPosition;
        // Y座標だけ下にセットする
        searchPosition.y = RayPosY;

        // レイキャストで衝突したか調べる
        return Physics.Raycast(searchPosition, moveDistance, StageSpace);
    }

    /// <summary>
    /// 他のオブジェクトに当たったかを判定する
    /// </summary>
    /// <param name="other">
    /// オブジェクト情報
    /// </param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "StageSE")
        {
            return;
        }

        targetPosition = other.transform.position;

        // ぶつかったオブジェクトのタグを取得する
        hitObjName = other.tag;
        // 接触した判定をtrueにする
        isHit = true;
    }

    public bool PlayerStop()
    {
        return playerState == PlayerState.lose || playerState == PlayerState.win || playerState == PlayerState.exit;
    }

    public void EndDownAnimation()
    {
        changeMaterial.SetChangeMaterial(PlayerChangeMaterial.ChangeMaterials.lose);
        game.SetGameProgress(GameController.GameProgress.gameover);
    }

    public void EndHappyAnimation()
    {
        // ゲームの進行状態がゲームクリアじゃ無かったら…
        if (game.gameProgress != GameController.GameProgress.goal)
        {
            // ゲームの進行状態をゲームクリアにする
            game.SetGameProgress(GameController.GameProgress.goal);
        }
    }

    public void EndCloakAnimation()
    {
        game.TurnEnd();
        animator.SetBool("State_Item", false);
        playerState = PlayerState.stay;
    }
}
