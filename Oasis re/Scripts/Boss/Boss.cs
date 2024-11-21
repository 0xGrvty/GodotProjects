using Godot;
using System;

public partial class Boss : CharacterBody2D {
  private Sprite2D sprite;
  private CollisionShape2D hitbox;
  private AnimationPlayer ap;
  private BIdle bIdleState;
  private BRoam bRoamState;
  private StateMachine sm;
  private State currentState;

  // Seek radius to find the player
  [Export]
  private float seekRaidus = 400;
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
}
