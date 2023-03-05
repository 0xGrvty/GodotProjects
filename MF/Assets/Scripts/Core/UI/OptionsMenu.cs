using Godot;
using System;

public class OptionsMenu : Control
{
	public void OnGoBackButtonButtonUp() {
		GetTree().ChangeScene(Singleton.Instance.mainMenuPath);
	}
}
