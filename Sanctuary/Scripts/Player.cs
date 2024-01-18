using Godot;
using Godot.Collections;
using System;


// Need to overhaul the Player script.  Wanted to reuse the AABB collisions, but started to realize
// it wouldn't work with this type of game.
// Would need to implement SAT collision detection or GJK algorithm
// which I believe Godot already does to check for collisions between bodies, so let's just use that.
// In the future, I would like to research these algorithms and implement them on my own to fully understand
// exactly how they work.
public partial class Player : CharacterBody2D {
    // May be used
    [Signal]
    public delegate void MoveEventHandler();
    

    // Private variables
    private float maxSpeed = 200;
    private float maxAccel = 800;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 moveTo = Vector2.Zero;
    private Vector2 attackDir = Vector2.Zero;
    private bool isMoving = false;
    private int projCount = 1;
    private bool isAttacking = false;
    private PackedScene projScene;

    // Public variables
    public FiniteStateMachine fsm;
    public PlayerRunState runState;
    public PlayerIdleState idleState;
    public PlayerAttackState attackState;
    public Dictionary<string, PackedScene> scenes;

    public override void _Ready() {
        // Instantiate and load packed scenes and put them in a dictionary for later use
        projScene = GD.Load<PackedScene>(Globals.PROJ_SCENE_PATH);
        scenes = new Dictionary<string, PackedScene> {
            { Scenes.PROJ_SCENE, projScene }
        };

        fsm = (FiniteStateMachine)GetNode<Node>("StateMachine");
        runState = (PlayerRunState)GetNode<Node>("StateMachine/Run");
        idleState = (PlayerIdleState)GetNode<Node>("StateMachine/Idle");
        attackState = (PlayerAttackState)GetNode<Node>("StateMachine/Attack");

    }
    public override void _Process(double delta) {
    }
    public override void _PhysicsProcess(double delta) {
    }

    public bool GetIsMoving() {
        return isMoving;
    }

    public void SetIsMoving(bool isMoving) {
        this.isMoving = isMoving;
    }

    public bool GetIsAttacking() {
        return isAttacking;
    }

    public void SetIsAttacking(bool isAttacking) {
        this.isAttacking = isAttacking;
    }

    public Vector2 GetMoveTo() {
        return moveTo;
    }

    public void SetMoveTo(Vector2 to) {
        moveTo = to;
    }

    public Vector2 GetAttackDir() {
        return attackDir;
    }

    public void SetAttackDir(Vector2 dir) {
        attackDir = dir;
    }

    public PackedScene GetScene(StringName scene) {
        return scenes[scene];
    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    public void SetVelocity(Vector2 velocity) { 
        this.velocity = velocity;
    }

}
