using Godot;
using System;

public class Health : Node
{
    [Signal]
    public delegate void HealthChanged(int health);
    [Signal]
    public delegate void HealthDepleted();

    [Export]
    public int health = 10;
    [Export]
    private int maxHealth = 100;

    public override void _Ready() {
        GD.PrintS("Max Value in health.cs instantiation: " + maxHealth);
        health = maxHealth;
        EmitSignal("HealthChanged", health);
    }

    public void TakeDamage(int damage) {
        health -= damage;
        health = Math.Max(0, health);
        EmitSignal("HealthChanged", health);
    }

    public void Heal(int heal) {
        health += heal;
        health = Math.Max(health, maxHealth);
        EmitSignal("HealthChanged", health);
    }
    public int GetMaxHealth() {
        return maxHealth;
    }
}
