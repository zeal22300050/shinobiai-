using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLoadController : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    // プレイヤーが進める方向
    private Vector3[] loadAngle =
    {
        new Vector3(0.0f,-0.1f,+1.0f), // 後
        new Vector3(0.0f,-0.1f,-1.0f), // 前
        new Vector3(+1.0f,-0.1f,0.0f), // 右
        new Vector3(-1.0f,-0.1f,0.0f), // 左
    };

    private Vector3 currentPosition;

    private Vector3 playerPosition;

    [SerializeField]
    private GameObject[] starLoadObjects;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        for (int i = 0; i < starLoadObjects.Length; i++)
        {
            starLoadObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.playerState == PlayerController.PlayerState.stay)
        {
            playerPosition = player.transform.position;
            currentPosition = new Vector3(playerPosition.x, currentPosition.y, playerPosition.z);
            transform.position = currentPosition;

            for (int i = 0; i < starLoadObjects.Length; i++)
            {
                Debug.DrawRay(currentPosition, loadAngle[i] * 3.0f, Color.green, 5.0f, true);
                if (Physics.Raycast(currentPosition, loadAngle[i], 3.0f))
                {
                    starLoadObjects[i].SetActive(true);
                }
                else
                {
                    starLoadObjects[i].SetActive(false);
                }
            }
        }
        else if(player.playerState != PlayerController.PlayerState.stay && player.playerState != PlayerController.PlayerState.move)
        {
            for (int i = 0; i < starLoadObjects.Length; i++)
            {
                starLoadObjects[i].SetActive(false);
            }
        }
    }

}
