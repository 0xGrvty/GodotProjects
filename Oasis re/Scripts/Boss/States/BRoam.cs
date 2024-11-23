using Godot;
using System;

public partial class BRoam : State {
  [Export]
  private Boss b;
  public override void EnterState() {
  }

  public override void ExitState() {
  }

  public override void Update(double delta) {
    
  }

  public override void PhysicsUpdate(double delta) {
    if (!b.IsPlayerNear) EmitSignal(nameof(StateFinished), "Idle");
  }
}
