using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Boss : CharacterBody2D {
  // Seek radius to find the player
  [Export]
  private float seekRaidus = 400;
  [Export]
  private AnimationPlayer ap;

  [Export]
  private float slamGravity = 50.0f;
  
  private Sprite2D sprite;
  private CollisionShape2D hitbox;
  private BIdle bIdleState;
  private BRoam bRoamState;
  private StateMachine sm;
  private State currentState;
  private bool isPlayerNear = false;
  private Vector2 targetPos;

  public bool IsPlayerNear { get => isPlayerNear; }
  public AnimationPlayer AP { get => ap; }
  public Vector2 TargetPos { get => targetPos; }
  
  public override void _Ready() {
    sm = (StateMachine)GetNode<Node>("StateMachine");
    bIdleState = (BIdle)GetNode<Node>("StateMachine/Idle");
    bRoamState = (BRoam)GetNode<Node>("StateMachine/Roam");

    sprite = GetNode<Sprite2D>("Sprite2D");
    hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
    ap = GetNode<AnimationPlayer>("AnimationPlayer");

    currentState = sm.GetState();

  }

  public override void _Draw() {
    DrawArc(Vector2.Zero, seekRaidus, 0, Mathf.Pi * 2, 30, Colors.YellowGreen, 5);
  }

  public override void _Process(double delta) {
    QueueRedraw();
  }

  public override void _PhysicsProcess(double delta) {

  }

  private void OnArea2dBodyEntered(Node2D body) {
    if (body is Player) {
      targetPos = body.GlobalPosition;
      isPlayerNear = true;
    }
  }

  private void OnArea2dBodyExited(Node2D body) {
    if (body is Player) isPlayerNear = false;
  }

  public float GetSlamGravity() {
    return slamGravity;
  }
}