using Godot;
using System;

public partial class Afterimage : Node2D {
	[Export]
	private float duration = 0.5f;
	private Sprite2D s;

	public override void _Ready() {
		var tween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.Connect("finished", new Callable(this, nameof(OnTweenFinished)));

		s = GetNode<Sprite2D>("GhostTexture");

		tween.TweenProperty(s, "modulate:a", 0, duration);
	}

	private void OnTweenFinished() {
		QueueFree();
	}
}
