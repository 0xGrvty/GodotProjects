using Godot;
using System;

public interface IStateMachine {
    IStateMachine EnterState(Player player);
    void EmitChangeStateSignal(Player player, IStateMachine state);
}
