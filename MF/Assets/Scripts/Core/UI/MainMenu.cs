using Godot;
using System;

public class MainMenu : Node2D
{
    [Export]
    private PackedScene mainGame;
    [Export]
    private PackedScene optionsMenu;

    public override void _Ready() {
    }

    public void OnNewGameButtonButtonUp() {
        Disconnect("button_up", this, nameof(OnNewGameButtonButtonUp));
        GetTree().ChangeSceneTo(mainGame);
    }

    public void OnOptionsButtonButtonUp() {
        Disconnect("button_up", this, nameof(OnOptionsButtonButtonUp));
        GetTree().ChangeSceneTo(optionsMenu);
    }

    public void OnExitButtonButtonUp() {
        GetTree().Quit();
    }

}
