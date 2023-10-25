using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class GaugeController : MonoBehaviour
{
    // 各種コンポーネント
    [SerializeField]
    private Slider slider; 
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameController gameController;

    // スライダーの初期の大きさ
    private const float DefaultSliderValue = 1;

    private int moveLimitCount; // 移動回数上限
    private float decreaseGauge; // 一回の移動で減るゲージの量

    // Start is called before the first frame update
    void Start()
    {
        slider.value = DefaultSliderValue; // 初期状態ではゲージを満タンにしておく
        moveLimitCount = gameController.GetMoveLimit(StageName.Stage1); // 移動回数の上限を取得する
        decreaseGauge = DefaultSliderValue / (float)moveLimitCount; // 一回の移動で減るゲージの量を設定
    }

    // Update is called once per frame
    void Update()   
    {
        // プレイヤーの状態が1(移動状態)なら
        if (playerController.GetPlayerCondition() == 1)
        {
            slider.value = DefaultSliderValue - decreaseGauge * playerController.GetCurrentMoveCount(); // ゲージ(バリュー)を減らす
        }
    }
}
