using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// ステージ名
    /// </summary>
    public enum StageName
    {
        Stage1,
        Stage2,
        Stage3,
    }
    /// <summary>
    /// ゲームの進行状況
    /// </summary>
    public enum GameProgress
    {
        Start,
        InGame,
        Goal,
        GameOver
    }
    /// <summary>
    /// ステージ名
    /// </summary>
    private StageName stageName;

    /// <summary>
    /// ゲームの進行状況
    /// </summary>
    private GameProgress gameProgress;

    /// <summary>
    /// ステージごとの移動回数上限を保存する変数
    /// </summary>
    private readonly Dictionary<StageName, int> moveLimit = new(){
        {StageName.Stage1, 10},
        {StageName.Stage2, 8},
        {StageName.Stage3, 6},
    };

    private void Start()
    {
        // 最初はゲーム中状態
        gameProgress = GameProgress.Start;
    }

    /// <summary>
    /// 進行状況をゲーム中にする時に呼ばれる関数
    /// </summary>
    public void StartInGame()
    {
        // 進行状況をゲーム中に戻す
        gameProgress = GameProgress.InGame;
    }

    /// <summary>
    ///  プレイヤーがゴールした時に呼び出される関数
    /// </summary>
    public void Goal()
    {
        //ゲームの進行状況をゴールにする
        gameProgress = GameProgress.Goal;
    }

    public void GameOver()
    {
        gameProgress = GameProgress.GameOver;
    }

    /// <summary>
    /// ゲームの進行状況を取得する関数
    /// </summary>
    /// <returns> ゲーム進行状況 </returns>
    public GameProgress GetGameProgress()
    {
        return gameProgress;
    }

    // ステージ名を取得する関数
    public StageName GetStageName()
    {
        return stageName;
    }

    /// <summary>
    /// 受け取った番号に対応したステージの移動回数上限を取得する
    /// </summary>
    /// <param name="stageNames"> ステージ名 </param>
    /// <returns> 移動回数上限 </returns>
    public int GetMoveLimit(StageName stageNames)
    {
        return moveLimit[stageNames]; // 移動回数上限を返す
    }
}
