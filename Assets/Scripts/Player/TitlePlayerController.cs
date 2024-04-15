using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TitlePlayerController : AllCharacterController
{
    [SerializeField]
    protected FadeManager fade;

    private Animator animator;

    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    [HideInInspector]
    public enum PlayerState
    {
        /// <summary> 待機 </summary>
        stay,
        /// <summary> 移動 </summary>
        move,
        /// <summary> 勝利 </summary>
        win,
    };

    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    private PlayerState playerState;

    /// <summary>
    /// 状態処理の関数を入れる配列
    /// </summary>
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(PlayerState)).Length];

    //移動する速さ
    private float speed = 20.0f;

    // 現在の位置
    private Vector3 currentPosition;

    // 次の移動位置
    private Vector3 nextPosition;

    //一度に移動する距離
    private Vector3 moveDistance;

    //次のマスまでの距離
    private float titleDistance = 20.0f;

    // 回転後のy座標
    private float angle = 120.0f;

    // 現在の角度を保持
    private Vector3 currentRotation;

    // 次の回転の角度
    private Vector3 nextRotation;

    //今の回転位置
    private float nowRotation = 0.0f;

    // プレイヤーの見る方向
    private Vector3 lookDirection;

    //十字キーが押されているかの判定
    private bool gameDPad = false;

    //スティックに入力があるかの判定
    private bool gameStick = false;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(currentRotation.x, angle, currentRotation.z);
        // プレイヤーの状態を待機にする
        playerState = PlayerState.stay;

        SetStateFunction();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerAnimation();
    }

    private void FixedUpdate()
    {
        // プレイヤーの状態ごとの処理
        StateProcess(playerState);
        //Debug.Log(prsentLocation);
    }

    /// <summary>
    /// それぞれの状態に対応した関数を設定
    /// </summary>
    private void SetStateFunction()
    {
        stateFunc[(int)PlayerState.stay] = StayProcess;
        stateFunc[(int)PlayerState.move] = MoveProcess;
        stateFunc[(int)PlayerState.win] = WinProcess;
    }

    /// <summary>
    /// 受け取った状態に対応した関数を呼び出す
    /// </summary>
    /// <param name="state"> プレイヤーの状態 </param>
    private void StateProcess(PlayerState state)
    {
        stateFunc[(int)state]();
    }

    private void PlayerAnimation()
    {
        switch (playerState)
        {
            // プレイヤーの状態が待機の場合
            case PlayerState.stay:
                // アニメーションを切り替える
                animator.SetBool("State_Walk", false);
                break;

            // プレイヤーの状態が移動の場合
            case PlayerState.move:
                // アニメーションを切り替える
                animator.SetBool("State_Walk", true);
                break;

            case PlayerState.win:
                // アニメーションを切り替える
                animator.SetBool("State_StageClear", true);
                //animator.SetBool("State_Walk", false);
                break;
        }
    }

    private void StayProcess()
    {
        if (ArrowKeyDown())
        {
            StayToMoveState();
        }
    }

    public void MoveProcess()
    {
        CharacterMove(currentPosition, nextPosition,speed);

        lookDirection = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        if (positionRatio > 1)
        {
            playerState = PlayerState.stay;
            currentPosition = transform.position;
            CharacterRotate(currentRotation,new Vector3(currentRotation.x,180.0f,currentRotation.z));
            MoveParamReset();
        }
    }

    private void WinProcess()
    {
        Invoke(nameof(PlayerMove), 3.0f);
    }

    private void StayToMoveState()
    {
        if (gameDPad)
        {
            // ゲームパッドの十字キーの入力を取得する
            moveDistance = new Vector3((int)Input.GetAxisRaw("D_Pad_H"), 0.0f, 0.0f);
            Debug.Log("pad");
        }
        else if (gameStick)
        {
            // ゲームパッドの十字キーの入力を取得する
            moveDistance = new Vector3((int)Input.GetAxisRaw("L_Stick_H"), 0.0f, 0.0f);
            Debug.Log("stick");
        }
        // 次の移動位置を更新する
        nextPosition = currentPosition + (moveDistance * titleDistance);

        if (PlayerMoveLimit())
        {
            // プレイヤーの状態を移動にする
            playerState = PlayerState.move;
        }
    }

    protected void PlayerRotate()
    {
        // 回転させる
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(currentRotation), Quaternion.Euler(nextRotation), nowRotation);
    }

    private void PlayerMove()
    {
        playerState = PlayerState.move;
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
        // Y座標だけ下げる
        searchPosition.y = currentPosition.y - 0.5f;

        // レイキャストが他のコライダーに当たったら…
        if (Physics.Raycast(searchPosition, moveDistance, titleDistance))
        {
            Debug.Log("MoveTrue");
            // 真を返す
            return true;
        }
        else // 当たらなければ…
        {
            Debug.Log("MoveFalse");
            // 偽を返す
            return false;
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
        bool directionKeyInput = (int)Input.GetAxisRaw("D_Pad_H") != 0;//|| (int)Input.GetAxisRaw("D_Pad_V") != 0;
        bool joyStickInput = (int)Input.GetAxisRaw("L_Stick_H") != 0;//|| (int)Input.GetAxisRaw("L_Stick_V") != 0;
        // ジョイスティックまたは十字キーの入力を検知したら
        if (joyStickInput)
        {
            gameDPad = false;
            gameStick = true;
            // 真を返す
            return true;
        }
        else if (directionKeyInput)
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

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Start")
        {
            Debug.Log("Start");
            playerState = PlayerState.stay;
            nextPosition = new Vector3(60.0f, -17.5f, 40.0f);
            StartCoroutine(fade.FadeOutAndLoadScene());
            fade.fadeOutPanel.enabled = true;

        }
    }

}
