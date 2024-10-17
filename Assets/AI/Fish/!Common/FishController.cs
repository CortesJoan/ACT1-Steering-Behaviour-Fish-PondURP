using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum FishType
{
    TYPEA,
    TYPEB,
    TYPEC,
    TYPED,
    TYPEE
}
public class FishController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] FishType fishType;
    [SerializeField] SteeringBehaviour steeringBehaviourArrive;
    [SerializeField] float distanceToAvoid = 30;
    [SerializeField] Camera mainCamera;
    UnityEvent onFlee;
    float fleeSpeedMultiplier = 2;


    [SerializeField] private RenderTexture bloodAccumulation;
    [SerializeField] private Material bloodSplatterMaterial;
    [SerializeField] private Material additiveBlendMaterial;
    [SerializeField] private Material bloodEffectMaterial;
    [SerializeField] private float bloodSplatterRadius = 0.1f;
    // Start is called before the first frame update
    public UnityEvent<GameObject> onFishKilled;
    void Start()
    {
        steeringBehaviourArrive.onAvoidEvent.AddListener(HandleFishCollisionAvoidance);

        mainCamera = Camera.main;
    }

    private void HandleFishCollisionAvoidance(GameObject other )
    {
        FishController otherFishController = other.GetComponentInParent<FishController>();
        if (otherFishController)
        {
            if (transform.localScale.y < otherFishController.transform.localScale.y)
            {
                 steeringBehaviourArrive.ToggleFlee();
            }
        }    
        else{ 
        steeringBehaviourArrive.ToggleFlee();
        }
    }
    bool inRange;
    bool wasInRange;
    public float debugDistance;
    public Vector3 debugMousePositionInWorld;
    [SerializeField] private BloodEffectRenderFeature bloodEffectRenderFeature;

    // Update is called once per frame
    void Update()
    {
        debugMousePositionInWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var distance = Vector3.Distance(transform.position, debugMousePositionInWorld);
        debugDistance = distance;
        inRange = distance < distanceToAvoid;


        if (inRange && !wasInRange)
        {
            steeringBehaviourArrive.ToggleFlee();
            steeringBehaviourArrive.UpdateMaxVelocity(steeringBehaviourArrive.GetCurrentMaxVelocity() * fleeSpeedMultiplier);

            steeringBehaviourArrive.ChangeWanderState(false);
        }
        else if (!inRange && wasInRange)
        {
            steeringBehaviourArrive.ChangeWanderState(true);
            steeringBehaviourArrive.UpdateMaxVelocity(steeringBehaviourArrive.GetCurrentMaxVelocity() / fleeSpeedMultiplier);

        }

        wasInRange = inRange;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer Click");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            KillFish();
        }
    }

    private void KillFish()
    {
 
        RenderTexture bloodAccumulationTexture = bloodEffectRenderFeature.settings.bloodAccumulationTexture;

         RenderTexture tempRT = RenderTexture.GetTemporary(bloodAccumulationTexture.width, bloodAccumulationTexture.height, 0);
        RenderTexture.active = tempRT;
        GL.Clear(true, true, Color.clear);

         Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        float scale = bloodSplatterRadius / Screen.width;

         Graphics.Blit(null, tempRT, bloodSplatterMaterial);
        
        RenderTexture.active = bloodAccumulationTexture;
        Graphics.Blit(tempRT, bloodAccumulationTexture, additiveBlendMaterial);

        RenderTexture.ReleaseTemporary(tempRT);
        RenderTexture.active = null;
        onFishKilled?.Invoke(this.gameObject);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
    }
    public FishType GetFishType() =>fishType;

    internal void SetMass(float xScale)
    {
        steeringBehaviourArrive.mass = xScale;
     }
}
