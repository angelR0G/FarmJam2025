using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PigComponent : MonoBehaviour
{
    // Components
    [Header("Components References")]
    public Rigidbody2D body;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains pig's states. Should be a child of pig game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public PigState currentState = null;
    [HideInInspector] public PigWalkingState walkingState = null;
    [HideInInspector] public PigEatingState eatingState = null;
    [HideInInspector] public PigSleepingState sleepingState = null;
    [HideInInspector] public PigPanicState panicState = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();

        // Init pig states
        walkingState = statesContainer.GetComponent<PigWalkingState>();
        eatingState = statesContainer.GetComponent<PigEatingState>();
        sleepingState = statesContainer.GetComponent<PigSleepingState>();
        panicState = statesContainer.GetComponent<PigPanicState>();

        ChangeState(walkingState);
    }

    void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(PigState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }
}
