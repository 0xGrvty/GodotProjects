using Godot;
using System;

public partial class Player : Actor {
    // Signals
    [Signal]
    public delegate void HitstopEventHandler();
    [Signal]
    public delegate void ShakeCameraEventHandler();
    [Signal]
    public delegate void MoveEventHandler();

    // Constants
    private const int NUM_JUMPS = 1;
    private const int NUM_HEAD_RAYS = 4;
    private const int CORNER_CORRECTION = 4;

    // Static variables
    private static float COYOTE_TIME = 3f * Game.ONE_FRAME;
    private static float JUMP_BUFFER_TIME = 6f * Game.ONE_FRAME;
    private static float JUMP_HOLD_TIME = 12f * Game.ONE_FRAME;
    public static int ATTACK_INPUT_BUFFER = 24; // 24 frame buffer

    // private variables
    private Vector2 velocity = Vector2.Zero;
    [Export]
    private float maxSpeed = 100;
    [Export]
    private float maxAccel = 800;
    private bool canSpecial = true;
    private float localHoldTime = 0;
    private bool onGround = true;
    [Export]
    private float jumpHeight = 21f;
    [Export]
    private float jumpTimeToPeak = 0.3f;
    [Export]
    private float jumpTimeToDescent = 0.2f;
    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;
    
    private bool isJumping;
    private int attackCounter = 3;
    private Timer attackTimer;
    private int numJumps = NUM_JUMPS;
    private float coyoteTime = 0;
    private bool wasGrounded = true;
    private float jumpBufferTime = 0;
    private InputBuffer dirInputBuffer;
    private InputBuffer attackInputBuffer;
    private bool showComboList = false;
    //private float attackInputBuffer = 0;
    private Vector2 snapshotVelocity;
    private int snapshotDirection;
    private AnimationPlayer animationPlayer;

    // public variables
    public int AttackCounter { get => attackCounter; set => attackCounter = value; }
    public float JumpBufferTime { get => jumpBufferTime; set => jumpBufferTime = value; }
    public float CoyoteTime { get => coyoteTime; set => coyoteTime = value; }
    
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    public bool IsJumping { get => isJumping; }
    public int NumJumps { get => numJumps; set => numJumps = value; }
    public bool WasGrounded { get => wasGrounded; set => wasGrounded = value; }
    public bool CanSpecial { get => canSpecial; set => canSpecial = value; }
    public InputBuffer AttackInputBuffer { get => attackInputBuffer; }
    public AnimationPlayer AP { get => animationPlayer; set => animationPlayer = value; }
    // State machine
    public IStateMachine currentState;
    public FiniteStateMachine fsm;
    public PlayerRunState playerRunState;
    public PlayerIdleState playerIdleState;
    public PlayerJumpState playerJumpState;
    public PlayerJumpSquatState playerJumpSquatState;
    public PlayerFallState playerFallState;
    public PlayerAttackState1 playerAttackState1;
    public PlayerAttackState2 playerAttackState2;
    public PlayerAttackState3 playerAttackState3;
    public PlayerSceneTransitionState playerSceneTransitionState;
    public PlayerChargeAttackState playerChargeAttackState;
    public PlayerUppercutState playerUppercutState;

    public override void _Ready() {
        // No onready, so get the node in this part of the pipeline.
        AnimatedSprite = GetNode<AnimatedSprite2D>("Animations");
        AP = GetNode<AnimationPlayer>("AnimationPlayer");
        Hurtbox = (Hitbox)GetNode<Node2D>("Hurtbox");
        Sprite = GetNode<Sprite2D>("Sprite");
        // Since C# does not have onready, we still need to fetch the globals.
        GM = GetNode<Game>("/root/Game");
        AddToGroup("Actors");
        AddToGroup("CameraShakers");

        // Building a Better Jump GDC talk
        jumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f; // In Godot 2D, down is positive, so flip the signs
        jumpGravity = ((-2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0f;
        fallGravity = ((-2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) * -1.0f;

        // player state machine
        //playerRunState = new PlayerRunState();
        //playerIdleState = new PlayerIdleState();
        //playerJumpState = new PlayerJumpState();
        //playerJumpSquatState = new PlayerJumpSquatState();
        //playerFallState = new PlayerFallState();
        //playerAttackState1 = new PlayerAttackState1();
        //playerAttackState2 = new PlayerAttackState2();
        //playerAttackState3 = new PlayerAttackState3();
        //playerSceneTransitionState = new PlayerSceneTransitionState();
        //currentState = playerIdleState;
        playerAttackState1 = (PlayerAttackState1)GetNode<Node>("StateMachine/Attack1");
        playerAttackState2 = (PlayerAttackState2)GetNode<Node>("StateMachine/Attack2");
        playerAttackState3 = (PlayerAttackState3)GetNode<Node>("StateMachine/Attack3");
        playerRunState = (PlayerRunState)GetNode<Node>("StateMachine/Run");
        playerIdleState = (PlayerIdleState)GetNode<Node>("StateMachine/Idle");
        playerJumpState = (PlayerJumpState)GetNode<Node>("StateMachine/Jump");
        playerFallState = (PlayerFallState)GetNode<Node>("StateMachine/Fall");
        fsm = (FiniteStateMachine)GetNode<Node>("StateMachine");
        Facing = Facing.RIGHT;
        wasGrounded = true;
        dirInputBuffer = new InputBuffer(9);
        attackInputBuffer = new InputBuffer(ATTACK_INPUT_BUFFER);
    }
    public override void _Draw() {
        DrawLine(Vector2.Zero, 15f * Vector2.Down, Colors.Yellow);
        DrawLine(Vector2.Zero, 15f * Vector2.Up, Colors.Yellow);
        DrawLine(Vector2.Zero, 15f * Vector2.Right, Colors.Green);
        DrawLine(Vector2.Zero, 15f * Vector2.Left, Colors.Green);
    }

    public override void _Process(double delta) {
        onGround = GM.CheckWallsCollision(this, Vector2.Down);
        //currentState = currentState.EnterState(this);
        QueueRedraw();
    }

    public void OnCollisionX() {
        velocity.X = 0;
        ZeroRemainderX();
    }

    public void OnCollisionY() {

        // First do a fuzzy check to see if the player hits a corner
        if (velocity.Y < 0) {
            if (velocity.X >= 0) {
                for (int i = 1; i <= CORNER_CORRECTION; i++) {
                    // If the fuzzy check returns false, then move them and let them jump!
                    if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
                        GlobalPosition += new Vector2(i, -1);
                        // return since we don't want to zero the remainder when we correct the jump
                        return;
                    }
                    if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
                        GlobalPosition += new Vector2(-i, -1);
                        // return since we don't want to zero the remainder when we correct the jump
                        return;
                    }
                }
            }
            if (velocity.X <= 0) {
                for (int i = 1; i <= CORNER_CORRECTION; i++) {
                    if (!GM.CheckWallsCollision(this, new Vector2(i, -1))) {
                        GlobalPosition += new Vector2(i, -1);
                        return;
                    }
                    if (!GM.CheckWallsCollision(this, new Vector2(-i, -1))) {
                        GlobalPosition += new Vector2(-i, -1);
                        return;
                    }
                }
            }

        }
        velocity.Y = 0;
        ZeroRemainderY();
    }

    public int GetDirectionInput() {
        var directionVector = new Vector2(Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left")), Math.Sign(Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")));
        var direction = Math.Sign(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));

        if (direction > 0 && Facing == Facing.LEFT) {
            Facing = Facing.RIGHT;
            Sprite.FlipH = false;

        } else if (direction < 0 && Facing == Facing.RIGHT) {
            Facing = Facing.LEFT;
            Sprite.FlipH = true;
        }

        dirInputBuffer.AddInput(directionVector);
        return direction;
    }

    public void DoMovement(double delta, int direction) {
        Jump(delta);

        velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction, maxAccel * (float)delta);
        velocity.Y = Mathf.MoveToward(velocity.Y, GetGravity(), GetGravity() * (float)delta);

        MoveX(velocity.X * (float)delta, new Callable(this, nameof(OnCollisionX)));
        MoveY(velocity.Y * (float)delta, new Callable(this, nameof(OnCollisionY)));
        if (velocity != Vector2.Zero) EmitSignal(SignalName.Move);
    }

    public void Jump(double delta) {
        var jumpPressed = Input.IsActionJustPressed("Jump");
        var jumping = false;

        // If the player presses the jump button JUMP_BUFFER_TIME frames before hitting the ground,
        // allow them to jump
        jumpBufferTime -= (float)delta;
        if (jumpPressed) {
            jumpBufferTime = JUMP_BUFFER_TIME;
        }

        // Check if the player was allowed to jump, then check if the player is on the ground or was on the ground (coyote time)
        // or check if the player was allowed to jump and if they have enough jumps (double jump, mid-air jump)
        if ((jumpBufferTime > 0.0f && (onGround || wasGrounded)) || (jumpBufferTime > 0.0f && numJumps > 0)) {
            numJumps--;
           velocity = new Vector2(velocity.X, jumpVelocity);
            jumpBufferTime = 0;
            localHoldTime = JUMP_HOLD_TIME;
            jumping = true;
        } else if (localHoldTime > 0) {
            if (Input.IsActionPressed("Jump")) {
                velocity = new Vector2(velocity.X, jumpVelocity);
            } else {

                // Give the player a little nudge so that if they let go too early they can still make their jump
                //velocity = new Vector2(velocity.X, 1.15f * velocity.Y);

                localHoldTime = 0;
            }
            //velocity = new Vector2(velocity.X, 1.15f * velocity.Y);
            localHoldTime -= (float)delta;
        }

        isJumping = jumping;
    }

    // return jumpGravity if player is jumping, otherwise return fallGravity
    public float GetGravity() {
        return velocity.Y < 0.0 ? jumpGravity : fallGravity;
    }

    public bool IsGrounded() {
        return onGround;
    }

    public void ResetAttackCounter() {
        //attackTimer.Stop();
        //attackInputBuffer = 0;
        attackCounter = 3;
    }

    public Timer GetAttackTimer() {
        return attackTimer;
    }

    public void OnAttackTimeout() {
        attackCounter = 3;
    }

    public void ResetGroundedStats() {
        coyoteTime = COYOTE_TIME;
        numJumps = NUM_JUMPS;
        jumpBufferTime = 0;
    }

    public void DoAttack() {
        // Maybe we should add the attack input on every frame, that way we can buffer an attack while falling or something.
        attackInputBuffer.AddInput(new StringName());

        if (Input.IsActionJustPressed("Attack")) {
            var attackButton = new StringName("Attack");
            attackInputBuffer.AddInput(attackButton);
        }

    }

    // We use this a lot, so let's make a helper function.
    // Note:  We may get rid of this later as we refactor our player to use an AnimationPlayer instead.
    public bool IsOnLastFrame() {
        return AnimatedSprite.Frame >= AnimatedSprite.SpriteFrames.GetFrameCount(AnimatedSprite.Animation) - 1;
    }

    public Godot.Collections.Array<Variant> GetInputBufferContents() {
        return attackInputBuffer.GetBuffer();
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    public float GetMaxAccel() {
        return maxAccel;
    }
}
