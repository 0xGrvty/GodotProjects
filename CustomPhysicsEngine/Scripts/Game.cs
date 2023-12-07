using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Game : Node {
	internal static string HITSTOP_SIGNAL = "Hitstop";
	// Will be referencing this multiple times so we should make it static
	private static float one_frame = 1f / (float)ProjectSettings.GetSetting("application/run/max_fps");
	public static float ONE_FRAME { get => one_frame; }

	private static Vector2 screenSize = new Vector2((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static Vector2 ScreenSize { get => screenSize; }

	[Signal]
	public delegate void HitstopEventHandler(int frames);

	[Export]
	private string pathToTestLevel;
	private Player player;
	private Node2D levels;

	private bool isWaiting = false;
	private Camera2D camera;
	//private Node camera;

	public override void _Ready() {
		//levels = GetNode<Node2D>("Levels");
		GetTree().ChangeSceneToFile(pathToTestLevel);
		player = (Player)GetNode<Node2D>("Player");
		camera = GetNode<Camera2D>("CameraShake");

		/// TODO: Replace this with a for loop that loops through all things that can request hitstops
		player.Connect(HITSTOP_SIGNAL, new Callable(this, nameof(HandleHitstop)));
	}

	public override void _Process(double delta) {
		// Remember that we are moving by integers,
		// so simply doing this will cause a jittery camera.
		// I wonder if there's a way to make a smoother camera.
		camera.GlobalPosition = camera.GlobalPosition.Lerp(player.GlobalPosition, (float)delta * 5.0f);
		camera.GlobalPosition = player.GlobalPosition;

	}

	public bool CheckWallsCollision(Actor entity, Vector2 offset) {

		var walls = GetTree().GetNodesInGroup("Walls");

		foreach (Wall wall in walls) {
			if (wall.JumpThru) {
				if (offset == Vector2.Down && entity.IsRiding((Solid)wall, offset)) {
					return true;
				}
			} else if (entity.Hurtbox.Intersects(wall.Hitbox, offset)) {
				return true; 
			}
		}

		return false;
	}

	// Return ALL of the actors in the game
	public Godot.Collections.Array<Node> GetAllActors() {
		return GetTree().GetNodesInGroup("Actors");
	}

	// Simply return the actors that are riding a platform
	public Godot.Collections.Array GetAllRidingActors(Solid solid) {

		var riders = new Godot.Collections.Array();
		var actors = GetTree().GetNodesInGroup("Actors");

		foreach (Actor actor in actors) {
			if (actor.IsRiding(solid, Vector2.Down)) {
				riders.Add(actor);
			}
		}

		return riders;
	}

	public async void HandleHitstop(int frames) {
		if (isWaiting) {
			return;
		}
		isWaiting = true;
		Engine.TimeScale = 0;
		await ToSignal(GetTree().CreateTimer(Math.Abs(frames) * ONE_FRAME, true, false, true), SceneTreeTimer.SignalName.Timeout);
		Engine.TimeScale = 1;
		isWaiting = false;
	}
}
