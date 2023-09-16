using Godot;
using System;

public partial class Enemy : Actor {
    private Vector2 velocity = Vector2.Zero;
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    [Export]
    private float maxSpeed = 100;
    [Export]
    private float maxAccel = 800;
    private bool onGround = true;

    [Export]
    private float jumpHeight = 10f;
    [Export]
    private float jumpTimeToPeak = 0.4f;
    [Export]
    private float jumpTimeToDescent = 0.2f;
    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // Remove this later, just needed it for testing.
        // A Bat or Slime will be instantiated, but not an Enemy
        Hurtbox = (Hitbox)GetNode<Node2D>("Hitbox");

        // Since C# does not have onready, we still need to fetch the globals.
        // It is because GetTree is not a static method, it seems like.  I wonder if I am doing something incorrectly.
        // The documentation said we should be able to just call Game, but it did not work. <-- It didn't work because you Autoloaded the script, dummy.  Autoload the scene next time, you friggin' noob
        GM = GetNode<Game>("/root/Game");

        // remove this later, just needed it for instance variables
        jumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f; // In Godot 2D, down is positive, so flip the signs
        jumpGravity = ((-2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0f;
        fallGravity = ((-2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) * -1.0f;

        AddToGroup("Actors");
    }

    public override void _Process(double delta) {
        onGround = GM.CheckWallsCollision(this, Vector2.Down);
        velocity.X = Mathf.MoveToward(velocity.X, 0, maxAccel * (float)delta);
        velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);

        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
    }

    public float GetGravity() {
        return velocity.Y < 0.0 ? jumpGravity : fallGravity;
    }

    public void OnCollisionX() {
        velocity.X = 0;
        ZeroRemainderX();
    }

    public void OnCollisionY() {
        velocity.Y = 0;
        ZeroRemainderY();
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetMaxAccel() { 
        return maxAccel; 
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }
}
