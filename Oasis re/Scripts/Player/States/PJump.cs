using Godot;
using System;

public partial class PJump : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    p.BgAP.Play("Jump");
    if (p.Velocity == Vector2.Zero && p.Dir == Direction.NO_DIR) p.FgAP.Play("JumpNeutral");
    else p.FgAP.Play("Jump");
    
    p.IsJumping = false;
  }

  public override void ExitState() {
    p.prevState = this;
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
