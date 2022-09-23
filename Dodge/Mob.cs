using Godot;
using System;

public class Mob : RigidBody2D {

    [Signal]
    public delegate void Died();
    public override void _Ready() {
        var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animSprite.Playing = true;
        string[] mobTypes = animSprite.Frames.GetAnimationNames();
        animSprite.Animation = mobTypes[GD.Randi() % mobTypes.Length];
    }

    public void OnVisibilityNotifier2DScreenExited() {
        QueueFree();
    }

    public void Die() {
        // I just died, so announce it to the game manager
        EmitSignal(nameof(Died));
        QueueFree();
    }
}
