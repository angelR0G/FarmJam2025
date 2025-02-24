using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherEnemyComponent : MonoBehaviour
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public CircleCollider2D enemyCollider;
    public AttackComponent attackComponent;
    public Animator animator;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public AmbusherState currentState = null;
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

        // Init enemy states
        hidingState = statesContainer.GetComponent<AmbusherHidingState>();
        idleState = statesContainer.GetComponent<AmbusherIdleState>();
        attackingState = statesContainer.GetComponent<AmbusherAttackingState>();
        dyingState = statesContainer.GetComponent<AmbusherDyingState>();

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
        if (currentState == dyingState) return;

        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    private void OnDie()
    {
        if (currentState != dyingState)
        {
            ChangeState(dyingState);
        }
        else
        {
            GameObject corpse = ItemFactory.CreateCorpse(transform.position, bloodAmount, CorpseCreature.Ambusher, corpseSprite);
            corpse.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
            Destroy(gameObject);
        }
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }

    private void OnAttack()
    {
        if (currentState == attackingState)
        {
            attackingState.SetProjectileActive(true);
        }
    }
}
