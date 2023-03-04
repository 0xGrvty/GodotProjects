using Godot;
using Godot.Collections;
using System;

public class EnemyExplode : Node2D
{
    Node2D source;
    int damage;
    Dictionary upgrades;
    AnimatedSprite animation;
    public override void _Ready() {
        
    }

    public override void _PhysicsProcess(float delta) {
        if (animation.Frame >= animation.Frames.GetFrameCount(animation.Animation) - 1) {
            QueueFree();
        }
    }

    public void Init(Node2D source, Dictionary upgrades = null) {
        this.source = source;
        Position = source.Position;
        this.upgrades = upgrades.Duplicate(true);
        animation = GetNode<AnimatedSprite>("AnimatedSprite");
        animation.SpeedScale = 1;
        animation.Frame = 0;
        animation.Play();
        damage = 30;
    }

    public void OnExplosionBodyEntered(Node2D body) {
        if (body is Enemy) {
            body.EmitSignal("Hit", damage, source, upgrades);
        }
    }
}
