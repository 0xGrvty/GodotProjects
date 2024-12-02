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
    p.Jump(delta);
    p.Move(delta);

    if (p.Velocity.Y > 0 && !p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pFall.Name);
    else if (p.IsJumping) EmitSignal(SignalName.StateFinished, this, p.pJump.Name);
    else if (p.IsAttacking) EmitSignal(SignalName.StateFinished, this, p.pAttack.Name);
    
  }
}
