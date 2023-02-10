using Godot;
using System;

public class MainMenu : Node2D
{
#pragma warning disable 649
	[Export]
	private PackedScene mainGame;
	[Export]
	private PackedScene optionsMenu;
#pragma warning restore 649

	public override void _Ready() {
	}

	public void OnNewGameButtonButtonUp() {
		GetTree().ChangeSceneTo(mainGame);
	}

	public void OnOptionsButtonButtonUp() {
		GetTree().ChangeSceneTo(optionsMenu);
	}

	public void OnExitButtonButtonUp() {
		GetTree().Quit();
	}

}
