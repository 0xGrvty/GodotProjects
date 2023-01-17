using Godot;
using System;

public class Interface : Control
{
    [Signal]
    public delegate void HealthChanged(int health);
    [Signal]
    public delegate void InitStats(int initHealth);
    public override void _Ready() {
        Health healthNode = null;
        foreach (Node n in GetTree().GetNodesInGroup("actors")) {
            if (n.Name == "Player") {
                healthNode = (Health)n.GetNode<Node>("Player/Health");
                break;
            }
        }
        GD.PrintS("Max value in interface.cs: " + healthNode.GetMaxHealth());
        EmitSignal("InitStats", healthNode.GetMaxHealth());
        EmitSignal("HealthChanged", healthNode.health);
    }
    public void OnHealthChanged(int health) {
        EmitSignal("HealthChanged", health);
    }

}
