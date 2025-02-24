using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public SpriteRenderer sprite;
    public IState initialState;
    public IState currentState;
    bool fadingEnemy = false;
    bool deactivated = false;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (fadingEnemy)
        {
            float currentAlpha = sprite.color.a;

            if (currentAlpha <= 0)
                Deactivate();
            else
            {
                Color spriteColor = sprite.color;
                sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, currentAlpha - Time.deltaTime);
            }
        }
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public void Spawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        gameObject.SetActive(true);

        ChangeState(initialState);

        fadingEnemy = false;
        deactivated = false;
        if (sprite != null)
            sprite.color = Color.white;
    }

    public void FadeAndDeactivate()
    {
        if (deactivated) return;

        deactivated = true;

        if (sprite != null && gameObject.activeSelf)
            fadingEnemy = true;
        else
            Deactivate();
    }

    public void Deactivate()
    {
        deactivated = true;
        gameObject.SetActive(false);
    }
}
