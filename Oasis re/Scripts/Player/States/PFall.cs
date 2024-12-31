using Godot;
using System;

public partial class PFall : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    p.BgAP.Play("Fall");
    p.FgAP.Play("Fall");
  }

  public override void ExitState() {
    p.prevState = this;
  }

  public override void Update(double delta) {
    p.PollInputs();
    
  }

  public override void PhysicsUpdate(double delta) {
    p.Move(delta);
    
    if (p.IsOnFloor() && p.Dir == Direction.NO_DIR) EmitSignal(SignalName.StateFinished, this, p.pIdle.Name);
    else if (p.IsOnFloor() && p.Dir != Direction.NO_DIR) EmitSignal(SignalName.StateFinished, this, p.pRun.Name);
    else if (p.IsAttacking) EmitSignal(SignalName.StateFinished, this, p.pAttack.Name);
  }
}
