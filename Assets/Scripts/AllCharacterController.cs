using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの移動・回転を管理するスクリプト
/// </summary>

public class AllCharacterController : MonoBehaviour
{
    protected float StageSpace { get { return 3.0f; } }

    protected float MoveSpeed { get { return 5.0f; } }

    [SerializeField]
    protected GameController game;
   
    [SerializeField]
    protected PlayerController player;

    /// <summary>
    /// 現在の位置と次の移動位置の距離
    /// </summary>
    protected float distance;
    /// <summary>
    /// 現在の位置の割合
    /// </summary>
    protected float positionRatio;

    protected float moveTime;

    protected float rotateTime;
    protected float rotateSpeed;
    protected float rotateParsent;

    protected float moveSpeed = 5.0f;

    /// <summary>
    /// キャラクターの移動処理をする関数
    /// </summary>
    /// <param name="currentPos"> 現在の位置 </param>
    /// <param name="nextPos"> 移動先の位置 </param>
    /// <param name="speed"> 移動速度 </param>
    protected void CharacterMove(Vector3 currentPos, Vector3 nextPos,float speed)
    {
        // 現在の位置と次の移動位置の距離を求める
        distance = Vector3.Distance(currentPos, nextPos);
        // デルタタイムを足し続ける
        moveTime += Time.deltaTime;
        // 移動の割合を求める
        positionRatio = moveTime * speed / distance;

        // 位置を更新する
        transform.position = Vector3.Lerp(currentPos, nextPos, positionRatio);
    }

    /// <summary>
    /// キャラクターの回転処理をする関数
    /// </summary>
    /// <param name="currentRot"> 現在の向き </param>
    /// <param name="nextRot"> 次の向き </param>
    /// <param name="par"> 回転の割合 </param>
    protected void CharacterRotate(Vector3 currentRot, Vector3 nextRot)
    {
        distance = Vector3.Distance(new Vector3(0.0f,0.0f,0.0f), new Vector3(3.0f,0.0f,0.0f));

        rotateTime += Time.deltaTime;

        rotateParsent = (rotateTime * 5.0f) / distance;

        // 回転させる
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(currentRot), Quaternion.Euler(nextRot), rotateParsent);
    }

    /// <summary>
    /// 他キャラと当たった時の回転処理
    /// </summary>
    /// <param name="rotation"> 当たった瞬間の角度 </param>
    /// <param name="targetPos"> 他キャラの位置 </param>
    /// <param name="myPos"> 自分の位置 </param>
    protected void ChangeHitRotate(Quaternion rotation, Vector3 targetPos, Vector3 myPos)
    {
        // 回転する速さ
        rotateSpeed += Time.deltaTime / 2.0f;

        Quaternion currentRotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(targetPos - myPos), rotateSpeed);
        currentRotation.x = 0.0f;
        currentRotation.z = 0.0f;

        transform.rotation = currentRotation;
    }


    /// <summary>
    /// 移動に関するパラメータをリセットする関数
    /// </summary>
    protected void MoveParamReset()
    {
        distance = 0.0f;
        rotateParsent = 0.0f;
        positionRatio = 0.0f;
        moveTime = 0.0f;
        rotateTime = 0.0f;
    }

    protected bool NotMoving()
    {
        return positionRatio <= 0.0f;
    }

    protected bool EndMove()
    {
        return positionRatio > 1.0f;
    }

    /// <summary>
    /// ゲーム終了判定
    /// </summary>
    /// <returns>
    /// プレイヤーが負けor勝利時にtrueを返す
    /// </returns>
    protected bool GameEnd()
    {
        return player.playerState == PlayerController.PlayerState.lose || player.playerState == PlayerController.PlayerState.win;
    }
}
