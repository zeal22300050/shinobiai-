using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSkyBox : MonoBehaviour
{
    private const float RotateSpeed = 1.0f;

    private Material skyboxMaterial;

    // Start is called before the first frame update
    void Start()
    {
        skyboxMaterial = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        skyboxMaterial.SetFloat("_Rotation", Mathf.Repeat(skyboxMaterial.GetFloat("_Rotation") + RotateSpeed * Time.deltaTime, 360f));
    }
}
