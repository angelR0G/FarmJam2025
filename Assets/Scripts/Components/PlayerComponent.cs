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
    public InputComponent inputComponent;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public InventoryComponent inventory;

    // States
    public IState currentState = null;
    private PlayerWalkingState walkingState = null;

    // Other variables
    private List<InteractionTriggerComponent> interactables = new List<InteractionTriggerComponent>(2);
    public bool isInteractionEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        inventory = GetComponent<InventoryComponent>();

        // Init player states
        walkingState = gameObject.AddComponent<PlayerWalkingState>();

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
        if (!isInteractionEnabled || interactables.Count == 0) return;

        interactables[interactables.Count-1].Interact(this);
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
