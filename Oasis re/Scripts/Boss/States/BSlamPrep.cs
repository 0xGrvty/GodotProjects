using Godot;
using System;

public partial class BSlamPrep : State {
  [Export]
  private Boss b;

  private Vector2 moveTowards;

  public override void _Ready() {
    
  }

  public override void EnterState() {
    moveTowards = b.GlobalPosition.DirectionTo(b.TargetPos - new Vector2(0, 500));

    var tween = GetTree().CreateTween();
    tween.TweenProperty(b, "global_position", b.TargetPos - new Vector2(0, 500), 0.35f);
    tween.Finished += OnSlamPrepFinished;
  }

  public override void ExitState() {
    
  }

  public override void Update(double delta) {
    
  }

  public override void PhysicsUpdate(double delta) {
  }

  private void OnSlamPrepFinished() {
    EmitSignal(SignalName.StateFinished, this, b.bSlam.Name);
  }
}
