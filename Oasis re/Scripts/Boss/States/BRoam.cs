using Godot;
using System;

public partial class BRoam : State {
  [Export]
  private Boss b;
  public override void EnterState() {
    var tween = GetTree().CreateTween().SetLoops(2);
    tween.TweenProperty(b, "Velocity", new Vector2(1, -1f), 0.5f);
    tween.TweenProperty(b, "Velocity", new Vector2(0, 1), 0.5f);
    tween.TweenProperty(b, "Velocity", new Vector2(-1f, -1f), 0.5f);
    tween.TweenProperty(b, "Velocity", new Vector2(0, 1), 0.5f);

    tween.Finished += OnRoamFinished;
  }

  public override void ExitState() {
  }

  public override void Update(double delta) {
    
  }

  public override void PhysicsUpdate(double delta) {
    b.GlobalPosition += b.Velocity * b.MoveSpeed * (float)delta;
  }

  private void OnRoamFinished() {
    EmitSignal(SignalName.StateFinished, this, "Idle");
  }
}
