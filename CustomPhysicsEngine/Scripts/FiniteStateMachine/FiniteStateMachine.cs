using Godot;
using Godot.Collections;
using System;
public partial class FiniteStateMachine : Node {

    [Export]
    private State initState;
    [Export]
    private State currentState;

    private Godot.Collections.Dictionary<StringName, State> states;

    public override void _Ready() {
        states = new Godot.Collections.Dictionary<StringName, State>();
        foreach (var child in GetChildren()) {
            if (child is State c) {
                states[c.Name.ToString().ToLower()] = c;
                c.Connect(nameof(c.StateFinished), new Callable(this, nameof(OnStateFinished)));
            }
        }

        // If it exists
        if (initState != null) {
            initState.EnterState();
            currentState = initState;
        }
    }

    // Do the current state's Update function if it exists
    public override void _Process(double delta) {
        currentState?.Update(delta);
    }

    public override void _PhysicsProcess(double delta) {
        currentState?.PhysicsUpdate(delta);
    }

    public void ChangeState(State newState) {
        // There could be a time where it is null, such as when we don't assign an initial state in the inspector
        currentState?.ExitState();

        newState.EnterState();
        currentState = newState;

        return;
    }

    private void OnStateFinished(State state, StringName newStateName) {
        if (state != currentState) { return; }
        var newState = states?[newStateName.ToString().ToLower()];
        if (newState == null) { return; }

        // Exit the current state if it exists before entering the new state
        currentState?.ExitState();
        newState.EnterState();
        currentState = newState;
    }

    public State GetState() { return currentState; }
}
