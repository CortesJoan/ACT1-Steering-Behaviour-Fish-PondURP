using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SteeringBehaviour : MonoBehaviour
{ 
    [SerializeField] Transform target;
    [SerializeField] Vector3 velocity;
    [Range(1, 100)][SerializeField] float maxVelocity;
    [SerializeField] private float maxForce = 2;
    public float mass = 1;
    Vector3 currentPosition;
    Vector3 desiredVelocity;
    Vector3 currentDirectionVector;

    [SerializeField] bool flee = false;

    [Header("Arrival Settings")]
    [SerializeField] bool useArrival = false;
    [SerializeField] float slowRadius = 9999;

    [Header("Wander settings")]

    [SerializeField] bool wandering = false;
    [SerializeField] private float wanderEvaluationTime = 0.1f;
    private float wanderTimer = 0.1f;
    [SerializeField] private float wanderCircleDistance = 10f;
    [SerializeField] private float wanderCircleRadius = 10f;
    [SerializeField] private float wanderChange = 10f;
    [SerializeField] private Vector2 displacementVectorDirection = Vector2.up;
    float wanderAngle = 0;


    [Header("Collision avoidance")]
    [SerializeField] bool toggleCollisionAvoidance;
    [SerializeField] float maxSeeAhead = 3f;
    [SerializeField] float maxAvoidForce = 30f;
    [SerializeField] LayerMask avoidLayerMask;
    [SerializeField] public UnityEvent<GameObject> onAvoidEvent;
    void Start()
    {

    }
    //https://code.tutsplus.com/understanding-steering-behaviors-seek--gamedev-849t

    void Update()
    {
        if (wandering)
        {
            Wander();
        }
        else
        {

            UpdateDirection(target.transform.position);
            UpdateVelocity();
            ApplySteering();

        }
        UpdatePosition();
    }
    private void Wander()
    {
        Vector3 newTargetPos = target.transform.position;
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderEvaluationTime)
        {
            wanderTimer = 0;
            UpdateDirection(target.transform.position);
 

            Vector3 circleCenter = transform.position + desiredDirection * wanderCircleDistance;
            float randomAngleToAdd = Random.Range(-wanderChange, wanderChange);
            wanderAngle += randomAngleToAdd;

            Vector3 displacement = Quaternion.AngleAxis(wanderAngle, Vector3.forward) * (displacementVectorDirection * wanderCircleRadius);
            newTargetPos = circleCenter + displacement;
            target.transform.position = newTargetPos;
        }
        UpdateDirection(newTargetPos);
        UpdateVelocity();
        ApplySteering();
    }
    Vector3 desiredDirection;
    void UpdateDirection(Vector3 targetPosition)
    {
        currentPosition = transform.position;
        currentDirectionVector = (targetPosition - currentPosition);
        desiredDirection = currentDirectionVector.normalized;


        if (flee) desiredDirection *= -1;


    }

 
    void UpdateVelocity()
    {
        float distance = currentDirectionVector.magnitude;
        desiredVelocity = distance < slowRadius && useArrival ?
            (distance / slowRadius) * maxVelocity * desiredDirection
            : desiredDirection * maxVelocity;
    }
    Vector3 GetLookAheadVector()
    {
        return velocity.normalized * maxSeeAhead;
    }
    RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[4];
    Vector3 CollisionAvoidance()
    {

        Vector3 steeringForce = Vector3.zero;
        int resultsNumber = Physics2D.RaycastNonAlloc(transform.position, GetLookAheadVector(), raycastHit2Ds, maxSeeAhead, avoidLayerMask);
 
        foreach (var hit in raycastHit2Ds)
        {
            if (hit.collider && !hit.collider.isTrigger && !hit.collider.gameObject.GetComponentInParent<FishController>())
            {
 
                steeringForce = GetLookAheadVector() - hit.collider.transform.position;
                steeringForce = steeringForce.normalized * maxAvoidForce;
                onAvoidEvent?.Invoke(hit.collider.gameObject);
                break;
            }
        }

        return steeringForce;
    }
    void ApplySteering()
    {
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering / mass;

        if(toggleCollisionAvoidance)
        steering += CollisionAvoidance();


        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity);


    }
    private void UpdatePosition()
    {
        Vector3 newPosition = currentPosition + velocity * Time.deltaTime;
        transform.position = newPosition;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + GetLookAheadVector());
        Gizmos.color = Color.red;
    }
    public void ToggleFlee() {
        this.flee = !flee;
    }
    public void ChangeWanderState(bool newState)
    {
        this.wandering = newState;
    }
    public Vector3 GetCurrentDirection()
    {
        return currentDirectionVector;
    }
    public Vector3 GetCurrentVelocity()
    {
        return velocity;
    }
    public void UpdateMaxVelocity(float maxVelocity)
    {
          
        this.maxVelocity = maxVelocity;
    }

    public float GetCurrentMaxVelocity()
    {
        return maxVelocity;
    } 
}