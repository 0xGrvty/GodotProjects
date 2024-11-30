using Godot;
using System;

public partial class PFall : State {
  [Export]
  private Player p;
  [Export]
  private AnimationPlayer ap;
  public override void EnterState() {
    ap.Play("Fall");
  }

  public override void ExitState() {

  }

  public override void Update(double delta) {
    p.PollInputs();
  }

  public override void PhysicsUpdate(double delta) {
    p.Move(delta);
    if (p.IsOnFloor()) {
      EmitSignal(new StringName(nameof(StateFinished)), this, "Idle");
    }
  }
}
