using Godot;
using System;

public interface IStateMachine {
	IStateMachine EnterState(PlayerBody player);
	void EmitChangeStateSignal(PlayerBody player, IStateMachine state);
}
