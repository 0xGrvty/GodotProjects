using Godot;
using System;

public class LifeBar : HBoxContainer
{
    private int maximumValue;
    public void OnInterfaceInitStats(int maxHealth) {
        GD.PrintS("OnInterfaceInitStats max health: " + maximumValue);
        maximumValue = maxHealth;
    }
    public void OnInterfaceHealthChanged(int health) {
        GD.PrintS("Max Value: " + maximumValue);
        GetNode<TextureProgress>("TextureProgress").Value = health;
        GetNode<NinePatchRect>("Counter").GetNode<Label>("Number").Text = String.Format("{0} / {1}", health, maximumValue);
    }
}
