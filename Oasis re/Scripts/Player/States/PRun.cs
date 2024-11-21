using Godot;
using System;

public partial class PRun : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    ap.Play("Run");
  }

  public override void ExitState() {
  }

  public override void HandleInput(InputEvent e) {
    if (e.IsActionPressed("Jump")) p.IsJumping = true;
    else if (e.IsActionPressed("Attack")) p.IsAttacking = true;
  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Move();

    if (p.Dir == Direction.NO_DIR) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Idle");
    }

    if (p.IsJumping) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Jump");
    }

    if (p.IsAttacking) EmitSignal(new StringName(nameof(StateFinished)), this, "Attack");
  }
}
