using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Game : Node {
	internal static string HITSTOP_SIGNAL = "Hitstop";
	// Will be referencing this multiple times so we should make it static
	private static float one_frame = 1f / (float)ProjectSettings.GetSetting("application/run/max_fps");
	public static float ONE_FRAME { get => one_frame; }

	private static Vector2 screenSize = new Vector2((float)ProjectSettings.GetSetting("display/window/size/viewport_width"),
                                                    (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static Vector2 ScreenSize { get => screenSize; }

	[Signal]
	public delegate void HitstopEventHandler(int frames);

	[Export]
	private string pathToTestLevel;
	private Player player;
	private LevelManager lm;

	private bool isWaiting = false;

	public override void _Ready() {
		player = (Player)GetNode<Node2D>("Player");
        lm = (LevelManager)GetNode<Node>("LevelManager");
        lm.Connect(nameof(LevelManager.LevelLoaded), new Callable(this, MethodName.OnLevelChanged));
	}

	public override void _Process(double delta) {

	}

	public bool CheckWallsCollision(Actor entity, Vector2 offset) {
        var walls = GetTree().GetNodesInGroup("Walls");

		foreach (Wall wall in walls) {
            if (entity.Hurtbox.Intersects(wall.Hitbox, offset)) {
				return true; 
			}
		}

		return false;
	}

	// Return ALL of the actors in the game
	public Godot.Collections.Array<Node> GetAllActors() {
		return GetTree().GetNodesInGroup("Actors");
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

    public void OnLevelChanged(Marker2D startingPos) {
        player.GlobalPosition = startingPos.GlobalPosition;
    }
}
