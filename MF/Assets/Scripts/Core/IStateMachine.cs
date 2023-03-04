using Godot;
using System;

public interface IStateMachine {
	IStateMachine EnterState(Node2D player);
	void EmitChangeStateSignal(Node2D player, IStateMachine state);
}
