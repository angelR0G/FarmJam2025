using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigEatingState : PigState
{
    FoodContainerComponent foodSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void EnterState()
    {
        foodSource = pig.GetFoodInFront();

        if (foodSource == null)
            pig.ChangeState(pig.walkingState);
        else
        {
            pig.transform.localScale = Vector3.one * 0.1f;
        }
    }
}
