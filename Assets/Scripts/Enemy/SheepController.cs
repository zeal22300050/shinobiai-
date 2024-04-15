using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public enum Status
    {
        Sleep,
        Rise,
        Find
    }
    private Status status;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private GameObject searchRangeDeco; // 索敵範囲を見えるようにするオブジェクト

    private BoxCollider searchRange;

    private int sleepInterval; // 寝る間隔

    // Start is called before the first frame update
    private void Start()
    {
        searchRange = GetComponent<BoxCollider>(); // コンポーネントの取得

        // テスト用に2を代入
        sleepInterval = 2;
    }

    // Update is called once per frame
    private void Update()
    {   
        // ターン数を就寝間隔で割って0なら
        if (player.currentTurnNum % sleepInterval == 0) 
        {
            status = Status.Sleep;
            searchRange.enabled = false; // 索敵しなくなる
            searchRangeDeco.SetActive(false);
        }
        else // それ以外なら
        {
            status = Status.Rise;
            searchRange.enabled = true; // 索敵する
            searchRangeDeco.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーを発見した場合
        if (other.gameObject.tag == "Player")
        {
            status = Status.Find;
        }
    }

    public Status GetSheepStatus()
    {
        return status;
    }
}
