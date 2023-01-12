using Godot;
using System;
using System.Runtime.CompilerServices;

public class Player : KinematicBody2D
{
    private Vector2 screenSize = Vector2.Zero;
    private Vector2 velocity = Vector2.Zero;
    private float maxSpeed = 20.0f;
    bool isMoving = false;

    public override void _PhysicsProcess(float delta) {
        DoMovement();
        Update();
    }

    public override void _Ready() {
        maxSpeed = 200.0f;
        screenSize = GetViewportRect().Size;
    }

    public override void _Draw() {
        DrawLine(Vector2.Zero, velocity, Colors.Blue, 5.0f);
    }

    private Vector2 GetMovementInput() {
        if (Input.IsActionPressed("Up")) {
            velocity += Vector2.Up;
        }
        if (Input.IsActionPressed("Down")) {
            velocity += Vector2.Down;
        }
        if (Input.IsActionPressed("Left")) {
            velocity += Vector2.Left;
        }
        if (Input.IsActionPressed("Right")) {
            velocity += Vector2.Right;
        }
        return velocity;
    }

    public void DoMovement() {
        velocity = Vector2.Zero;
        velocity = GetMovementInput().Normalized() * maxSpeed; // GetMovementInput() returns velocity
        isMoving = velocity != Vector2.Zero; // Check to see if velocity is not zero
        MoveAndSlide(velocity);
        //Position += velocity * GetPhysicsProcessDeltaTime();
        //Velocity * GetPhysicsProcessDeltaTime();
        //Velocity = Vector2.Zero;
        //Position = new Vector2(
        //    x: Mathf.Clamp(Position.x, 0, screenSize.x),
        //    y: Mathf.Clamp(Position.y, 0, screenSize.y)
        //    );
    }
}
