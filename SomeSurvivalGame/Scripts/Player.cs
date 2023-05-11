using Godot;
using System;

public partial class Player : Node2D {
    private Area2D collider;

    public override void _Ready() {
        collider = GetNode<Area2D>("Collider");
    }

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionJustPressed("Attack")) {
            var space = GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(GlobalPosition,  GetGlobalMousePosition());
            var result = space.IntersectRay(query);
            GD.Print(query);
            if (result.Count > 0) {
                GD.Print("Suh dude");
            }
        }
    }

}
