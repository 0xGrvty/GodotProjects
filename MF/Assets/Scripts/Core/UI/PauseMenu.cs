using Godot;
using System;

public class PauseMenu : Control
{
	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("Pause")) {
			var newPauseState = !GetTree().Paused;
			GetTree().Paused = newPauseState;
			Visible = newPauseState;
		}
	}
	public void OnResumeButtonButtonUp() {
		var newPauseState = !GetTree().Paused;
		GetTree().Paused = newPauseState;
		Visible = newPauseState;
	}

	public void OnBackToMenuButtonButtonUp() {
		var newPauseState = !GetTree().Paused;
		GetTree().Paused = newPauseState;
		GetTree().ChangeScene(Singleton.Instance.mainMenuPath);
	}
}
