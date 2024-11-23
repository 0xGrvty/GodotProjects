using Godot;
using System;

public partial class BSlam : State {
  [Export]
  private Boss b;
  private Vector2 moveTo;

  public override void _Ready() {

  }
  public override void EnterState() {
    // Play slam animation
    // var initTween = GetTree().CreateTween().SetParallel(false);
    // initTween.TweenProperty(b, "global_position", b.TargetPos - new Vector2(0, 500), 3.0f);
    // initTween.TweenInterval(3.0f);
    b.GlobalPosition = b.TargetPos - new Vector2(0, 500);
    var tween = GetTree().CreateTween().SetLoops(3);
    tween.TweenProperty(b, "global_position:y", b.GlobalPosition.Y + 10, 0.05f);
    tween.TweenProperty(b, "global_position:y", b.GlobalPosition.Y - 10, 0.05f);
    
    tween.Finished += OnShakeFinished;
  }

  public override void ExitState() {
  }

  public override void Update(double delta) {

  }

  public override void PhysicsUpdate(double delta) {
    b.MoveAndSlide();
  }

  private void OnShakeFinished() {
    var tween = GetTree().CreateTween();
    tween.TweenProperty(b, "velocity:y", b.GetSlamGravity(), 0.15f).SetDelay(0.5f);
  }
}
