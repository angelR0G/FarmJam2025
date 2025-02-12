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

    // Components
    [Header("Components References")]
    public Rigidbody2D body;

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

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();

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
                // TODO: Pig grows
                Grow();
                hasEatenToday = false;
            }
            else if (starvingDaysCount >= STARVING_DAYS_TO_DIE)
            {
                // TODO: Pig dies, leaving its corpse
                Destroy(gameObject);
            }
        }
        else if (hour == hungryHour)
        {
            // Pig gets hungry at this hour every day
            if (!hasEatenToday)
                isHungry = true;
        }
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

    public void Eat(PlayerComponent p)
    {
        isHungry = false;
        hasEatenToday = true;
    }

    public FoodContainerComponent GetFoodInFront()
    {
        FoodContainerComponent food;
        RaycastHit2D[] hits = new RaycastHit2D[2];
        Physics2D.Raycast(transform.position, facingDirection, new ContactFilter2D(), hits, FOOD_DETECTION_DISTANCE);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<FoodContainerComponent>(out food))
                return food;
        }

        return null;
    }
}
