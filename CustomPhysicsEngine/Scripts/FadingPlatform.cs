using Godot;
using System;

public partial class FadingPlatform : Solid
{
    [Export]
    private float onTime = 1;
    [Export]
    private float offTime = 3;

    private bool fallThru = false;
    private Tween tween;

    public override void _Ready() {
        Hitbox = (Hitbox)GetNode<Node2D>("Hitbox");
        AddToGroup("Walls");
        InitTween();
    }

    private void InitTween() {
        tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Linear);
        tween.TweenProperty(this, "fallThru", false, onTime);
        tween.TweenProperty(this, "fallThru", true, offTime);
        tween.Connect("step_finished", new Callable(this, nameof(OnTweenStep)));
    }

    private void OnTweenStep(int idx) {
        Hitbox.Collidable = fallThru;
    }
}
