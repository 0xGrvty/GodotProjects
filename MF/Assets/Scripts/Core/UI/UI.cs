using Godot;
using System;

public class UI : Control
{
    [Signal]
    public delegate void HealthChanged(int health);

    public override void _Ready()
    {
        HealthListener healthListener = null;
        Connect("HealthChanged", this, "OnHealthChanged");
        foreach (Node n in GetTree().GetNodesInGroup("actors")) {
            if (n.Name == "Player") {
                healthListener = (HealthListener)n.GetNode<Node>("HealthListener");
                break;
            }
        }
        GD.Print("Health Listener found.  Max health: " + healthListener.GetMaxHealth());
        //EmitSignal("HealthChanged", healthListener.health, healthListener.GetMaxHealth());
    }

    public void OnHealthChanged(int health, int maxHealth) {
        EmitSignal("HealthChanged", health, maxHealth);
    }

}
