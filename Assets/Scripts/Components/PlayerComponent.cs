using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    public HealthComponent healthComponent;
    public SanityComponent sanityComponent;
    public SpriteRenderer interactionKeySprite;
    public AudioSource audioSource;
    public SpriteMask spriteMask;

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
    [HideInInspector] public PlayerDisabledState disabledState = null;
    [HideInInspector] public PlayerDyingState dyingState = null;

    // Other variables
    [Header("Player properties")]
    public Vector2 facingDirection = Vector2.down;
    public Vector3 spawnPosition;
    [SerializeField]
    private int safeAreasCount = 0;
    [Header("Events")]
    public UnityAction onAnimFinished;
    public UnityAction onAnimEvent;
    public UnityEvent onEnterSafeArea;
    public UnityEvent onExitSafeArea;
    [Header("Sounds")]
    public AudioClip painSound;
    public AudioClip pillSound;

    private Sequence interactionKeySequence;
    private List<InteractionTriggerComponent> interactables = new List<InteractionTriggerComponent>(2);
    private bool isInteractionEnabled = true;

    public bool IsInteractionEnabled {  
        get { return isInteractionEnabled; } 
        set { 
            isInteractionEnabled = value; 
            inventory.blockInventory = !value; 
        } 
    }

    public int SafeAreasCount
    {
        get { return safeAreasCount; }
        set
        {
            int previousValue = safeAreasCount;
            safeAreasCount = value;

            if (previousValue <= 0 && value >= 1)
                onEnterSafeArea.Invoke();
            else if (previousValue >= 1 && value <= 0)
                onExitSafeArea.Invoke();
        }
    }


    private void Awake()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackComponent = GetComponent<AttackComponent>();
        healthComponent = GetComponent<HealthComponent>();
        audioSource = GetComponent<AudioSource>();
        sanityComponent = GetComponent<SanityComponent>();
        spriteMask = GetComponent<SpriteMask>();

        // Init player states
        walkingState = statesContainer.GetComponent<PlayerWalkingState>();
        diggingState = statesContainer.GetComponent<PlayerDiggingState>();
        wateringState = statesContainer.GetComponent<PlayerWateringState>();
        attackState = statesContainer.GetComponent<PlayerAttackState>();
        finishAttackState = statesContainer.GetComponent<PlayerFinishAttackState>();
        carringState = statesContainer.GetComponent<PlayerCarryingState>();
        extractingBloodState = statesContainer.GetComponent<PlayerExtractingBloodState>();
        sacrificeState = statesContainer.GetComponent<PlayerSacrificingState>();
        disabledState = statesContainer.GetComponent<PlayerDisabledState>();
        dyingState = statesContainer.GetComponent<PlayerDyingState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(walkingState);

        // Bind inputs
        inventory.BindInput(inputComponent);
        inputComponent.interactInputEvent.AddListener(Interact);

        healthComponent.onDamageEvent.AddListener(OnDamaged);
        healthComponent.onDieCallback = () => ChangeState(dyingState);

        spawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        spriteMask.sprite = sprite.sprite;

        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SetEnabled(bool enabled)
    {
        ChangeState(enabled ? walkingState : disabledState);
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == dyingState) return;

        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    private void Interact()
    {
        if (!isInteractionEnabled || healthComponent.IsStunned()) return;

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
        else if (equipedTool.Id == ItemId.Painkillers)
        {
            if (healthComponent.GetHealthPercentage() < 1)
            {
                healthComponent.RestoreHealth(50);
                audioSource.PlayOneShot(pillSound);
                inventory.RemoveEquipedItem();
            }
        }else if (equipedTool.Id == ItemId.Antipsychotic)
        {
            if (sanityComponent.GetSanityPercentage() < 1)
            {
                sanityComponent.RestoreSanity(50);
                audioSource.PlayOneShot(pillSound);
                inventory.RemoveEquipedItem();
            }
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

        if (interactables.Count == 1)
        {
            if (interactionKeySequence != null && interactionKeySequence.IsActive())
                interactionKeySequence.Kill();

            interactionKeySequence = DOTween.Sequence().SetRecyclable(true);
            interactionKeySequence.AppendCallback(() => interactionKeySprite.enabled = true)
                .Append(interactionKeySprite.transform.DOScale(0.125f, 0.4f).SetEase(Ease.OutBack))
                .Join(interactionKeySprite.transform.DOLocalMoveY(0.3f, 0.4f).SetEase(Ease.OutBack))
                .Join(interactionKeySprite.DOFade(1f, 0.15f))
                .OnKill(() => interactionKeySequence = null);
        }
    }

    public void RemoveInteractableObject(InteractionTriggerComponent removedInteraction)
    {
        interactables.Remove(removedInteraction);

        if (interactables.Count == 0)
        {
            if (interactionKeySequence != null && interactionKeySequence.IsActive())
                interactionKeySequence.Kill();

            interactionKeySequence = DOTween.Sequence().SetRecyclable(true);
            interactionKeySequence.Append(interactionKeySprite.transform.DOScale(0f, 0.4f).SetEase(Ease.InBack))
                .Join(interactionKeySprite.transform.DOLocalMoveY(0.2f, 0.4f).SetEase(Ease.InBack))
                .Join(interactionKeySprite.DOFade(0f, 0.15f).SetDelay(0.25f))
                .OnComplete(() => interactionKeySprite.enabled = false)
                .OnKill(() => interactionKeySequence = null);
        }
    }

    public bool IsSafe()
    {
        return safeAreasCount > 0;
    }

    private void PlaySoundRequested()
    {
        audioSource.Play();
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
        audioSource.PlayOneShot(painSound);
        ChangeState(walkingState);
    }
}
