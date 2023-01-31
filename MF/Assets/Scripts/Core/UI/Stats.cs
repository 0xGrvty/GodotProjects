using Godot;
using System;

public class Stats : Control
{
    private VBoxContainer vbox;
    private Label speedLabel;
    private Label damageLabel;
    private Label killsLabel;

    private int killCounter;
    public override void _Ready()
    {
        vbox = GetNode<VBoxContainer>("VBoxContainer");
        speedLabel = vbox.GetNode<Label>("Speed");
        damageLabel = vbox.GetNode<Label>("Damage");
        killsLabel = vbox.GetNode<Label>("Kills");
        killCounter = 0;
        killsLabel.Text = String.Format("Kills: {0}", killCounter);
    }

    private void OnStatsContainerEnemyDied() {
        killCounter++;
        killsLabel.Text = String.Format("Kills: {0}", killCounter);
    }
}
