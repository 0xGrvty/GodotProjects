using Godot;
using System;

public class StatsContainer : Control
{
    [Signal]
    public delegate void StatsChanged(Godot.Collections.Dictionary d);
    [Signal]
    public delegate void EnemyDied();
    private Control stats;
    public override void _Ready()
    {
        stats = GetNode<Control>("Stats");
    }

    public void OnUIEnemyDied() {
        EmitSignal(nameof(EnemyDied));
    }

}
