using Godot;
using System;

public class Bullet : Area2D
{
    public int Speed = 300; // Max bullet speed

    private Vector2 Velocity = Vector2.Zero;

    public void OnBulletBodyEntered(Node2D body) {
        // Delete yourself NOW
        QueueFree();

        // Check to see if the body we hit is in the "mobs" group
        // since I don't want to destroy powerups, for example
        if (body.IsInGroup("mobs")) {

            // Call this function at the END of the CURRENT
            // physics step.  This ensures that it's SAFE
            // to do so.
            body.CallDeferred("Die");
        }
    }

    public void Init(Vector2 velocity) {
        Velocity = velocity;
    }

    public override void _Process(float delta) {
        Velocity =  Velocity.Normalized() * Speed;

        Position += Velocity * delta;
    }

    public void OnVisibilityNotifier2DScreenExited() {
        QueueFree();
    }
}
