using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaffoldController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObj;

    private Animator animator;

    private Transform playerTrans;
    private Transform trans;

    private Vector3 distance;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerTrans = playerObj.transform;
        trans = this.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        distance = new Vector3(trans.position.x - playerTrans.position.x, 0.0f, trans.position.z - playerTrans.position.z);
        
        if (distance.magnitude <= 3.0f && !((int)distance.x == 0 && (int)distance.z == 0))
        {
            animator.SetBool("State_Movable", true);
        }
        else
        {
            animator.SetBool("State_Movable", false);
        }

    }
}
