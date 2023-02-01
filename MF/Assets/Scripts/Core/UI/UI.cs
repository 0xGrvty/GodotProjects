using Godot;
using System;

// Godot's signals are very powerful and through the use of signal propogation,
// a scene is able to receive signals from other scenes and divide tasks amongst 
// its own child scenes.  Take this UI node for example.
// The UI node has the following children:
// - StatsContainer (to be completed)
// - Stats
// - DebugStatsContainer (to be completed)
// - VisualStatsContainer
// - - HealthBar
// The UI class takes a signal from the player
// and the UI class then propogates that signal down to the Health Orb.
// Pretty cool stuff.  I should learn how to really utilize this more.
public class UI : Control {

    [Signal]
    public delegate void PlayerHealthChanged();
    private Stats stats;
    private HealthBar healthBar;
    //private DebugContainer debugContainer;

    public override void _Ready() {
        // Let's try to decouple this even more.  We have a main root scene we can utilize
        //HealthListener healthListener = null;
        //PlayerBody player = null;
        //foreach (Node n in GetTree().GetNodesInGroup("actors")) {
        //    if (n.Name == "Player") {
        //        healthListener = (HealthListener)n.GetNode<Node>("HealthListener");
        //        player = (PlayerBody)n;
        //        break;
        //    }
        //}
        //GD.Print("Health Listener found.  Max health: " + healthListener.GetMaxHealth());
        stats = (Stats)GetNode<Control>("StatsContainer/Stats");
        healthBar = (HealthBar)GetNode<Control>("VisualStatsContainer/HealthBar");
        //Connect(nameof(PlayerHealthChanged), this, nameof(OnPlayerHealthChanged));
    }

    // On HealthListener Changed
    public void OnHealthListenerChanged(int health, int maxHealth) {
        EmitSignal(nameof(HealthBar.UIPlayerHealthChanged), health, maxHealth);
    }

    public void OnPlayerHealthChanged(int health, int maxHealth) {
        healthBar.EmitSignal(nameof(HealthBar.UIPlayerHealthChanged), health, maxHealth);
    }

    // On Enemy Died
    public void OnEnemyDied() {
        stats.EmitSignal(nameof(Stats.EnemyDied));
    }

    public void OnPlayerVelocityChanged(int speed) {
        stats.EmitSignal(nameof(Stats.PlayerMoveSpeedChanged), speed);
    }
}
