using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMouse : MonoBehaviour
{
    Camera mainCamera;
    public bool mousePressed;
    void Start(){
        mainCamera = Camera.main;
    }
    void Update()
    {
        this.transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePressed = Input.GetMouseButtonDown(0);
 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var fish = collision.GetComponentInParent<FishController>();
        if (fish &&mousePressed)
        {
            fish.KillFish();
        }
    }
}
