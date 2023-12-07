using Godot;
using System;

public partial class PlayerAttackState2 : State, IAttackState {

    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Attack2");
        EmitSignal(nameof(OnAttack), this);
    }

    public override void ExitState() {
        p.AttackInputBuffer.ClearBuffer();
    }

    public override void PhysicsUpdate(double delta) {
        p.DoAttack();
    }
    public void ChangeState() {
        if (p.GetInputBufferContents().Contains((int)InputBuffer.BUTTON.ATTACK)) {
            EmitSignal(nameof(StateFinished), this, "Attack3");
        } else {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }
    }
}
