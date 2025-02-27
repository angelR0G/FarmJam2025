using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public SpriteRenderer sprite;
    public IState initialState;
    public IState currentState;
    protected bool deactivated = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
    }

    public void ChangeState(IState newState)
    {
        if (deactivated) return;

        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public void Spawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        
        deactivated = false;

        gameObject.SetActive(true);
        sprite.DOFade(1f, 1f);

        ChangeState(initialState);
    }

    public void Deactivate(bool fade = true)
    {
        if (!gameObject.activeSelf) return;

        deactivated = true;

        if (fade)
        {
            Sequence enemyDisappear = DOTween.Sequence();

            enemyDisappear.Append(sprite.DOFade(0f, 1f))
                .AppendCallback(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
