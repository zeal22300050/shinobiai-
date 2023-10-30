using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    /// <summary>
    /// スプライト更新用
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// プレイヤーの状態取得用
    /// </summary>
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private GameObject zonePattern1;
    [SerializeField]
    private GameObject zonePattern2;

    // Start is called before the first frame update
    void Start()
    {
        // コンポーネントの取得
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // 現在のターン数が偶数なら
        if (playerController.GetTurnNumber() % 2 == 0)
        {
            // パターン1を無効化、パターン2を有効化
            zonePattern1.SetActive(false);
            zonePattern2.SetActive(true);

            spriteRenderer.flipX = true; // X軸を基準にして反転する
        }
        else // 奇数なら
        {
            // パターン1を有効化、パターン2を無効化
            zonePattern1.SetActive(true);
            zonePattern2.SetActive(false);

            spriteRenderer.flipX = false; // そのまま
        }
    }
}
