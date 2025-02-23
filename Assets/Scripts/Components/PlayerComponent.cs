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
    public AttackComponent attackComponent;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains player's states. Should be a child of player game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public PlayerState currentState = null;
    [HideInInspector] public PlayerWalkingState walkingState = null;
    [HideInInspector] public PlayerDiggingState diggingState = null;
    [HideInInspector] public PlayerWateringState wateringState = null;
    [HideInInspector] public PlayerAttackState attackState = null;
    [HideInInspector] public PlayerFinishAttackState finishAttackState = null;
    [HideInInspector] public PlayerCarryingState carringState = null;
    [HideInInspector] public PlayerExtractingBloodState extractingBloodState = null;
    [HideInInspector] public PlayerSacrificingState sacrificeState = null;

    // Other variables
    [Header("Player properties")]
    private bool isInteractionEnabled = true;
    public Vector2 facingDirection = Vector2.down;
    private List<InteractionTriggerComponent> interactables = new List<InteractionTriggerComponent>(2);
    public UnityAction onAnimFinished;
    public UnityAction onAnimEvent;

    public bool IsInteractionEnabled {  
        get { return isInteractionEnabled; } 
        set { 
            isInteractionEnabled = value; 
            inventory.blockInventory = !value; 
        } 
    }


    // Start is called before the first frame update
    void Start()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackComponent = GetComponent<AttackComponent>();

        // Init player states
        walkingState = statesContainer.GetComponent<PlayerWalkingState>();
        diggingState = statesContainer.GetComponent<PlayerDiggingState>();
        wateringState = statesContainer.GetComponent<PlayerWateringState>();
        attackState = statesContainer.GetComponent<PlayerAttackState>();
        finishAttackState = statesContainer.GetComponent<PlayerFinishAttackState>();
        carringState = statesContainer.GetComponent<PlayerCarryingState>();
        extractingBloodState = statesContainer.GetComponent<PlayerExtractingBloodState>();
        sacrificeState = statesContainer.GetComponent<PlayerSacrificingState>();

        ChangeState(walkingState);

        // Bind inputs
        inventory.BindInput(inputComponent);
        inputComponent.interactInputEvent.AddListener(Interact);
        inventory.AddItem(ItemId.Pumpkin, 10);

        GetComponent<HealthComponent>().onDamageEvent.AddListener(OnDamaged);
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
        else if (equipedTool.Id == ItemId.Pitchfork)
        {
            ChangeState(attackState);
        }
        else if (equipedTool.Id == ItemId.Dagger)
        {
            ChangeState(extractingBloodState);
        }
        else if (equipedTool.Id == ItemId.Torch)
        {
            GameObject torch = ItemFactory.CreateTorch(transform.position);
            if (torch != null) inventory.RemoveEquipedItem();
        }
        else
        {
            return false;
        }

        return true;
    }

    private void InteractWithWorld()
    {
        if (interactables.Count > 1)
        {
            // Interact with the closest interactable object
            int closestInteractableIndex = 0;
            float distanceToClosestInteractable = float.MaxValue;

            for (int i = 0; i < interactables.Count; i++)
            {
                float distance = Vector3.SqrMagnitude(interactables[i].transform.position - transform.position);
                if (distance < distanceToClosestInteractable)
                {
                    closestInteractableIndex = i;
                    distanceToClosestInteractable = distance;
                }
            }

            interactables[closestInteractableIndex].Interact(this);
        }
        else if (interactables.Count == 1)
        {
            interactables[0].Interact(this);
        }
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

    private void OnAnimationEvent()
    {
        onAnimEvent?.Invoke();
    }

    private void OnDamaged()
    {
        ChangeState(walkingState);
    }
}
