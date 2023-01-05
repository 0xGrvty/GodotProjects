using Godot;
using System;

public abstract class  Enemy : KinematicBody2D
{

    private int health;
    private float moveSpeed;
    private Vector2 velocity;
    private Vector2 target;

    public void Spawn(Vector2 target) {
        this.target = target;
        health = 100;
        moveSpeed = 100;
        velocity = Vector2.Zero;
        velocity = (target - Position).Normalized() * moveSpeed;
    }

    public override void _PhysicsProcess(float delta) {
        MoveAndSlide(velocity);
    }

}
