using Godot;
using System;

public partial class MovingPlatform : Solid
{
    private Tween tween;
    private Vector2 start;
    private Vector2 follow;

    [Export]
    Vector2 offset = Vector2.Zero;
    [Export]
    float time = 2;
    [Export]
    float delay = 1;
    public override void _Ready()
    {
        Hitbox = (Hitbox)GetNode<Node2D>("Hitbox");
        GM = GetNode<Game>("/root/Game");
        start = GlobalPosition;
        follow = GlobalPosition;
        AddToGroup("Walls");
        InitTween();
    }

    public override void _Process(double delta) {
        // Should there be a way to fire this off via signals
        MoveY(follow.Y - (GlobalPosition.Y + remainder.Y));
        MoveX(follow.X - (GlobalPosition.X + remainder.X));
    }

    private void InitTween()
    {
        tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
        //tween.Connect(Tween.SignalName.StepFinished, new Callable(this, nameof(OnTweenStep)));
        tween.TweenProperty(this, "follow", start + offset, time).SetDelay(delay);
        tween.TweenProperty(this, "follow", start, time).SetDelay(delay);

        //tween.Play();
    }

    // I couldn't get this to work, from Godot 3.5 -> 4.0, tween_step changed to step_finished,
    // however step_finished fires when the end of a tweener (tween_property) is completed rather than
    // when the end of a step of the tween happens.
    private void OnTweenStep(int idx) {
        MoveY(follow.Y - (GlobalPosition.Y + remainder.Y));
        MoveX(follow.X - (GlobalPosition.X + remainder.X));
    }
}
