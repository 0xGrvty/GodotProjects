using Godot;
using System;

public partial class Game : Node {
    // Will be referencing this multiple times so we should make it static
    private static float one_frame = 1f / (float)ProjectSettings.GetSetting("application/run/max_fps");
    public static float ONE_FRAME { get => one_frame; }

    [Export]
    private string pathToLevel1;
    private Player player;
    private Node2D levels;

    public override void _Ready() {
        //levels = GetNode<Node2D>("Levels");
        player = (Player)GetNode<Node2D>("Player");
        //levels.AddChild(GD.Load<PackedScene>(pathToLevel1).Instantiate());
        GetTree().ChangeSceneToFile(pathToLevel1);
    }

    public bool CheckWallsCollision(Actor entity, Vector2 offset) {

        var walls = GetTree().GetNodesInGroup("Walls");

        foreach (Wall wall in walls) {
            if (wall.JumpThru) {
                if (offset == Vector2.Down && entity.IsRiding((Solid)wall, offset)) {
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
}
