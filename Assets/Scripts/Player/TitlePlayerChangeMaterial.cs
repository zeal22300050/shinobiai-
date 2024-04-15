using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayerChangeMaterial : MonoBehaviour
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

        // マテリアルを設定する
        smr.material = materials[(int)changeMaterials];

        if (random == 0)
        {
            // マテリアルを変更する
            smr.material = materials[(int)ChangeMaterials.closeEye];
        }
    }
}
