using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Game : Node {
    internal static string HITSTOP_SIGNAL = "Hitstop";
    // Will be referencing this multiple times so we should make it static
    private static float one_frame = 1f / (float)ProjectSettings.GetSetting("application/run/max_fps");
    public static float ONE_FRAME { get => one_frame; }

    [Signal]
    public delegate void HitstopEventHandler(int frames);

    [Export]
    private string pathToLevel1;
    private Player player;
    private Node2D levels;

    private bool isWaiting = false;
    private Camera2D camera;

    public override void _Ready() {
        //levels = GetNode<Node2D>("Levels");
        player = (Player)GetNode<Node2D>("Player");
        camera = GetNode<Camera2D>("Camera");
        //levels.AddChild(GD.Load<PackedScene>(pathToLevel1).Instantiate());
        GetTree().ChangeSceneToFile(pathToLevel1);

        /// TODO: Replace this with a for loop that loops through all things that can request hitstops
        player.Connect(HITSTOP_SIGNAL, new Callable(this, nameof(HandleHitstop)));
    }

    public override void _Process(double delta) {
        /// TODO: FIGURE OUT HOW TO MAKE A CAMERA YAY
        /// Remember that we are moving by integers, so simply
        /// doing this will cause a jittery camera.
        /// Maybe try some kind of interpolation here
        camera.GlobalPosition = player.GlobalPosition;
    }

    public bool CheckWallsCollision(Actor entity, Vector2 offset) {

        var walls = GetTree().GetNodesInGroup("Walls");

        foreach (Wall wall in walls) {
            if (wall.JumpThru) {
                if (offset == Vector2.Down && entity.IsRiding((Solid)wall, offset)) {
                    if (entity is Missile) {
                        return false;
                    }
                    return true;
                }
            } else if (entity.Hitbox.Intersects(wall.Hitbox, offset)) {
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
        await ToSignal(GetTree().CreateTimer(frames * ONE_FRAME, true, false, true), SceneTreeTimer.SignalName.Timeout);
        Engine.TimeScale = 1;
        isWaiting = false;
    }
}
