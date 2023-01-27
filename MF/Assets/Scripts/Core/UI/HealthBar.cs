using Godot;
using System;

public class HealthBar : Control
{
    private TextureProgress tp;
    private Label lv;
    private VBoxContainer vBoxContainer;

    public override void _Ready()
    {
        //vBoxContainer = GetNode<VBoxContainer>("VContainer");
        //tp = vBoxContainer.GetNode<TextureProgress>("HealthOrb");
        //lv = vBoxContainer.GetNode<Label>("LifeValue");
    }

    private void OnUIHealthChanged(int health, int maxHealth) {
        GetNode<Label>("VContainer/LifeValue").Text = String.Format("{0}/{1}", health, maxHealth);
    }
}
