using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public BoxCollider2D damageArea = null;
    public int damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        damageArea.enabled = false;
        damageArea.isTrigger = true;
    }

    public void UpdateDamageArea(float attackRange, float attackWidth, float attackAngle)
    {
        damageArea.size = new Vector2(attackRange, attackWidth);
        damageArea.offset = new Vector2(attackRange/2, 0);
        damageArea.gameObject.transform.rotation = Quaternion.Euler(0, 0, attackAngle);
    }

    public void SetDamageAreaActive(bool newState)
    {
        damageArea.enabled = newState;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == gameObject) return;

        HealthComponent damagedObjectHealth = other.gameObject.GetComponent<HealthComponent>();
        damagedObjectHealth?.LooseHealth(damage);
    }
}
