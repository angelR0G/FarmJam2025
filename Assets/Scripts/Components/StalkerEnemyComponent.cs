using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StalkerEnemyComponent : MonoBehaviour
{
    // Components
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public SpriteRenderer sprite;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public StalkerState currentState = null;
    [HideInInspector] public StalkerWanderState wanderState = null;
    [HideInInspector] public StalkerStalkState stalkState = null;
    [HideInInspector] public StalkerChaseState chaseState = null;
    [HideInInspector] public StalkerAttackState attackState = null;
    [HideInInspector] public StalkerRecoverState recoverState = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        healthComp = GetComponent<HealthComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // Init enemy states
        wanderState = statesContainer.GetComponent<StalkerWanderState>();
        stalkState = statesContainer.GetComponent<StalkerStalkState>();
        chaseState = statesContainer.GetComponent<StalkerChaseState>();
        attackState = statesContainer.GetComponent<StalkerAttackState>();
        recoverState = statesContainer.GetComponent<StalkerRecoverState>();

        ChangeState(wanderState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(StalkerState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public bool IsGameObjectInSight(GameObject obj, float maxDistance = -1)
    {
        if (obj == null) return false;

        Vector2 vectorToObject = obj.transform.position - transform.position;

        if (maxDistance > 0 && vectorToObject.magnitude > maxDistance) return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorToObject.normalized, vectorToObject.magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == gameObject || hit.collider.gameObject == obj.gameObject) continue;

            // Another object's collision is in the line of sight
            return false;
        }

        return true;
    }
}
