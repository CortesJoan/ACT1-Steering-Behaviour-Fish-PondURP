using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishVIsualsController : MonoBehaviour
{
    [SerializeField] SteeringBehaviour steeringBehaviourArrive;
    [SerializeField] SpriteRenderer spriteRenderer;
    UnityEvent onFlee;
    [SerializeField] Color fishColor;
    [SerializeField] ParticleSystem fishSpeedParticleSystem;
    [SerializeField] float maxParticles = 1000;

    private void Start()
    {
        var main = fishSpeedParticleSystem.main;
        main.startColor = fishColor;
  

    }


    // Update is called once per frame
    void LateUpdate()
    {

        Quaternion newRotation = Quaternion.LookRotation(steeringBehaviourArrive.GetCurrentDirection(), transform.TransformDirection(Vector3.up));
        spriteRenderer.transform.rotation = new Quaternion(0, 0, newRotation.z, newRotation.w);
        spriteRenderer.flipX = steeringBehaviourArrive.GetCurrentVelocity().x < 0;


        var effectMain = fishSpeedParticleSystem.main;
        effectMain.maxParticles = (int)(steeringBehaviourArrive.GetCurrentVelocity().magnitude / steeringBehaviourArrive.GetCurrentMaxVelocity() * maxParticles);
        fishSpeedParticleSystem.transform.rotation = Quaternion.Euler(newRotation.eulerAngles.x, fishSpeedParticleSystem.transform.rotation.eulerAngles.y, fishSpeedParticleSystem.transform.rotation.eulerAngles.z);
        // new Quaternion(newRotation.z, fishSpeedParticleSystem.transform.rotation.y, fishSpeedParticleSystem.transform.rotation.z,   fishSpeedParticleSystem.transform.rotation.w);

    }
}
