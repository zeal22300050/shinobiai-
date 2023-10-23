using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Grid mapGrid;

    [SerializeField]
    private Texture2D gridIcon;

    private Vector2 moveDistance;
    private Vector3 arrowKeyInput;

    // Start is called before the first frame update
    void Start()
    {        
        moveDistance = new Vector2(mapGrid.cellSize.x, mapGrid.cellSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        arrowKeyInput = new Vector3(Input.GetAxisRaw("Horizontal") * moveDistance.x, Input.GetAxisRaw("Vertical") * moveDistance.y, 0);
        if (ArrowKeyDown())
        {
            transform.position += arrowKeyInput;
        }
    }

    bool ArrowKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
