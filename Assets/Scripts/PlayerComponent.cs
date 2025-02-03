using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    // Components
    public InputComponent inputComponent;
    public Rigidbody2D body;

    // States
    public IState currentState = null;
    private PlayerWalkingState walkingState = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();

        // Init player states
        walkingState = new PlayerWalkingState(this);

        ChangeState(walkingState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState();
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }
}
