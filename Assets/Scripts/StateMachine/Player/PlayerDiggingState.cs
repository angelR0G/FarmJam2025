using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiggingState : MonoBehaviour, IState
{
    private PlayerComponent player;
    [SerializeField]
    private GameObject plotPrefab;

    public void Awake()
    {
        player = GetComponentInParent<PlayerComponent>();
    }

    public void EnterState() 
    { 
        if (HasSpaceToDig())
        {
            Instantiate(plotPrefab);
        }

        player.ChangeState(player.walkingState);
    }

    public void ExitState() { }

    public void UpdateState() { }

    public void FixedUpdateState() { }

    private bool HasSpaceToDig()
    {
        return true;
    }
}
