using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMouse : MonoBehaviour
{
    Camera mainCamera;

    void Start(){
        mainCamera = Camera.main;
    }
    void Update()
    {
        this.transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);

    }
}
