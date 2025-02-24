using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveEnemyComponent : MonoBehaviour
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public CircleCollider2D enemyCollider;
    public Animator animator;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public CorrosiveState currentState = null;
    [HideInInspector] public CorrosiveIdleState idleState = null;
    [HideInInspector] public CorrosiveFollowState followState = null;
    [HideInInspector] public CorrosiveChargingState chargingState = null;
    [HideInInspector] public CorrosiveExplodingState explodingState = null;
    [HideInInspector] public CorrosiveDyingState dyingState = null;
    [HideInInspector] public CorrosiveReturningState returningState = null;

    // Other properties
    [Header("Enemy properties")]
    public Vector3 originPosition;
    public GameObject followTarget;
    public Sprite corpseSprite;
    public int bloodAmount = 80;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        healthComp = GetComponent<HealthComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();

        // Init enemy states
        idleState = statesContainer.GetComponent<CorrosiveIdleState>();
        followState = statesContainer.GetComponent<CorrosiveFollowState>();
        chargingState = statesContainer.GetComponent<CorrosiveChargingState>();
        explodingState = statesContainer.GetComponent<CorrosiveExplodingState>();
        dyingState = statesContainer.GetComponent<CorrosiveDyingState>();
        returningState = statesContainer.GetComponent<CorrosiveReturningState>();

        ChangeState(idleState);

        healthComp.onDieCallback = Die;
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
        if (currentState == dyingState) return;

        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }

    private void Die()
    {
        if (currentState != dyingState) ChangeState(dyingState);
    }

    private void DestroyEnemy()
    {
        if (currentState == dyingState)
        {
            GameObject corpse = ItemFactory.CreateCorpse(transform.position, bloodAmount, CorpseCreature.Corrosive, corpseSprite);
            corpse.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
        }

        Destroy(gameObject);
    }
}
