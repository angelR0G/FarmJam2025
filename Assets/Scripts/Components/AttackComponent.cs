using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Animator attackAnim;
    public BoxCollider2D damageArea = null;
    public int damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        damageArea.enabled = false;
        damageArea.isTrigger = true;

        if (sprite != null)
            sprite.enabled = false;
    }

    private void LateUpdate()
    {
        if (sprite != null && sprite.enabled)
        {
            if (attackAnim != null && attackAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                StopAnimation();
        }
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == gameObject || other.isTrigger) return;

        HealthComponent damagedObjectHealth; 
        if (!other.gameObject.TryGetComponent<HealthComponent>(out damagedObjectHealth))
            return;

        damagedObjectHealth.LooseHealth(damage);

        Rigidbody2D damagedObjectBody;
        if (other.TryGetComponent<Rigidbody2D>(out damagedObjectBody))
        {
            Vector3 forceDirection = (other.gameObject.transform.position - transform.position).normalized;
            damagedObjectBody.AddForce(new Vector2(forceDirection.x, forceDirection.y) * 10 * damagedObjectBody.mass, ForceMode2D.Impulse);
        }
    }

    public void ConfigureSprite(Vector3 spriteOffset = new Vector3(), bool flipH = false, bool flipV = false)
    {
        if (sprite == null) return;

        sprite.gameObject.transform.localPosition = spriteOffset;
        sprite.flipX = flipH;
        sprite.flipY = flipV;
    }

    public void PlayAnimation(string animationName)
    {
        if (sprite == null || attackAnim == null) return;

        sprite.enabled = true;
        attackAnim.Play(animationName, 0, 0);
    }

    public void StopAnimation()
    {
        if (sprite == null) return;

        sprite.enabled = false;
    }
}
