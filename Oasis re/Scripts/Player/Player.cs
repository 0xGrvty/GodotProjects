using Godot;
using System;
using Godot.Collections;

public partial class Player : CharacterBody2D {
  // Constants
  private const float JUMP_HOLD_TIME = 0.1f;

  // State Machine
  public StateMachine sm;
  public PIdle pIdleState;
  public PRun pRunState;
  public PJump pJumpState;
  public PAttack pAttackState;
  private State currentState;


  // Export variables
  [Export]
  private float maxSpeed = 100;
  [Export]
  private float maxAccel = 800;
  [Export]
  private float jumpHeight = 18f;
  [Export]
  private float jumpTimeToPeak = 0.25f;
  [Export]
  private float jumpTimeToDescent = 0.15f;
  [Export]
  private Area2D attack;

  // Private variables
  private Dictionary<StringName, State> states;
  private float jumpVelocity;
  private float jumpGravity;
  private float fallGravity;
  private bool isJumping = false;
  private AnimationPlayer topAP;
  private AnimationPlayer botAP;

  private Vector2 directionVector;
  private Direction dir;

  private float localHoldTime = 0;
  private bool jumpPressed = false;
  private bool attackPressed = false;
  private bool isAttacking = false;


  private Sprite2D sprite;
  private Direction facing = Direction.RIGHT;

  // Public variables
  public bool IsJumping { get => isJumping; set => isJumping = value; }
  public bool JumpPressed { get => jumpPressed; set => jumpPressed = value; }
  public float JumpVelocity { get => jumpVelocity; }
  public Direction Dir { get => dir; }
  public Direction Facing { get => facing; }
  public bool AttackPressed { get => attackPressed; set => attackPressed = value; }
  public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
  public AnimationPlayer TopAP { get => topAP; }
  public AnimationPlayer BotAP { get => botAP; }

  public override void _Ready() {
    sm = (StateMachine)GetNode<Node>("StateMachine");
    pIdleState = (PIdle)GetNode<Node>("StateMachine/Idle");
    pRunState = (PRun)GetNode<Node>("StateMachine/Run");
    pJumpState = (PJump)GetNode<Node>("StateMachine/Jump");
    pAttackState = (PAttack)GetNode<Node>("StateMachine/Attack");

    states = new Godot.Collections.Dictionary<StringName, State>();

    attack = GetNode<Area2D>("Attack");
    
    sprite = GetNode<Sprite2D>("Sprite");

    topAP = GetNode<AnimationPlayer>("Top");
    botAP = GetNode<AnimationPlayer>("Bottom");

    // Initializations
    // Building a Better Jump GDC talk
    jumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f; // In Godot 2D, down is positive, so flip the signs
    jumpGravity = ((-2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0f;
    fallGravity = ((-2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) * -1.0f;

    currentState = sm.GetState();
  }

  public override void _Draw() {
    // Shows the Node's global position, useful for debugging
    DrawLine(Vector2.Zero, 35f * Vector2.Down, Colors.Yellow);
    DrawLine(Vector2.Zero, 35f * Vector2.Up, Colors.Blue);
    DrawLine(Vector2.Zero, 35f * Vector2.Right, Colors.Green);
    DrawLine(Vector2.Zero, 35f * Vector2.Left, Colors.Red);
  }

  public override void _Process(double delta) {
    QueueRedraw();
  }

  public void PollInputs() {
    var theScale = attack.Position;


    directionVector = new Vector2(Math.Sign(Input.GetActionStrength("Right") - Input.GetActionStrength("Left")), Math.Sign(Input.GetActionStrength("Down") - Input.GetActionStrength("Up")));
    dir = (Direction)Math.Sign(Input.GetActionStrength("Right") - Input.GetActionStrength("Left"));

    // Flip the entire CharacterBody2D since we have asymmetric hitboxes for our attack
    if (dir == Direction.RIGHT && facing == Direction.LEFT) {
      ApplyScale(new Vector2(-1, 1));
      facing = Direction.RIGHT;
    } else if (dir == Direction.LEFT && facing == Direction.RIGHT) {
      ApplyScale(new Vector2(-1, 1));
      facing = Direction.LEFT;
    }
  }

  public void Move(double delta) {
    Velocity = new Vector2(Mathf.MoveToward(Velocity.X, maxSpeed * (int)dir, maxAccel * (float)delta), Mathf.MoveToward(Velocity.Y, GetGravity(), GetGravity() * (float)delta));
    MoveAndSlide();
  }

  private void CheckHitbox() {
    attack.Monitoring = true;
  }

  public void Jump() {
    var delta = GetPhysicsProcessDeltaTime();

    // If they initially jumped while on the floor
    if (IsOnFloor()) {

      Velocity = new Vector2(Velocity.X, jumpVelocity);
      localHoldTime = JUMP_HOLD_TIME;

      // Else, they are in the air already and holding the jump key
    } else if (localHoldTime > 0) {

      // Poll the jump key to see if the player is holding it
      if (Input.IsActionPressed("Jump")) Velocity = new Vector2(Velocity.X, jumpVelocity);
      else localHoldTime = 0;

      localHoldTime -= (float)delta;
    }

  }

  public float GetGravity() {
    return Velocity.Y < 0.0f ? jumpGravity : fallGravity;
  }
}
