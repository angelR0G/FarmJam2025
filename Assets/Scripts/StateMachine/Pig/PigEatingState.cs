using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigEatingState : PigState
{
    private const float EATING_TIME = 8f;

    FoodContainerComponent foodSource = null;
    private float remainingEatingTime = 0;

    public override void EnterState()
    {
        foodSource = pig.GetFoodInFront();

        if (foodSource == null)
            pig.ChangeState(pig.walkingState);
        else
        {
            remainingEatingTime = EATING_TIME;

            pig.animator.SetTrigger("Eating");
        }
    }

    public override void UpdateState()
    {
        if (foodSource == null || !foodSource.HasFood)
        {
            pig.ChangeState(pig.walkingState);
        }

        if (remainingEatingTime <= 0)
            FinishEating();
        else
            remainingEatingTime -= Time.deltaTime;
    }

    private void FinishEating()
    {
        if (foodSource != null)
        {
            foodSource.EmptyContainer();
            pig.isHungry = false;
            pig.hasEatenToday = true;
        }

        pig.ChangeState(pig.walkingState);
    }
}
