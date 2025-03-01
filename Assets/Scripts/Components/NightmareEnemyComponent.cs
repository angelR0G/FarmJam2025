using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareEnemyComponent : EnemyComponent
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public CircleCollider2D enemyCollider;
    public AttackComponent attackComponent;
    public Animator animator;
    public LightDetectorComponent lightDetector;
    public AudioSource audioSource;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    [HideInInspector] public NightmareFlyingState flyingState = null;
    [HideInInspector] public NightmareAttackState attackState = null;
    [HideInInspector] public NightmareAvoidState avoidState = null;
    [HideInInspector] public NightmareTeleportState teleportState = null;
    [HideInInspector] public NightmareDyingState dyingState = null;

    // Other properties
    [Header("Enemy properties")]
    public float targetTimeInLight = 0;
    public Sprite corpseSprite;
    public int bloodAmount = 800;
    public AudioClip wingSound;

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
        audioSource = GetComponent<AudioSource>();

        // Init enemy states
        flyingState = statesContainer.GetComponent<NightmareFlyingState>();
        attackState = statesContainer.GetComponent<NightmareAttackState>();
        avoidState = statesContainer.GetComponent<NightmareAvoidState>();
        teleportState = statesContainer.GetComponent<NightmareTeleportState>();
        dyingState = statesContainer.GetComponent<NightmareDyingState>();

        initialState = teleportState;
        ChangeState(teleportState);

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
        }
    }

    private void OnAttack()
    {
        if (currentState == (attackState as IState))
        {
            attackState.Attack();
        }
    }

    private void OnEnterLight()
    {
        animator.SetFloat("AnimSpeed", 0.3f);

        if (currentState == flyingState as IState || currentState == avoidState as IState)
            SetWingSoundEnabled(true);
    }

    private void OnExitLight()
    {
        animator.SetFloat("AnimSpeed", 0.75f);
        if (currentState == flyingState as IState || currentState == avoidState as IState)
            SetWingSoundEnabled(true);
    }

    public void Disappear()
    {
        Deactivate();
    }

    public void SetWingSoundEnabled(bool newState)
    {
        CancelInvoke("PlayWingSound");

        if (newState)
        {
            InvokeRepeating("PlayWingSound", 0f, lightDetector.IsInsideLight() ? 1f : 0.5f);
        }
    }

    private void PlayWingSound()
    {
        if (deactivated)
            CancelInvoke("PlayWingSound");
        else
            audioSource.PlayOneShot(wingSound);
    }

    private void OnDestroy()
    {
        if (lightDetector)
        {
            lightDetector.enterLight.RemoveAllListeners();
            lightDetector.exitLight.RemoveAllListeners();
        }
    }
}
