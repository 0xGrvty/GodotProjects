using Godot;
using System;

public class HealthBarContainer : Control
{
    private void OnUIHealthChanged(int health, int maxHealth) {
        GetNode<Label>("VContainer/LifeValue").Text = String.Format("{0}/{1}", health, maxHealth);
        GetNode<TextureProgress>("VContainer/HealthOrb").Value = health;
    }
}
