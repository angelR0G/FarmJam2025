using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CropComponent : MonoBehaviour
{
    // Components
    [Header("Components References")]
    public Collider2D cropCollider;
    public Rigidbody2D body;
    public SpriteRenderer sprite;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains crop's states. Should be a child of crop game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public CropState currentState = null;
    [HideInInspector] public CropDryState dryState = null;
    [HideInInspector] public CropWateredState wateredState = null;
    [HideInInspector] public CropGrownState grownState = null;
    public UnityEvent stateChanged = new UnityEvent();

    // Other variables
    public short wateredDays = 0;
    public List<Sprite> cropSprites = new List<Sprite>();
    public ItemId collectableCrop = ItemId.Default;
    public int minCropProduction = 1;
    public int maxCropProduction = 1;
    [HideInInspector]
    public int cropsProducedCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        cropCollider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();

        // Init crop states
        dryState = statesContainer.GetComponent<CropDryState>();
        wateredState = statesContainer.GetComponent<CropWateredState>();
        grownState = statesContainer.GetComponent<CropGrownState>();

        ChangeState(dryState);

        if (minCropProduction <= 0) minCropProduction = 1;
        if (maxCropProduction < minCropProduction) maxCropProduction = minCropProduction;

        cropsProducedCount = Random.Range(minCropProduction, maxCropProduction + 1);
    }

    public void ChangeState(CropState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();

        stateChanged.Invoke();
    }
}
