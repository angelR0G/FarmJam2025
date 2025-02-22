using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

public class PigComponent : MonoBehaviour
{
    private const int STARVING_DAYS_TO_DIE = 3;
    private const int MIN_HUNGRY_HOUR = 10;
    private const int MAX_HUNGRY_HOUR = 13;
    private const int MAX_AGE = 3;
    private const float MASS_INCREMENT_WITH_AGE = 3;
    private const float FOOD_DETECTION_DISTANCE = 0.15f;
    private static readonly int[] BLOOD_BY_AGE = { 100, 250, 400, 1000 };

    // Components
    [Header("Components References")]
    public Rigidbody2D body;
    public HealthComponent healthComp;
    public Animator animator;
    public SpriteRenderer sprite;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains pig's states. Should be a child of pig game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public PigState currentState = null;
    [HideInInspector] public PigWalkingState walkingState = null;
    [HideInInspector] public PigEatingState eatingState = null;
    [HideInInspector] public PigSleepingState sleepingState = null;
    [HideInInspector] public PigPanicState panicState = null;

    // Other properties
    public bool isHungry = false;
    public bool hasEatenToday = false;
    public int starvingDaysCount = 0;
    public int hungryHour = 0;
    public int age = 0;
    public FarmyardComponent farmyard = null;
    public Vector2 facingDirection = Vector2.down;
    public Sprite corpseSprite = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();
        healthComp = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        // Init pig states
        walkingState = statesContainer.GetComponent<PigWalkingState>();
        eatingState = statesContainer.GetComponent<PigEatingState>();
        sleepingState = statesContainer.GetComponent<PigSleepingState>();
        panicState = statesContainer.GetComponent<PigPanicState>();

        ChangeState(walkingState);

        // Initialize hunger properties and bind events
        hungryHour = Random.Range(MIN_HUNGRY_HOUR, MAX_HUNGRY_HOUR + 1);
        GameManager.Instance.dayChanged += OnDayChanged;
        GameManager.Instance.hourChanged += OnHourChanged;

        healthComp.onDieCallback = TransformIntoCorpse;
        healthComp.onDamageEvent.AddListener(EnterPanic);
    }

    void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(PigState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public void OnDayChanged(object sender, int dayNumber)
    {
        CheckHungerState();
    }

    public void OnHourChanged(object sender, int hour)
    {
        if (hour == 3)
        {
            // At night, pig can die or grow depending on its hunger
            if (hasEatenToday)
            {
                Grow();
                hasEatenToday = false;
            }
            else if (starvingDaysCount >= STARVING_DAYS_TO_DIE)
            {
                healthComp.Kill();
            }
        }
        else if (hour == hungryHour)
        {
            // Pig gets hungry at this hour every day
            if (!hasEatenToday)
                isHungry = true;
        }
    }

    private void EnterPanic()
    {
        if (currentState == panicState) 
            panicState.RestartPanicTime();
        else
            ChangeState(panicState);
    }

    public void CheckHungerState()
    {
        if (!hasEatenToday) 
            starvingDaysCount++;
        else
            starvingDaysCount = 0;
    }

    private void Grow()
    {
        if (age >= MAX_AGE)
            return;
        
        age++;
        body.mass += MASS_INCREMENT_WITH_AGE;
        transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
    }

    public FoodContainerComponent GetFoodInFront()
    {
        FoodContainerComponent food;
        RaycastHit2D[] hits = new RaycastHit2D[2];
        Physics2D.Raycast(transform.position, facingDirection, new ContactFilter2D(), hits, FOOD_DETECTION_DISTANCE);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<FoodContainerComponent>(out food) && food.HasFood)
                return food;
        }

        return null;
    }

    public void TransformIntoCorpse()
    {
        ItemFactory.CreateCorpse(transform.position, BLOOD_BY_AGE[age], CorpseCreature.Pig, corpseSprite);

        Destroy(gameObject);
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }

    public void OnDestroy()
    {
        ChangeState(null);
        GameManager.Instance.dayChanged -= OnDayChanged;
        GameManager.Instance.hourChanged -= OnHourChanged;
    }
}
