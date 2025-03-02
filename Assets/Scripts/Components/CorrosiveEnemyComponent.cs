using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveEnemyComponent : EnemyComponent
{
    // Components
    [Header("Components")]
    public CircleCollider2D enemyCollider;
    public Animator animator;
    public AudioSource audioSource;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    [HideInInspector] public CorrosiveIdleState idleState = null;
    [HideInInspector] public CorrosiveFollowState followState = null;
    [HideInInspector] public CorrosiveChargingState chargingState = null;
    [HideInInspector] public CorrosiveExplodingState explodingState = null;
    [HideInInspector] public CorrosiveDyingState dyingState = null;
    [HideInInspector] public CorrosiveReturningState returningState = null;

    // Other properties
    [Header("Enemy properties")]
    public Vector3 originPosition;
    public Sprite corpseSprite;
    public int bloodAmount = 80;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Init enemy states
        idleState = statesContainer.GetComponent<CorrosiveIdleState>();
        followState = statesContainer.GetComponent<CorrosiveFollowState>();
        chargingState = statesContainer.GetComponent<CorrosiveChargingState>();
        explodingState = statesContainer.GetComponent<CorrosiveExplodingState>();
        dyingState = statesContainer.GetComponent<CorrosiveDyingState>();
        returningState = statesContainer.GetComponent<CorrosiveReturningState>();

        initialState = idleState;
        ChangeState(idleState);

        healthComponent.onDieCallback = Die;
        originPosition = transform.position;
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

    public void ChangeState(CorrosiveState newState)
    {
        if (currentState == (dyingState as IState)) return;

        base.ChangeState(newState);
    }

    private void Die()
    {
        if (currentState != (dyingState as IState)) ChangeState(dyingState);
    }

    private void DestroyEnemy()
    {
        if (currentState == (dyingState as IState))
        {
            GameObject corpse = ItemFactory.CreateCorpse(transform.position, bloodAmount, CorpseCreature.Corrosive, corpseSprite);
            corpse.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
        }

        Deactivate(false);
    }
}
