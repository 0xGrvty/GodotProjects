using Godot;
using System;

public partial class PFall : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    // ap.Play("Fall");
    p.TestAnimsArms.Play("Fall");
    p.TestAnimsTorso.Play("Fall");
    p.TestAnimsLegs.Play("Fall");
  }

  public override void ExitState() {

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
