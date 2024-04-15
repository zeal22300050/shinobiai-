using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のドラゴンに関する行動
/// </summary>
public class DragonController : AllCharacterController
{
    private enum DragonMoveState
    {
        stay,
        rotate
    };

    private DragonMoveState dragonMove;

    // 生成する索敵範囲
    [SerializeField]
    private GameObject searchArea;

    private Animator animator;

    private enum DragonAnimationState
    {
        /// <summary> 待機 </summary>
        idel,
        /// <summary> 向きを変える </summary>
        turn,
        /// <summary> ブレスを吐く </summary>
        breath,
    };

    private DragonAnimationState dragonAnimaState;

    [SerializeField]
    // 1回の回転量
    private float angle = 90.0f;

    [SerializeField]
    private GameObject breathEffect;

    // 次の回転量
    private float nextAngle;

    // 次の回転の角度
    private Vector3 nextRotation;

    // 現在の角度を保持
    private Vector3 currentRotation;

    // パラメータを再設定するかのフラグ
    private bool isChange = false;

    private string turnDirection;

    // ボックスコライダー
    private BoxCollider boxCollider;

    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {  
        // 最初の角度を設定する
        currentRotation = transform.rotation.eulerAngles;
        nextRotation = new Vector3(currentRotation.x, currentRotation.y + angle, currentRotation.z);
        nextAngle = currentRotation.y + angle;

        // 角度を代入する
        transform.rotation = Quaternion.Euler(currentRotation);

        // 右回りか左回りか判別する
        if(angle > 0.0f)
        {
            turnDirection = "turnRight";
        }
        else if(angle < 0.0f)
        {
            turnDirection = "turnLeft";
        }

        dragonMove = DragonMoveState.stay;
        dragonAnimaState = DragonAnimationState.idel;

        // サーチエリアの生成
        searchArea = Instantiate(searchArea, new Vector3(transform.position.x, 0.0f, transform.position.z), Quaternion.Euler(nextRotation));
        animator = GetComponent<Animator>();

        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        bool endTerms = player.playerState == PlayerController.PlayerState.lose || player.playerState == PlayerController.PlayerState.win;

        if (endTerms && game.isChengeTurn)
        {
            Destroy(searchArea);
        }

        DragonAnimation();

        if (endTerms)
        {
            animator.SetBool("State_Stay", true);
            return;
        }

        dragonMove = ChangeDragonMoveState();

        DragonMove();

        ChangeAnimaState();
    }

    /// <summary>
    /// ドラゴンのアニメーション
    /// </summary>
    private void DragonAnimation()
    {
        switch (dragonAnimaState)
        {
            // ドラゴンの状態が待機の場合
            case DragonAnimationState.idel:
                animator.SetBool("State_TurnR", false);
                animator.SetBool("State_TurnL", false);
                break;

            case DragonAnimationState.turn:
                switch (turnDirection)
                {
                    case "turnRight":
                        animator.SetBool("State_TurnR", true);
                        breathEffect.SetActive(false);
                        break;

                    case "turnLeft":
                        animator.SetBool("State_TurnL", true);
                        breathEffect.SetActive(false);
                        break;
                }
                break;

            case DragonAnimationState.breath:
                animator.SetBool("State_Breath", true);
                breathEffect.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// アニメーションの遷移処理
    /// </summary>
    private void ChangeAnimaState()
    {
        if (rotateParsent != 0.0f)
        {
            // アニメーションを向きを変えるにする
            dragonAnimaState = DragonAnimationState.turn;
        }
        if (rotateParsent == 0.0f)
        {
            // アニメーションを首を傾げるにする
            dragonAnimaState = DragonAnimationState.idel;
        }

        if(isHit)
        {
            dragonAnimaState = DragonAnimationState.breath;
        }
    }

    /// <summary>
    /// 次の回転に必要なパラメータを設定する関数
    /// </summary>
    private void NextRotationParam()
    {
        // 現在の角度を更新する
        currentRotation = nextRotation;

        // 次の角度を更新する
        nextAngle += angle;

        // 次の回転の角度を設定する
        nextRotation.y = nextAngle;

        searchArea.transform.rotation = Quaternion.Euler(nextRotation);

        // 一回に回転する量が180度かつ次の角度が360以上だったら…
        if (nextAngle >= 360.0f)
        {
            // それぞれのパラメータから-360する
            nextAngle -= 360.0f;
            nextRotation.y -= 360.0f;
            currentRotation.y -= 360.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isHit = true;
        }
    }

    private DragonMoveState ChangeDragonMoveState()
    {
        if(game.isChengeTurn)
        {
            return DragonMoveState.rotate;
        }
        else
        {
            return DragonMoveState.stay;
        }
    }

    /// <summary>
    /// ドラゴンの行動処理
    /// </summary>
    private void DragonMove()
    {
        switch (dragonMove)
        {
            case DragonMoveState.stay:
                boxCollider.enabled = true;
                // フラグがtrueだったら…
                if (isChange)
                {
                    MoveParamReset();
                    // パラメータを再設定する
                    NextRotationParam();
                    // フラグをfalseにする
                    isChange = false;
                }
                break;

            case DragonMoveState.rotate:
                boxCollider.enabled = false;
                // ドラゴンを回転させる
                CharacterRotate(currentRotation, nextRotation);
                // フラグをtrueにする
                isChange = true;
                break;
        }
    }
    public void IdelBreathStart()
    {
        breathEffect.SetActive(true);
    }

    public void IdelBreathEnd()
    {
        breathEffect.SetActive(false);
    }
}
