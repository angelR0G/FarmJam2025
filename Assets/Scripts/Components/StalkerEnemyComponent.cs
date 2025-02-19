using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerEnemyComponent : MonoBehaviour
{
    // Components
    public HealthComponent healthComp;
    public Rigidbody2D body;

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
}
