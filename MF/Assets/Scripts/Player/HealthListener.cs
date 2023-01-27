using Godot;
using System;

public class HealthListener : Node
{
    [Signal]
    public delegate void HealthListenerChanged(int health, int maxHealth);
    [Signal]
    public delegate void TakeDamage(int health);
    [Signal]
    public delegate void HealDamage(int health);

    public int health;
    private int maxHealth;

    public override void _Ready() {
        //EmitSignal("HealthChanged", health);
    }

    public void Init(int health, int maxHealth) {
        this.health = health;
        this.maxHealth = maxHealth;
        //GD.Print(String.Format("Initial Health: {0}\nMax Health: {1}", health, maxHealth));
        EmitSignal("HealthListenerChanged", health, maxHealth);
    }

    public void OnTakeDamage(int health) {
        //health -= damage;
        //health = Math.Max(0, health);
        
        EmitSignal("HealthListenerChanged", health, maxHealth);
    }

    public void OnHealDamage(int health) {
        //health += heal;
        //health = Math.Min(health, maxHealth);
        
        EmitSignal("HealthListenerChanged", health, maxHealth);
    }
    public int GetMaxHealth() {
        return maxHealth;
    }
}
