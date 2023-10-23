using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Grid mapGrid; // セルサイズ取得用

    private Vector2 moveDistance; // 一度に移動する距離
    private Vector3 arrowKeyInput; // 矢印キーの入力を取得する変数

    // Start is called before the first frame update
    void Start()
    {
        // 一回の移動距離をセルサイズに合わせる
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        // 矢印キーの入力を取得
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);

        // キーが押された瞬間なら
        if (ArrowKeyDown())
        {
            transform.position += arrowKeyInput; // 移動する
        }
    }

    // 矢印キーが押された瞬間かどうかを返す関数
    bool ArrowKeyDown()
    {
        // 矢印キーのいずれかが押された瞬間なら
        if (Input.GetKeyDown(KeyCode.RightArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            return true; // true
        }
        else // そうでなければ
        {
            return false; // false
        }
    }
}
