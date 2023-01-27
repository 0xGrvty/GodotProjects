using Godot;
using System;

public class HealthListener : Node
{
    [Signal]
    public delegate void HealthChanged(int health, int maxHealth);
    [Signal]
    public delegate void HealthDepleted();

    public int health;
    private int maxHealth;

    public override void _Ready() {
        //EmitSignal("HealthChanged", health);
    }

    public void Init(int health, int maxHealth) {
        this.health = health;
        this.maxHealth = maxHealth;
        GD.Print(String.Format("Initial Health: {0}\nMax Health: {1}", health, maxHealth));
        EmitSignal("HealthChanged", health, maxHealth);
    }

    public void OnTakeDamage() {
        //health -= damage;
        //health = Math.Max(0, health);
        
        EmitSignal("HealthChanged", health, maxHealth);
    }

    public void OnHealDamage() {
        //health += heal;
        //health = Math.Min(health, maxHealth);
        
        EmitSignal("HealthChanged", health, maxHealth);
    }
    public int GetMaxHealth() {
        return maxHealth;
    }
}
