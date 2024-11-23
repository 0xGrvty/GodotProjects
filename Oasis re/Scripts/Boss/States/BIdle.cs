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
    if (b.IsPlayerNear) EmitSignal(new StringName(nameof(StateFinished)), this, "Slam");
  }
}
