using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のウサギに関する行動
/// </summary>
public class RabitController : AllCharacterController
{
    /// <summary>
    /// ウサギが移動する方向
    /// </summary>
    private enum RabitMoveDirection
    {
        /// <summary> 前方向 </summary>
        up,
        /// <summary> 後方向 </summary>
        down,
        /// <summary> 右方向 </summary>
        right,
        /// <summary> 左方向 </summary>
        left
    };

    /// <summary>
    /// ウサギが移動する方向
    /// </summary>
    [SerializeField]
    private RabitMoveDirection rabitMove;

    /// <summary>
    /// ウサギのアニメーション状況
    /// </summary>
    private enum RabitAnimaState
    {
        /// <summary> 待機 </summary>
        idel,
        /// <summary> 移動 </summary>
        jump,
        /// <summary> 頭突き </summary>
        attack,
    };

    /// <summary>
    /// ウサギのアニメーション状況
    /// </summary>
    private RabitAnimaState rabitAnima;

    private Animator animator;

    private Transform trans;

    [SerializeField]
    private GameObject searchArea;

    /// <summary>
    /// 衝突するレイヤーの名前
    /// </summary>
    [SerializeField]
    private LayerMask layerMask;

    /// <summary>
    /// 進行方向に足場があるか探索する位置
    /// </summary>
    private Vector3 searchPosition;

    // 現在の位置
    private Vector3 currentPosition;

    // 次の移動位置
    private Vector3 nextPosition;

    // 次の索敵位置
    private Vector3 nextSearchPosition;

    private Vector3 localAngle;

    /// <summary>
    /// ステージが続いているか調べる方向
    /// </summary>
    private Vector3[] distances =
    {
        new Vector3(0.0f,0.0f,+1.0f), // 後
        new Vector3(0.0f,0.0f,-1.0f), // 前
        new Vector3(+1.0f,0.0f,0.0f), // 右
        new Vector3(-1.0f,0.0f,0.0f), // 左
    };

    // 次の向き
    private float nextRotation;

    private bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        // 現在の位置を取得する
        currentPosition = trans.position;

        NextPositionParam();

        localAngle = trans.localEulerAngles;

        // サーチエリアの生成
        searchArea = Instantiate(searchArea, nextSearchPosition, Quaternion.identity);

        rabitAnima = RabitAnimaState.idel;

        animator = GetComponent<Animator>();

        isHit = false;
    }

    private void FixedUpdate()
    {
        RabitAnimation(); // アニメーション処理

        // 終了しているかつターンが終わっていないもしくは動いていなかったら
        if (GameEnd() && (game.isChengeTurn || NotMoving()))
        {
            Destroy(searchArea); // 索敵範囲を削除する
            ChangeHitRotate(trans.rotation, player.transform.position, currentPosition); // プレイヤーの方を向く

            if (!isHit)
            {
                rabitAnima = RabitAnimaState.idel;
            }
            return;
        }

        MoveProcess();
    }

    /// <summary>
    /// アニメーションの変更
    /// </summary>
    private void RabitAnimation()
    {
        switch(rabitAnima)
        {
            case RabitAnimaState.idel:
                animator.SetBool("State_Jump", false);
                animator.SetBool("State_Headbutt", false);
                break;

            case RabitAnimaState.jump:
                animator.SetBool("State_Jump", true);
                break;

            case RabitAnimaState.attack:
                animator.SetBool("State_Headbutt", true);
                break;
        }
    }

    private void MoveProcess()
    {
        // 攻撃状態でないなら
        if (rabitAnima != RabitAnimaState.attack)
        {
            MoveLimit(); // 移動制限処理
            RabitAngle(); // 回転処理
        }

        // プレイヤーのターン中なら
        if (game.isChengeTurn)
        {
            rabitAnima = RabitAnimaState.jump; // ジャンプ状態に遷移
            CharacterMove(currentPosition, nextPosition, MoveSpeed); // 移動する

            // 移動が終わったら
            if (EndMove())
            {
                rabitAnima = RabitAnimaState.idel; // 待機状態に遷移
            }

        }
        else // そうでないなら
        {
            NextPositionParam(); // 次の移動先座標を設定

            // 移動が終わったら
            if (EndMove())
            {
                MoveParamReset(); // パラメータをリセット
            }

            rabitAnima = RabitAnimaState.idel; // 待機状態に遷移
        }

    }

    private void RabitAngle()
    {
        switch (rabitMove)
        {
            // 兎の移動方向が前の場合
            case RabitMoveDirection.up:
                nextRotation = 0.0f;
                break;

            // 兎の移動方向が後の場合
            case RabitMoveDirection.down:
                nextRotation = 180.0f;
                break;

            // 兎の移動方向が右の場合
            case RabitMoveDirection.right:
                nextRotation = 90.0f;
                break;

            // 兎の移動方向が左の場合
            case RabitMoveDirection.left:
                nextRotation = -90.0f;
                break;
        }
        localAngle.y = nextRotation;
        trans.localEulerAngles = localAngle;

    }

    /// <summary>
    /// 現在地を更新するための関数
    /// </summary>
    private void NextPositionParam()
    {
        // 現在の位置を取得する
        currentPosition = trans.position;

        // 次の位置を現在の位置に設定する
        nextPosition = currentPosition;

        switch(rabitMove)
        {
            // 兎の移動方向が上の場合
            case RabitMoveDirection.up:
                nextPosition.z += StageSpace;
                break;

            // 兎の移動方向が下の場合
            case RabitMoveDirection.down:
                nextPosition.z -= StageSpace;
                break;

            // 兎の移動方向が右の場合
            case RabitMoveDirection.right:
                nextPosition.x += StageSpace;
                break;

            // 兎の移動方向が左の場合
            case RabitMoveDirection.left:
                nextPosition.x -= StageSpace;
                break;
        }

        nextSearchPosition = new Vector3(nextPosition.x, 0.0f, nextPosition.z);

        if (searchArea)
        {
            searchArea.transform.position = nextSearchPosition;
        }
    }

    private void MoveLimit()
    {
        // 探索を開始する位置を設定する
        searchPosition = currentPosition;
        // 探索するY座標だけ下げる
        searchPosition.y = -0.3f;

        int a = (int)rabitMove;

        if (!Physics.Raycast(searchPosition, distances[a], StageSpace, layerMask))
        {
            switch (rabitMove)
            {
                // 兎の移動方向が前の場合
                case RabitMoveDirection.up:
                    rabitMove = RabitMoveDirection.down;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;

                // 兎の移動方向が後の場合
                case RabitMoveDirection.down:
                    rabitMove = RabitMoveDirection.up;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;

                // 兎の移動方向が右の場合
                case RabitMoveDirection.right:
                    rabitMove = RabitMoveDirection.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;

                // 兎の移動方向が左の場合
                case RabitMoveDirection.left:
                    rabitMove = RabitMoveDirection.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rabitAnima = RabitAnimaState.attack;
            isHit = true;
        }
    }
}
