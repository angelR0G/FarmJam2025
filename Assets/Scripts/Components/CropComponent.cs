using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropComponent : MonoBehaviour
{
    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains crop's states. Should be a child of crop game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public CropState currentState = null;
    [HideInInspector] public CropDryState dryState = null;
    [HideInInspector] public CropWateredState wateredState = null;
    [HideInInspector] public CropGrownState grownState = null;

    // Start is called before the first frame update
    void Start()
    {
        // Init crop states
        dryState = gameObject.GetComponentInChildren<CropDryState>();
        wateredState = gameObject.GetComponentInChildren<CropWateredState>();
        grownState = gameObject.GetComponentInChildren<CropGrownState>();

        ChangeState(dryState);
    }

    public void ChangeState(CropState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }
}
