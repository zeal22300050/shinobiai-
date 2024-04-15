using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrihimeController : AllCharacterController
{
    /// <summary>
    /// 織姫の行動ステート
    /// </summary>
    private enum OrihimeState
    {
        /// <summary> 待機 </summary>
        stay,
        /// <summary> 勝利 </summary>
        win,
        /// <summary> 敗北 </summary>
        lose,
        /// <summary> 退場 </summary>
        exit
    };

    /// <summary>
    /// 織姫の行動ステート
    /// </summary>
    private OrihimeState orihimeState;
    
    // 織姫の行動ごとの処理を入れる配列
    private Action[] stateFunc = new Action[Enum.GetValues(typeof(OrihimeState)).Length];

    // 現在の位置
    private Vector3 currentPosition;

    private Vector3 nextPosition;

    // 現在の向き
    private Vector3 currentRotation;

    // 次の向き
    private Vector3 nextRotation;

    // アニメーター取得
    private Animator animator;

    [SerializeField]
    private PlayerController playerController;

    private int oldState;

    private bool moveFlag;

    // Start is called before the first frame update
    void Start()
    {
        // 行動ステートを待機にする
        orihimeState = OrihimeState.stay;

        // アニメーターコンポーネントを取得する
        animator = GetComponent<Animator>();

        // 初期化
        SetOrihimeStateFunctions();

        // 現在の向き
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(0.0f, 180.0f, 0.0f);

        oldState = (int)playerController.playerState;

        moveFlag = false;
    }

    private void FixedUpdate()
    {
        if (oldState != ChangeAction())
        {
            // 織姫の行動呼び出し
            ActionFunc(orihimeState);
        }
    }

    /// <summary>
    /// 状態に対応した関数を設定
    /// </summary>
    private void SetOrihimeStateFunctions()
    {
        stateFunc[(int)OrihimeState.stay] = StayAciton;
        stateFunc[(int)OrihimeState.win] = WinAction;
        stateFunc[(int)OrihimeState.lose] = LoseAction;
        stateFunc[(int)OrihimeState.exit] = ExitAction;
    }

    /// <summary>
    /// 織姫の状態に対応した関数を呼び出す
    /// </summary>
    /// <param name="state">
    /// 織姫の状態
    /// </param>
    private void ActionFunc(OrihimeState state)
    {
        stateFunc[(int)state]();
    }

    /// <summary>
    /// 待機時の処理
    /// </summary>
    private void StayAciton()
    {
    }

    /// <summary>
    /// 勝利時の処理
    /// </summary>
    private void WinAction()
    {
        CharacterRotate(currentRotation, nextRotation);
        animator.SetBool("Win", true);
    }

    /// <summary>
    /// 敗北時の処理
    /// </summary>
    private void LoseAction()
    {
        animator.SetBool("Lose", true);
    }

    /// <summary>
    /// 退場時の処理
    /// </summary>
    private void ExitAction()
    {
        if (!moveFlag)
        {
            MoveParamReset();
            animator.SetBool("Win", false);

            currentRotation = transform.rotation.eulerAngles;
            nextRotation = new Vector3(0.0f, 90.0f, 0.0f);

            currentPosition = transform.position;
            nextPosition = transform.position + new Vector3(3.0f * 3.0f, 0.0f, 0.0f);

            moveFlag = true;
        }

        if(rotateParsent <= 1.0f)
        {
            CharacterRotate(currentRotation, nextRotation);
            return;
        }

        CharacterMove(currentPosition, nextPosition, MoveSpeed);
    }

    /// <summary>
    /// 行動を変えるための処理
    /// </summary>
    private int ChangeAction()
    {
        switch (playerController.playerState)
        {
            case PlayerController.PlayerState.lose:
                orihimeState = OrihimeState.lose;
                break;

            case PlayerController.PlayerState.win:
                orihimeState = OrihimeState.win;
                break; ;

            case PlayerController.PlayerState.exit:
                orihimeState = OrihimeState.exit;
                break;
        }

        return (int)playerController.playerState;
    }
}
