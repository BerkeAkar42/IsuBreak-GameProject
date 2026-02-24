using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //Sens ayarýný 50 ila 500 arasýnda bir deðer yapmamýzý saðlýyor.
    [Range(50, 500)]
    public float sens;

    public Transform body;

    float xRot = 0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float rotX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float rotY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        xRot -= rotY;
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        body.Rotate(Vector3.up * rotX);

    }
}
