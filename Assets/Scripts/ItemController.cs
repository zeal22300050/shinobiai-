using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    // 使用できるアイテムの最大数
    public int maxItemNumber { get; private set; }

    // アイテムの残りの数
    public int itemNumber { get; private set; }

    // 数字表示するためのテキスト
    [SerializeField]
    private TextMeshProUGUI numberText;

    // アイテムのアイコン
    [SerializeField]
    private Image itemImage;

    // Start is called before the first frame update
    void Start()
    {
        // アイテムの数を設定
        maxItemNumber = 5;
        itemNumber = maxItemNumber;
    }

    private void FixedUpdate()
    {
        ItemNumberDisplay();

        if(itemNumber == 0)
        {
            ItemColorChange();
        }
    }

    /// <summary>
    /// 残りアイテム数を画面上に表示させる
    /// </summary>
    private void ItemNumberDisplay()
    {
        // 残りアイテム数を表示させる
        numberText.GetComponent<TextMeshProUGUI>().text = "<sprite=" + itemNumber + ">";
    }

    /// <summary>
    /// アイテムアイコンの色を変更させる
    /// </summary>
    private void ItemColorChange()
    {
        // アイテムアイコンの色を変更する
        itemImage.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    /// <summary>
    /// 残りアイテム数再設定用の関数
    /// </summary>
    /// <param name="num">
    /// アイテムの数
    /// </param>
    public void SetItemNumber(int num)
    {
        itemNumber = num;
    }
}
