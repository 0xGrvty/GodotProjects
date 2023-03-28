using Godot;
using System;

public partial class Game : Node {
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
