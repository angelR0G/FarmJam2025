using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    // Components
    [Header("Components References")]
    public InputComponent inputComponent;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public InventoryComponent inventory;
    public Animator animator;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains player's states. Should be a child of player game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public PlayerState currentState = null;
    [HideInInspector] public PlayerWalkingState walkingState = null;
    [HideInInspector] public PlayerDiggingState diggingState = null;
    [HideInInspector] public PlayerWateringState wateringState = null;

    // Other variables
    [Header("Player properties")]
    public bool isInteractionEnabled = true;
    public Vector2 facingDirection = Vector2.down;
    private List<InteractionTriggerComponent> interactables = new List<InteractionTriggerComponent>(2);
    public UnityAction onAnimFinished;


    // Start is called before the first frame update
    void Start()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Init player states
        walkingState = statesContainer.GetComponent<PlayerWalkingState>();
        diggingState = statesContainer.GetComponent<PlayerDiggingState>();
        wateringState = statesContainer.GetComponent<PlayerWateringState>();

        ChangeState(walkingState);

        // Bind inputs
        inventory.BindInput(inputComponent);
        inputComponent.interactInputEvent.AddListener(Interact);

        inventory.AddItem(ItemId.Hoe);
        inventory.AddItem(ItemId.WaterCan);
        inventory.AddItem(ItemId.CornSeed);
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

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    private void Interact()
    {
        if (!isInteractionEnabled) return;

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
        }
        else if (equipedTool.Id == ItemId.WaterCan)
        {
            ChangeState(wateringState);
        }
        else
        {
            return false;
        }

        return true;
    }

    private void InteractWithWorld()
    {
        if (interactables.Count == 0) return;

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

    private void OnAnimationFinished()
    {
        onAnimFinished?.Invoke();
    }
}
