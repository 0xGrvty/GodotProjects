using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BIdle : State {
  [Export]
  private Boss b;

  public override void _Ready() {
  }
  public override void EnterState() {
    // Play idle animation
  }

  public override void ExitState() {
  }

  public override void Update(double delta) {
    
  }

  public override void PhysicsUpdate(double delta) {
    if (b.IsPlayerNear) {
      var rand = GD.Randf();
      if (rand <= 0.5f) EmitSignal(SignalName.StateFinished, this, "Roam");
      else EmitSignal(SignalName.StateFinished, this, "SlamPrep");
    }
  }
}
