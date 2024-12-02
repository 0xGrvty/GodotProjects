using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Boss : Area2D {
  // Seek radius to find the player
  [Export]
  private float seekRaidus = 400.0f;
  [Export]
  private AnimationPlayer ap;

  // Default value: 800
  [Export]
  private float slamGravity = 800.0f;

  // Default value: 800
  [Export]
  private float slamPrepSpeed = 800.0f;
  // Default value: 400
  [Export]
  private float moveSpeed = 400.0f;
  
  private Sprite2D sprite;
  private CollisionShape2D hitbox;

  // State Machine
  public BIdle bIdle;
  public BRoam bRoam;
  public BSlam bSlam;
  public BSlamPrep bSlamPrep;
  private StateMachine sm;


  public bool isPlayerNear = false;
  public Vector2 targetPos;
  public Vector2 velocity;

  public bool IsPlayerNear { get => isPlayerNear; }
  public AnimationPlayer AP { get => ap; }
  public Vector2 TargetPos { get => targetPos; }
  public float SlamPrepSpeed { get => slamPrepSpeed; }
  public float SlamGravity { get => slamGravity; }
  public Vector2 Velocity { get => velocity; set => velocity = value; }
  public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
  
  public override void _Ready() {
    sm = (StateMachine)GetNode<Node>("StateMachine");
    bIdle = (BIdle)GetNode<Node>("StateMachine/Idle");
    bRoam = (BRoam)GetNode<Node>("StateMachine/Roam");
    bSlam = (BSlam)GetNode<Node>("StateMachine/Slam");
    bSlamPrep = (BSlamPrep)GetNode<Node>("StateMachine/SlamPrep");

    sprite = GetNode<Sprite2D>("Sprite2D");
    hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
    ap = GetNode<AnimationPlayer>("AnimationPlayer");

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
}
