using Godot;
using System;

public partial class PlayerAttackState2 : State {

    [Export]
    private Player actor;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Attack2");
    }

    public override void PhysicsUpdate(double delta) {
        actor.DoAttack();
    }
    public void ChangeState() {
        if (actor.GetInputBufferContents().Contains(1)) {
            EmitSignal(nameof(StateFinished), this, "Attack3");
        } else {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }
    }
}
