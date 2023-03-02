using Godot;
using System;

public class HealthBar : Control
{
	[Signal]
	public delegate void UIPlayerHealthChanged();
	private VBoxContainer vContainer;
	private Label lifeValue;
	private TextureProgress healthOrb;
	public override void _Ready() {
		vContainer = GetNode<VBoxContainer>("VContainer");
		lifeValue = vContainer.GetNode<Label>("LifeValue");
		healthOrb = vContainer.GetNode<TextureProgress>("HealthOrb");
		Connect(nameof(UIPlayerHealthChanged), this, nameof(OnUIHealthChanged));
	}

	private void OnUIHealthChanged(int health, int maxHealth) {
		lifeValue.Text = String.Format("{0}/{1}", health, maxHealth);
		healthOrb.Value = health;
	}
}
