using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStates : MonoBehaviour
{
    private Dictionary<Type, IBehavior> _behaviorsMap;
    private IBehavior _currentBehavior;

    private void Start()
    {
        this.InitBehaviors();
        this.SetBehaviorByDefault();
    }


    private void InitBehaviors()
    {
        this._behaviorsMap = new Dictionary<Type, IBehavior>();

        this._behaviorsMap[typeof(BehaviorIdle)] = new BehaviorIdle();

        this._behaviorsMap[typeof(BehaviorRangeCombat)] = new BehaviorRangeCombat();

        this._behaviorsMap[typeof(BehaviorMeleeCombat)] = new BehaviorMeleeCombat();

        this._behaviorsMap[typeof(BehaviorMoveToCover)] = new BehaviorMoveToCover();

        this._behaviorsMap[typeof(BehaviorFollowThePlayer)] = new BehaviorFollowThePlayer();

        this._behaviorsMap[typeof(BehaviorDeath)] = new BehaviorDeath();
    }


    private void SetBehavior(IBehavior _newBehavior)
    {
        if (this._currentBehavior != null)
            this._currentBehavior.Exit();

        this._currentBehavior = _newBehavior;
        this._currentBehavior.Enter();
    }


    private void SetBehaviorByDefault()
    {
        this.SetBehaviorIdle();
    }


    private IBehavior GetBehavior<T>() where T: IBehavior
    {
        var type = typeof(T);
        return this._behaviorsMap[type];
    }

    private void Update()
    {
        if (this._currentBehavior != null)
            this._currentBehavior.Update();
    }

    public void SetBehaviorIdle()
    {
        var behavior = this.GetBehavior<BehaviorIdle>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorRangeCombat()
    {
        var behavior = this.GetBehavior<BehaviorRangeCombat>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorMeleeCombat()
    {
        var behavior = this.GetBehavior<BehaviorMeleeCombat>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorFollowThePlayer()
    {
        var behavior = this.GetBehavior<BehaviorFollowThePlayer>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorMoveTocover()
    {
        var behavior = this.GetBehavior<BehaviorMoveToCover>();
        this.SetBehavior(behavior);
    }

    public void SetBehaviorDeath()
    {
        var behavior = this.GetBehavior<BehaviorDeath>();
        this.SetBehavior(behavior);
    }
}
