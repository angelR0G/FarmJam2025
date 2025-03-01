using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

public class StalkerEnemyComponent : EnemyComponent
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public CircleCollider2D enemyCollider;
    public AttackComponent attackComp;
    public Animator animator;
    public AudioSource audioSource;
    public PositionedAudioComponent positionedAudioComp;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    [HideInInspector] public StalkerWanderState wanderState = null;
    [HideInInspector] public StalkerStalkState stalkState = null;
    [HideInInspector] public StalkerChaseState chaseState = null;
    [HideInInspector] public StalkerAttackState attackState = null;
    [HideInInspector] public StalkerRecoverState recoverState = null;
    [HideInInspector] public StalkerDieState dieState = null;

    // Other properties
    [Header("Enemy properties")]
    public Sprite corpseSprite;
    public int bloodAmount = 150;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        healthComp = GetComponent<HealthComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();
        attackComp = GetComponent<AttackComponent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        positionedAudioComp = GetComponent<PositionedAudioComponent>();

        // Init enemy states
        wanderState = statesContainer.GetComponent<StalkerWanderState>();
        stalkState = statesContainer.GetComponent<StalkerStalkState>();
        chaseState = statesContainer.GetComponent<StalkerChaseState>();
        attackState = statesContainer.GetComponent<StalkerAttackState>();
        recoverState = statesContainer.GetComponent<StalkerRecoverState>();
        dieState = statesContainer.GetComponent<StalkerDieState>();

        initialState = wanderState;
        ChangeState(wanderState);

        healthComp.onDieCallback = OnDie;
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

    public void ChangeState(StalkerState newState)
    {
        if (currentState == (dieState as IState)) return;

        base.ChangeState(newState);
    }

    private void OnDie()
    {
        if (currentState != (dieState as IState))
        {
            ChangeState(dieState);
        }
        else
        {
            GameObject corpse = ItemFactory.CreateCorpse(transform.position, bloodAmount, CorpseCreature.Stalker, corpseSprite);
            corpse.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
            Deactivate(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == (attackState as IState))
            attackState.FinishAttack();
    }
}
