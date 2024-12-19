using Godot;
using System;
using Godot.Collections;

public partial class Player : CharacterBody2D {
  // Constants
  [Export]
  private float JUMP_HOLD_TIME = 0.2f;

  // State Machine
  public StateMachine sm;
  public PIdle pIdle;
  public PRun pRun;
  public PJump pJump;
  public PAttack pAttack;
  public PFall pFall;


  // Export variables
  [Export]
  private float maxSpeed = 700;
  [Export]
  private float maxAccel = 7000;
  [Export]
  private float jumpHeight = 100f;
  [Export]
  private float jumpTimeToPeak = 0.3f;
  [Export]
  private float jumpTimeToDescent = 0.25f;
  [Export]
  private Area2D attack;

  // Private variables
  private Dictionary<StringName, State> states;
  private float jumpVelocity;
  private float jumpGravity;
  private float fallGravity;
  private bool isJumping = false;
  private bool isHoldingJump = false;
  private AnimationPlayer topAP;
  private AnimationPlayer botAP;
  private AnimatedSprite2D testAnimsArms;
  private AnimatedSprite2D testAnimsTorso;
  private AnimatedSprite2D testAnimsLegs;

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
  public bool IsHoldingJump { get => isHoldingJump; set => isHoldingJump = value; }
  public bool JumpPressed { get => jumpPressed; set => jumpPressed = value; }
  public float JumpVelocity { get => jumpVelocity; }
  public Direction Dir { get => dir; }
  public Direction Facing { get => facing; }
  public bool AttackPressed { get => attackPressed; set => attackPressed = value; }
  public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
  public AnimationPlayer TopAP { get => topAP; }
  public AnimationPlayer BotAP { get => botAP; }
  public AnimatedSprite2D TestAnimsArms { get => testAnimsArms; }
  public AnimatedSprite2D TestAnimsTorso { get => testAnimsTorso; }
  public AnimatedSprite2D TestAnimsLegs { get => testAnimsLegs; }

    // If you need to check if an event happened, i.e. if jump was pressed, then handle it here!
  // However, if you need to poll an input over a certain amount of time, then handle it in _PhysicsProcess!
  // This was the explanation I needed back then.  I don't know why everything I Googled was so poorly explained.
  // It's the difference of polling vs checking the event once.  Holy crap.
  public override void _UnhandledInput(InputEvent @event) {
    var e = @event;

    if (e.IsActionPressed("Jump")) isJumping = true;
    else if (e.IsActionPressed("Attack")) isAttacking = true;
  }

  public override void _Ready() {
    sm = (StateMachine)GetNode<Node>("StateMachine");
    pIdle = (PIdle)GetNode<Node>("StateMachine/Idle");
    pRun = (PRun)GetNode<Node>("StateMachine/Run");
    pJump = (PJump)GetNode<Node>("StateMachine/Jump");
    pAttack = (PAttack)GetNode<Node>("StateMachine/Attack");
    pFall = (PFall)GetNode<Node>("StateMachine/Fall");

    states = new Godot.Collections.Dictionary<StringName, State>();

    attack = GetNode<Area2D>("Attack");
    
    sprite = GetNode<Sprite2D>("Sprite");

    topAP = GetNode<AnimationPlayer>("Top");
    botAP = GetNode<AnimationPlayer>("Bottom");

    testAnimsTorso = GetNode<AnimatedSprite2D>("TestAnimsTorso");
    testAnimsArms = GetNode<AnimatedSprite2D>("TestAnimsArms");
    testAnimsLegs = GetNode<AnimatedSprite2D>("TestAnimsLegs");

    

    // Initializations
    // Building a Better Jump GDC talk
    jumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f; // In Godot 2D, down is positive, so flip the signs
    jumpGravity = ((-2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0f;
    fallGravity = ((-2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) * -1.0f;

  }

  public override void _Draw() {
    // Shows the Node's global position, useful for debugging
    DrawLine(Vector2.Zero, 35f * Vector2.Down, Colors.Yellow);
    DrawLine(Vector2.Zero, 35f * Vector2.Up, Colors.Blue);
    DrawLine(Vector2.Zero, 35f * Vector2.Right, Colors.Green);
    DrawLine(Vector2.Zero, 35f * Vector2.Left, Colors.Red);

    if (sm.GetState() is PAttack) DrawLine(new Vector2(0, -20), new Vector2(300, -20), Colors.Cyan);
  }

  public override void _Process(double delta) {
    QueueRedraw();
    
  }

  public void PollInputs() {
    isHoldingJump = Input.IsActionPressed("Jump");

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

  public void Jump(double delta) {
    // If they initially jumped while on the floor
    if (IsOnFloor()) {

      Velocity = new Vector2(Velocity.X, jumpVelocity);
      localHoldTime = JUMP_HOLD_TIME;
    }
    
    else if (localHoldTime > 0) {
      localHoldTime -= (float)delta;
      // Poll the jump key to see if the player is holding it.  Max jump height if the player holds it down all the way
      if (isHoldingJump) Velocity = new Vector2(Velocity.X, jumpVelocity);

      // Else they've released the key before the maximum hold time, cut the jump velocity
      else {
        Velocity = new Vector2(Velocity.X, 0.25f * jumpVelocity);
        localHoldTime = 0;
      }

    }
  }

  public float GetGravity() {
    return Velocity.Y < 0.0f ? jumpGravity : fallGravity;
  }
}
