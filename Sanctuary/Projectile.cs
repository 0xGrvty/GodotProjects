using Godot;
using System;

public partial class Projectile : Area2D {
    private Vector2 velocity = Vector2.Zero;
    private Vector2 range = new Vector2(200, 200);
    private Vector2 start = Vector2.Zero;
    private float speed = 200;
    private float lifetime = 0.5f;

    public override void _Ready() {
        start = GlobalPosition;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Linear).SetLoops().SetParallel();
        tween.Connect("loop_finished", new Callable(this, MethodName.HandleLoopFinished));
        tween.TweenProperty(this, "rotation", 4 * Mathf.Pi, lifetime);
        tween.TweenProperty(this, "global_position", GlobalPosition + range, lifetime).SetTrans(Tween.TransitionType.Quad);
        tween.Chain().TweenProperty(this, "rotation", 2 * Mathf.Pi, lifetime).From(0f);
        tween.TweenProperty(this, "global_position", start, lifetime).SetTrans(Tween.TransitionType.Quad);
    }

    public override void _PhysicsProcess(double delta) {
    }

    public void HandleLoopFinished(int loopCount) {
        QueueFree();
    }

    public void Init(Vector2 pos, Vector2 dir) {
        GlobalPosition = pos;
        range = speed * dir;
    }

}
