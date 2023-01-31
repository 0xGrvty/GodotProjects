using Godot;
using System;

public class Stats : Control
{
    private VBoxContainer vbox;
    private Label speedLabel;
    private Label damageLabel;
    private Label killsLabel;
    public override void _Ready()
    {
        vbox = GetNode<VBoxContainer>("VBoxContainer");
        speedLabel = vbox.GetNode<Label>("Speed");
        damageLabel = vbox.GetNode<Label>("Damage");
        killsLabel = vbox.GetNode<Label>("Kills");
    }

    private void OnStatsContainerEnemyDied() {
        killsLabel.Text = 100.ToString();
    }
}
