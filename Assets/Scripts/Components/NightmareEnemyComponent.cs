using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareEnemyComponent : MonoBehaviour
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public CircleCollider2D enemyCollider;
    public AttackComponent attackComponent;
    public Animator animator;
    public LightDetectorComponent lightDetector;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public NightmareState currentState = null;
    [HideInInspector] public NightmareFlyingState flyingState = null;
    [HideInInspector] public NightmareAttackState attackState = null;
    [HideInInspector] public NightmareAvoidState avoidState = null;
    [HideInInspector] public NightmareTeleportState teleportState = null;
    [HideInInspector] public NightmareDyingState dyingState = null;

    // Other properties
    [Header("Enemy properties")]
    public GameObject attackTarget;
    public float targetTimeInLight = 0;
    public Sprite corpseSprite;
    public int bloodAmount = 800;

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
        lightDetector = GetComponent<LightDetectorComponent>();

        // Init enemy states
        flyingState = statesContainer.GetComponent<NightmareFlyingState>();
        attackState = statesContainer.GetComponent<NightmareAttackState>();
        avoidState = statesContainer.GetComponent<NightmareAvoidState>();
        teleportState = statesContainer.GetComponent<NightmareTeleportState>();
        dyingState = statesContainer.GetComponent<NightmareDyingState>();

        ChangeState(flyingState);

        healthComp.onDieCallback = OnDie;
        lightDetector.enterLight.AddListener(OnEnterLight);
        lightDetector.exitLight.AddListener(OnExitLight);
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

    public void ChangeState(NightmareState newState)
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

    private void OnAttack()
    {
        if (currentState == attackState)
        {
            attackState.Attack();
        }
    }

    private void OnEnterLight()
    {
        animator.SetFloat("AnimSpeed", 0.3f);
    }

    private void OnExitLight()
    {
        animator.SetFloat("AnimSpeed", 0.75f);
    }

    public void Disappear()
    {
        ChangeState(null);
        Destroy(gameObject);
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }

    private void OnDestroy()
    {
        lightDetector.enterLight.RemoveAllListeners();
        lightDetector.exitLight.RemoveAllListeners();
    }
}
