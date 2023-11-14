using Godot;
using System;

public abstract partial class AttackState : Node {
    private Attack attack;
    private int activeFrame = 0;

    public void InitAttack(Node hitboxGroup, int activeFrame = 0) {
        this.activeFrame = activeFrame;
    }

    public IStateMachine ChangeState() {
        throw new NotImplementedException();
    }

    public Attack GetAttack() { return attack; }
}
