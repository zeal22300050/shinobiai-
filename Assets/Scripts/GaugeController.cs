using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    // スライダーコンポーネント
    [SerializeField]
    private Slider slider; 

    // プレイヤーコントローラーコンポーネント
    [SerializeField]
    private PlayerController playerController; 

    private int moveLimitCount; // 移動回数上限
    private float decreaseGauge; // 一回の移動で減るゲージの量

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1; // 初期状態ではゲージを満タンにしておく
        moveLimitCount = 10; // 一旦10回移動したら動けなくしたい(後々変更)
        decreaseGauge = 1 / (float)moveLimitCount; // 一回の移動で減るゲージの量を設定
    }

    // Update is called once per frame
    void Update()   
    {
        // プレイヤーの状態が1(移動状態)なら
        if (playerController.GetPlayerCondition() == 1)
        {
            slider.value -= decreaseGauge; // ゲージ(バリュー)を減らす
        }
    }
}
