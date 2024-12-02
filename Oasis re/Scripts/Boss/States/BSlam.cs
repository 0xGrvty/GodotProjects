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
  }

  private void OnShakeFinished() {
    var tween = GetTree().CreateTween();
    tween.TweenProperty(b, "global_position:y", b.TargetPos.Y, 0.10f).SetDelay(0.5f);
    tween.TweenCallback(new Callable(this, MethodName.RequestStateFinished)).SetDelay(1.0f);
  }

  private void RequestStateFinished() {
    EmitSignal(SignalName.StateFinished, this, b.bIdle.Name);
  }
}
