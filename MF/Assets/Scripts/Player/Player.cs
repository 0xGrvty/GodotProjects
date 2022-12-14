using Godot;
using Godot.Collections;
using System;

public class Player : KinematicBody2D {

#pragma warning disable 649
    [Export]
    public PackedScene hammerScene;
    [Export]
    private AnimatedSprite animations;
#pragma warning restore 649
    [Signal]
    private delegate void StateChanged();
    [Signal]
    private delegate void HealthChanged(int health);
    [Signal]
    private delegate void InitStats(int initMaxHealth);

    // Constants
    private float BASE_ATTACK_SPEED = 1f;
    private int BASE_MOVE_SPEED = 200;

    // Private variables
    public enum FaceDir {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3,
        UP_LEFT = 4,
        UP_RIGHT = 5
    };
    private int maxSpeed;
    private FaceDir facing = FaceDir.RIGHT;
    private Vector2 velocity = Vector2.Zero;
    public Vector2 screenSize = Vector2.Zero;
    private bool isMoving = false;
    private float attackCooldown;
    private float baseAttackCooldown;
    private bool isAttacking = false;
    private Health health;
    private Dictionary playerStats = new Dictionary();

    // State Machine
    private IStateMachine currentState;
    public PlayerIdleState playerIdleState = new PlayerIdleState();
    public PlayerRunState playerRunState = new PlayerRunState();
    public PlayerAttackState playerAttackState = new PlayerAttackState();

    // Getters/Setters
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    public override void _Ready() {
        Position = GetNode<Position2D>("../StartPosition").Position;
        screenSize = GetViewportRect().Size;

        currentState = playerIdleState;
        maxSpeed = BASE_MOVE_SPEED;
        animations = GetNode<AnimatedSprite>("AnimatedSprite");
        baseAttackCooldown = (14f / 12);
        attackCooldown = baseAttackCooldown;
        Connect("StateChanged", this, "OnStateChanged");
        health = (Health)GetNode<Node>("Health");
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventKey e) {
            //if (e.IsActionPressed("BlessedHammer")) {
            //    currentState = playerAttackState;
            //}
        }
    }

    public override void _PhysicsProcess(float delta) {
        //DoMovement();
        velocity = Vector2.Zero;
        isMoving = GetMovementInput() != Vector2.Zero;
        currentState = currentState.EnterState(this);
        Update();
    }

    public override void _Draw() {
        DrawLine(new Vector2().Rotated(-Rotation), velocity, Color.ColorN("blue"), 5.0f);
    }

    private Vector2 GetMovementInput() {
        if (Input.IsActionPressed("Up")) {
            velocity += Vector2.Up;
            facing = FaceDir.UP;
        }
        if (Input.IsActionPressed("Down")) {
            velocity += Vector2.Down;
            facing = FaceDir.DOWN;
        }
        if (Input.IsActionPressed("Left")) {
            velocity += Vector2.Left;
            facing = (velocity.y < 0) ? FaceDir.UP_LEFT : FaceDir.LEFT;
        }
        if (Input.IsActionPressed("Right")) {
            velocity += Vector2.Right;
            facing = (velocity.y < 0) ? FaceDir.UP_RIGHT : FaceDir.RIGHT;
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
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, screenSize.x),
            y: Mathf.Clamp(Position.y, 0, screenSize.y)
            );
    }

    public AnimatedSprite GetAnimatedSprite() {
        return animations;
    }

    private void OnStateChanged(String state) {
        Console.WriteLine("Current State: " + state);
    }

    // A ton of getters/setters, we might not use all of them
    // But they seem relevant for a rougelike game
    // Where your stats could be altered with
    public float GetAttackCooldown() {
        return attackCooldown;
    }

    public void ResetAttackCooldown() {
        isAttacking = false;
        attackCooldown = baseAttackCooldown;
    }

    public float GetBaseAttackCooldown() {
        return baseAttackCooldown;
    }
    public void SetBaseAttackCooldown(float cooldown) {
        baseAttackCooldown = cooldown;
    }
    public float UpdateAttackCooldown() {
        attackCooldown -= GetPhysicsProcessDeltaTime();
        return attackCooldown;
    }

    public FaceDir GetFacing() {
        return facing;
    }

    public void SetFacing(FaceDir direction) {
        facing = direction;
    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    public void SetPlayerVelocity(Vector2 velocity) {
        this.velocity = velocity;
    }

    public void UpdateVelocity(Vector2 update) {
        velocity += update;
    }

    public int GetMaxSpeed() {
        return maxSpeed;
    }

    public void SetMaxSpeed(int speed) {
        maxSpeed = speed;
    }

    public Health GetHealth() {
        return health;
    }
}
