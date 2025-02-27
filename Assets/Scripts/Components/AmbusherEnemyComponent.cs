using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherEnemyComponent : EnemyComponent
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public CircleCollider2D enemyCollider;
    public AttackComponent attackComponent;
    public Animator animator;
    public AudioSource audioSource;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    [HideInInspector] public AmbusherHidingState hidingState = null;
    [HideInInspector] public AmbusherIdleState idleState = null;
    [HideInInspector] public AmbusherAttackingState attackingState = null;
    [HideInInspector] public AmbusherDyingState dyingState = null;

    // Other properties
    [Header("Enemy properties")]
    public GameObject attackTarget;
    public GameObject attackProjectile;
    public Sprite corpseSprite;
    public int bloodAmount = 120;
    public bool hidden = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        healthComp = GetComponent<HealthComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        attackComponent = GetComponent<AttackComponent>();
        audioSource = GetComponent<AudioSource>();

        // Init enemy states
        hidingState = statesContainer.GetComponent<AmbusherHidingState>();
        idleState = statesContainer.GetComponent<AmbusherIdleState>();
        attackingState = statesContainer.GetComponent<AmbusherAttackingState>();
        dyingState = statesContainer.GetComponent<AmbusherDyingState>();

        initialState = hidingState;
        ChangeState(hidingState);

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

    public void ChangeState(AmbusherState newState)
    {
        if (currentState == (dyingState as IState)) return;

        base.ChangeState(newState);
    }

    private void OnDie()
    {
        if (currentState != (dyingState as IState))
        {
            ChangeState(dyingState);
        }
        else
        {
            GameObject corpse = ItemFactory.CreateCorpse(transform.position, bloodAmount, CorpseCreature.Ambusher, corpseSprite);
            corpse.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
            Deactivate(false);
            hidden = true;
        }
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }

    private void OnAttack()
    {
        if (currentState == (attackingState as IState))
        {
            attackingState.SetProjectileActive(true);
        }
    }
}
