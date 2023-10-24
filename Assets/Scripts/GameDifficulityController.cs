using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficulityController : MonoBehaviour
{
    /// <summary>
    /// ステージ名
    /// </summary>
    private enum StageName
    {
        Stage1,
        Stage2,
        Stage3,
    }

    /// <summary>
    /// ステージごとの移動回数上限を保存する変数
    /// </summary>
    private Dictionary<StageName, int> moveLimit = new Dictionary<StageName, int>(){
        {StageName.Stage1, 10},
        {StageName.Stage2, 8},
        {StageName.Stage3, 6},
    };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 受け取った番号に対応したステージの移動回数上限を取得する
    /// </summary>
    /// <param name="stageNumber"> ステージ番号 </param>
    /// <returns> 移動回数上限 </returns>
    public int GetMoveLimit(int stageNumber)
    {
        // 対応したステージ番号がない場合
        if (moveLimit.Count <= stageNumber || 0 > stageNumber)
        {
            return 0; // 0を返す
        }
        return moveLimit[(StageName)stageNumber]; // 移動回数上限を返す
    }
}
