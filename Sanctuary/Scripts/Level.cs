using Godot;
using System;

public partial class Level : Node2D {
    [Signal]
    public delegate void LevelChangedEventHandler();

    [Export]
    public string levelGoto;

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionJustPressed("Test_ChangeLevel")) {
            EmitSignal(nameof(LevelChanged), levelGoto);
        }
    }

}
