using Godot;
using System;

public class EventBus : Node
{
    // Singleton pattern
    private static EventBus instance;
    public static EventBus Instance {
        get {
            if(instance == null) instance = new EventBus();
            return instance;
        }
    }

    // Signals
    [Signal]
    public delegate void PlayerHealthChanged();
    [Signal]
    public delegate void PlayerVelocityChanged();
    [Signal]
    public delegate void InitializePlayer();
}
