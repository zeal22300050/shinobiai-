using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSEController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    private bool isStageSE;

    // Start is called before the first frame update
    void Start()
    {
        isStageSE = false;
    }

    private void Update()
    {
        if(isStageSE)
        {
            audioSource.Play();
            isStageSE = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isStageSE = true;
        }
    }
}
