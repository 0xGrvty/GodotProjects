using Godot;
using System;

// Godot's signals are very powerful and through the use of signal propogation,
// a scene is able to receive signals from other scenes and divide tasks amongst 
// its own child scenes.  Take this UI node for example.
// The UI node has the following children:
// - Stats (to be completed)
// - DebugStats (to be completed)
// - Visual Stats
// - - Health Orb
// The UI class takes a signal from the player's HealthListener node
// and the UI class then propogates that signal down to the Health Orb.
// Pretty cool stuff.  I should learn how to really utilize this more.
public class UI : Control
{
    [Signal]
    public delegate void HealthChanged(int health);
    [Signal]
    public delegate void EnemyDied();

    public override void _Ready()
    {
        HealthListener healthListener = null;
        foreach (Node n in GetTree().GetNodesInGroup("actors")) {
            if (n.Name == "Player") {
                healthListener = (HealthListener)n.GetNode<Node>("HealthListener");
                break;
            }
        }
        GD.Print("Health Listener found.  Max health: " + healthListener.GetMaxHealth());

        
    }

    // On HealthListener Changed
    public void OnHealthListenerChanged(int health, int maxHealth) {
        EmitSignal(nameof(HealthChanged), health, maxHealth);
    }

    // On Enemy Died
    public void OnEnemyDied() {
        GD.Print("We fucking died");
        EmitSignal(nameof(EnemyDied));
    }
}
