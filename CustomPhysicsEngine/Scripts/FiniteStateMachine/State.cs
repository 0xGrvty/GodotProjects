using Godot;
using System;

public abstract partial class State : Node {

    [Signal]
    public delegate void StateFinishedEventHandler(State state, String newState);
    [Signal]
    public delegate void OnAttackEventHandler(State state);
    [Signal]
    public delegate void AttackShiftEventHandler(State state, float weight, float duration);


    // Each state could do something different, so make these functions overridable
    public virtual void EnterState() {

    }

    public virtual void ExitState() {

    }

    public virtual void Update(double delta) {
        
    }

    public virtual void PhysicsUpdate(double delta) {

    }

}
