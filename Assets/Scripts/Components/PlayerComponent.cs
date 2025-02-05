using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InventoryComponent))]
public class PlayerComponent : MonoBehaviour
{
    // Components
    [Header("Components References")]
    public InputComponent inputComponent;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public InventoryComponent inventory;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains player's states. Should be a child of player game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public IState currentState = null;
    [HideInInspector] public PlayerWalkingState walkingState = null;
    [HideInInspector] public PlayerDiggingState diggingState = null;

    // Other variables
    [Header("Player properties")]
    public bool isInteractionEnabled = true;
    private List<InteractionTriggerComponent> interactables = new List<InteractionTriggerComponent>(2);


    // Start is called before the first frame update
    void Start()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        inventory = GetComponent<InventoryComponent>();

        // Init player states
        walkingState = gameObject.GetComponentInChildren<PlayerWalkingState>();
        diggingState = gameObject.GetComponentInChildren<PlayerDiggingState>();

        ChangeState(walkingState);

        // Bind inputs
        inventory.BindInput(inputComponent);
        inputComponent.interactInputEvent.AddListener(Interact);
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

    public void ChangeState(IState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    private void Interact()
    {
        ItemComponent equipedItem = inventory.GetEquipedItem();

        if (UseTool(equipedItem)) return;

        InteractWithWorld();
    }

    private bool UseTool(ItemComponent equipedTool)
    {
        if (equipedTool == null || equipedTool.Type != ItemType.Tool) return false;

        if (equipedTool.Id == ItemId.Hoe)
        {
            ChangeState(diggingState);
            return true;
        }

        return false;
    }

    private void InteractWithWorld()
    {
        if (!isInteractionEnabled || interactables.Count == 0) return;

        interactables[interactables.Count - 1].Interact(this);
    }

    public void SetInteractableObject(InteractionTriggerComponent newInteraction)
    {
        interactables.Add(newInteraction);
        sprite.color = Color.red;
    }

    public void RemoveInteractableObject(InteractionTriggerComponent removedInteraction)
    {
        interactables.Remove(removedInteraction);

        if (interactables.Count == 0 )
            sprite.color = Color.white;
    }
}
