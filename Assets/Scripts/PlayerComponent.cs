using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    private InputComponent inputComponent;
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = inputComponent.movementDirection;
    }
}
