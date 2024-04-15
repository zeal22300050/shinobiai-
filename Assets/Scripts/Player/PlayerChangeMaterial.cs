using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeMaterial : MonoBehaviour
{
    /// <summary>
    /// プレイヤーのマテリアルの種類
    /// </summary>
    [HideInInspector]
    public enum ChangeMaterials
    {    
        /// <summary> 通常 </summary>
        normal,
        /// <summary> 瞬き </summary>
        closeEye,
        /// <summary> 負け </summary>
        lose,
        /// <summary> 勝利 </summary>
        smile,
        /// <summary> 驚き </summary>
        surprise,

        item,
    };

    /// <summary>
    /// プレイヤーのマテリアルの種類
    /// </summary>
    private ChangeMaterials changeMaterials;

    // 変更するマテリアル
    [SerializeField]
    private Material[] materials;

    // スキンメッシュレンダラの取得
    private SkinnedMeshRenderer smr;

    // プレイヤーコントローラーの取得
    [SerializeField]
    private PlayerController player;

    private int random;

    // Start is called before the first frame update
    void Start()
    {
        /// プレイヤーのマテリアルの種類を通常にする
        changeMaterials = ChangeMaterials.normal;

        // 最初のマテリアルを設定する
        smr = GetComponent<SkinnedMeshRenderer>();
        smr.material = materials[(int)changeMaterials];

        random = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        // 0 から 99 の間でランダムな数を代入する
        random = Random.Range(0, 100);

        ChangeExpression();

        // マテリアルを設定する
        smr.material = materials[(int)changeMaterials];

        if (random == 0)
        {
            // マテリアルを変更する
            smr.material = materials[(int)ChangeMaterials.closeEye];
        }
    }

    private void ChangeExpression()
    {
        switch (player.playerState)
        {
            case PlayerController.PlayerState.appear:
                break;

            case PlayerController.PlayerState.stay:
                changeMaterials = ChangeMaterials.normal;
                break;

            case PlayerController.PlayerState.move:
                break;

            case PlayerController.PlayerState.lose:
                changeMaterials = ChangeMaterials.lose;
                break;

            case PlayerController.PlayerState.win:
                changeMaterials = ChangeMaterials.smile;
                break;

            case PlayerController.PlayerState.exit:
                break;
        }
    }

    public ChangeMaterials GetChangeMaterials()
    {
        return changeMaterials;
    }

    public void SetChangeMaterial(ChangeMaterials materials)
    {
        changeMaterials = materials;
    }
}
