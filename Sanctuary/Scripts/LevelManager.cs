using Godot;
using System;

public partial class LevelManager : Node {
    [Signal]
    public delegate void LevelLoadedEventHandler();

    private Node currentLevel;
    private Marker2D startingPos;
    private Godot.Collections.Dictionary levels;
    public override void _Ready() {
        // This gets the last node in the LevelManager node.
        // This could break if it's not a Level, but there shouldn't
        // be anything else inside the LevelManager node besides Levels.
        // We can also use z-indexing if we want to overlay anything on top like a load screen.
        currentLevel = GetChild(GetChildCount() - 1);
        currentLevel.Connect(nameof(Level.LevelChanged), new Callable(this, MethodName.GotoLevel));
        startingPos = currentLevel.GetNode<Marker2D>("StartingPoint");
    }

    public void GotoLevel(string path) {
        // Ensures all processes are completed on the current frame first before executing
        CallDeferred(MethodName.DeferredGotoLevel, path);
    }

    public void DeferredGotoLevel(string path) {
        currentLevel.Free();

        var nextLevel = GD.Load<PackedScene>(path);
        currentLevel = nextLevel.Instantiate();
        // After instantiating the node, make sure to connect the signals.
        currentLevel.Connect(nameof(Level.LevelChanged), new Callable(this, MethodName.GotoLevel));
        startingPos = currentLevel.GetNode<Marker2D>("StartingPoint");
        AddChild(currentLevel);
        EmitSignal(nameof(LevelLoaded), startingPos);
    }
}
