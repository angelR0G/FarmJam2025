using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

public class StalkerEnemyComponent : MonoBehaviour
{
    // Components
    [Header("Components")]
    public HealthComponent healthComp;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public CircleCollider2D enemyCollider;

    // States
    [Header("State Machine")]
    [SerializeField, Tooltip("Game Object that contains enemy's states. Should be a child of enemy game object so that states can update their references correctly.")]
    private GameObject statesContainer = null;
    public StalkerState currentState = null;
    [HideInInspector] public StalkerWanderState wanderState = null;
    [HideInInspector] public StalkerStalkState stalkState = null;
    [HideInInspector] public StalkerChaseState chaseState = null;
    [HideInInspector] public StalkerAttackState attackState = null;
    [HideInInspector] public StalkerRecoverState recoverState = null;

    // Other properties
    [Header("Enemy properties")]
    public GameObject attackTarget;
    public Sprite corpseSprite;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        healthComp = GetComponent<HealthComponent>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();

        // Init enemy states
        wanderState = statesContainer.GetComponent<StalkerWanderState>();
        stalkState = statesContainer.GetComponent<StalkerStalkState>();
        chaseState = statesContainer.GetComponent<StalkerChaseState>();
        attackState = statesContainer.GetComponent<StalkerAttackState>();
        recoverState = statesContainer.GetComponent<StalkerRecoverState>();

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
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public bool IsGameObjectInSight(GameObject obj, float maxDistance = -1)
    {
        if (obj == null) return false;

        Vector2 vectorToObject = obj.transform.position - transform.position;

        if (maxDistance > 0 && vectorToObject.magnitude > maxDistance) return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorToObject.normalized, vectorToObject.magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == gameObject || hit.collider.gameObject == obj.gameObject) continue;

            // Another object's collision is in the line of sight
            return false;
        }

        return true;
    }

    public Vector3 GetAvoidanceDirection(Vector3 targetDirection, float rayDistance, List<GameObject> ignoreObjects)
    {
        Vector3 leftAvoid = Vector3.zero;
        Vector3 rightAvoid = Vector3.zero;

        // Left raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, 30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoreObjects.Contains(hit.collider.gameObject)) continue;

            leftAvoid += Quaternion.Euler(0, 0, -90) * targetDirection * 1f * (1 - (hit.distance/rayDistance));
        }

        // Right raycast
        hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, -30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hit.collider.isTrigger || ignoreObjects.Contains(hit.collider.gameObject)) continue;

            rightAvoid += Quaternion.Euler(0, 0, 90) * targetDirection * 1f * (1 - (hit.distance / rayDistance));
        }

        return leftAvoid.sqrMagnitude >= rightAvoid.sqrMagnitude ? leftAvoid : rightAvoid;
    }

    private void OnDie()
    {
        ItemFactory.CreateCorpse(transform.position, 100, corpseSprite);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == attackState)
            attackState.FinishAttack();
    }
}
