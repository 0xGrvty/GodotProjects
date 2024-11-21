using Godot;
using System;

public partial class PJump : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    ap.Play("Jump");
    p.IsJumping = false;
  }

  public override void ExitState() {

  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Jump();
    p.Move();

    if (p.Velocity.Y > 0 && !p.IsOnFloor()) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Fall");
    }
    if (p.IsJumping) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Jump");
    }
  }
}
