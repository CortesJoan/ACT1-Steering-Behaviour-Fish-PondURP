using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Teleporter teleportTo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var fish = collision.GetComponent<FishController>();
        if (fish)
        {
            fish.transform.position = teleportTo.transform.position;
        }
    }
}
