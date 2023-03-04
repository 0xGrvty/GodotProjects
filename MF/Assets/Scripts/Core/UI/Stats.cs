using Godot;
using System;

public class Stats : Control
{
    [Signal]
    public delegate void EnemyDied();
    [Signal]
    public delegate void PlayerMoveSpeedChanged();
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
        killsLabel.Text = String.Format("SCORE = {0}", killCounter);
        Connect(nameof(EnemyDied), this, nameof(OnEnemyDied));
        Connect(nameof(PlayerMoveSpeedChanged), this, nameof(OnPlayerMoveSpeedChanged));
    }

    private void OnEnemyDied() {
        killCounter++;
        killsLabel.Text = String.Format("SCORE = {0}", killCounter);
    }

    private void OnPlayerMoveSpeedChanged(int speed) {
        speedLabel.Text = String.Format("Speed: {0}", speed);
    }
}
