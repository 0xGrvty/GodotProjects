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

    // Because of how our scene tree is laid out, the Player's process function fires *before* this one
    // which causes the platform to clip the player when moving in upwards directions.
    // If we can think of a way to fix this, we will implement it.
    public override void _Process(double delta) {
        MoveY(follow.Y - (GlobalPosition.Y + remainder.Y));
        MoveX(follow.X - (GlobalPosition.X + remainder.X));
    }

    private void InitTween()
    {
        tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this, "follow", start + offset, time).SetDelay(delay);
        tween.TweenProperty(this, "follow", start, time).SetDelay(delay);
    }

}
