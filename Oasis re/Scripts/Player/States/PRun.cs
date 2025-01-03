using Godot;
using System;

public partial class PRun : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    p.FgAP.Play("Run");
    
    p.BgAP.Play("Run");
    p.BgAP.Seek(p.FgAP.CurrentAnimationPosition, true);
    
  }

  public override void ExitState() {
    p.prevState = this;
  }
  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Move(delta);

    if (p.Dir == Direction.NO_DIR) EmitSignal(SignalName.StateFinished, this, p.pIdle.Name);
    else if (p.IsJumping) EmitSignal(SignalName.StateFinished, this, p.pJump.Name);
    else if (p.IsAttacking) EmitSignal(SignalName.StateFinished, this, p.pAttack.Name);
    else if (!p.IsOnFloor()) EmitSignal(SignalName.StateFinished, this, p.pFall.Name);
  }
}
