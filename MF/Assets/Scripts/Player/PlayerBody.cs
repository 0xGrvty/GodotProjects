using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public class PlayerBody : KinematicBody2D {

#pragma warning disable 649
	[Export]
	public PackedScene hammerScene;
	[Export]
	private AnimatedSprite animations;
#pragma warning restore 649
	[Signal]
	private delegate void StateChanged();
	[Signal]
	public delegate void PlayerDamaged(int health);
	[Signal]
	public delegate void PlayerHealed(int health);
	[Signal]
	public delegate void PlayerHealthChanged(int health, int maxHealth);
	[Signal]
	public delegate void PlayerVelocityChanged(int speed);

	[Export]
	public int health = 100;
	[Export]
	public int maxHealth = 500;

	// Constants
	private float BASE_ATTACK_SPEED = 1f;
	private int BASE_MOVE_SPEED = 200;

	// Private variables
	public enum FaceDir {
		RIGHT = 0,
		UP_RIGHT = 1,
		UP = 2,
		UP_LEFT = 3,
		LEFT = 4,
		DOWN_LEFT = 5,
		DOWN_RIGHT = 6,
		DOWN = 7,
	};
	private int maxSpeed;
	private FaceDir facing = FaceDir.RIGHT;
	private Vector2 velocity = Vector2.Zero;
	public Vector2 Velocity { get => velocity; }
	public Vector2 screenSize = Vector2.Zero;
	private bool isMoving = false;
	private float attackCooldown;
	private float baseAttackCooldown;
	private bool isAttacking = false;
	private Dictionary playerStats = new Dictionary();
	private Vector2 lastPos = Vector2.Zero;
	public bool takenDamage = false;
	public bool healedDamage = false;
	//private KinematicBody2D kinematicBody2D;

	// State Machine
	private IStateMachine currentState;
	public PlayerIdleState playerIdleState = new PlayerIdleState();
	public PlayerRunState playerRunState = new PlayerRunState();
	public PlayerAttackState playerAttackState = new PlayerAttackState();
	public PlayerDeathState playerDeathState = new PlayerDeathState();

	// Getters/Setters
	public bool IsMoving { get => isMoving; set => isMoving = value; }
	public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

	public override void _Ready() {
		Position = GetParent().GetNode<Position2D>("StartPosition").Position;
		screenSize = GetViewportRect().Size;

		currentState = playerIdleState;
		maxSpeed = BASE_MOVE_SPEED;
		animations = GetNode<AnimatedSprite>("AnimatedSprite");
		baseAttackCooldown = (14f / 12);
		attackCooldown = baseAttackCooldown;
		Connect(nameof(StateChanged), this, nameof(OnStateChanged));
		Connect(nameof(PlayerDamaged), this, nameof(OnPlayerDamaged));
		//healthListener = (HealthListener)GetNode<Node>("HealthListener");
		//Connect("TakeDamage", healthListener, "OnTakeDamage");
		//Connect("HealDamage", healthListener, "OnHealDamage");
		//healthListener.Init(health, maxHealth);
		//kinematicBody2D = GetNode<KinematicBody2D>("KinematicBody2D");
		
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventKey e) {
			//if (e.IsActionPressed("BlessedHammer")) {
			//    currentState = playerAttackState;
			//}
		}
	}

	public void InitPlayerStats() {
		EventBus.Instance.EmitSignal(nameof(PlayerVelocityChanged), maxSpeed);
		EventBus.Instance.EmitSignal(nameof(PlayerHealthChanged), health, maxHealth);
	}

	public override void _Process(float delta) {
		// This fixes the jitter.  It gets a fraction  through the current physics tick
		// and uses that fraction as the weight to linearly interpolate the transform of the object (in this case the enemy)
		// from the previous position
		var fraction = Engine.GetPhysicsInterpolationFraction();
		var transform = Transform;
		transform.origin = lastPos.LinearInterpolate(GlobalTransform.origin, fraction);

		if (Input.IsActionJustPressed("DamageTest")) {
			// other things will damage the player, so we can propogate the signal to the player
			EmitSignal(nameof(PlayerDamaged));
		}

		// Since the player should keep track of their own health, we will have the healthListener emit it's own signal
		if (Input.IsActionJustPressed("HealTest")) {
			

		}
	}

	public override void _PhysicsProcess(float delta) {
		lastPos = velocity;
		currentState = currentState.EnterState(this);
		// This may be a bad idea to send a signal every single frame.  Let's think of another way to propogate the signal to the UI
		// EmitSignal(nameof(VelocityChanged()));
		// Redraw debug visuals
		//Update();
	}

	public override void _Draw() {
		DrawLine(Vector2.Zero, velocity, Color.ColorN("blue"), 5.0f);
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
			facing = FaceDir.LEFT;
			if (Mathf.Sign(velocity.y) > 0) { facing = FaceDir.DOWN_LEFT; } else if (Mathf.Sign(velocity.y) < 0) { facing = FaceDir.UP_LEFT; }
		}
		if (Input.IsActionPressed("Right")) {
			velocity += Vector2.Right;
			facing = FaceDir.RIGHT;
			if (Mathf.Sign(velocity.y) > 0) { facing = FaceDir.DOWN_RIGHT; } else if (Mathf.Sign(velocity.y) < 0) { facing = FaceDir.UP_RIGHT; }
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

	// Can probably get rid of attackCooldown.  There's an easier way to implement this using Animation.speedScale and we can have frame breakpoints just like in D2
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

	private void OnPlayerDamaged() {
		health -= 5;
		health = Math.Max(0, health);
		//GD.Print(String.Format("We took {0} damage and we are now at {1}", 5, health));
		EventBus.Instance.EmitSignal(nameof(PlayerHealthChanged), health, maxHealth);

		if (health <= 0) {
			currentState = playerDeathState;
		}
	}

	private void OnPlayerHealed() {
		health += 10;
		health = Math.Min(health, maxHealth);
		//GD.Print(String.Format("We healed {0} damage and we are now at {1}", 10, health));
		EventBus.Instance.EmitSignal(nameof(PlayerHealthChanged), health, maxHealth);
	}

}
