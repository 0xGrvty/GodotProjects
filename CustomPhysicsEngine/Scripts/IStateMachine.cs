using Godot;
using System;

public partial interface IStateMachine
{
    IStateMachine EnterState(Node actor);
    void EmitStateChanged(Node actor, IStateMachine state);
}
