using Godot;
using Godot.Collections;
using System.Runtime;
public partial class StateMachine : Node {
  [Export]
  private State initState;
  [Export]
  private State currentState;

  private Godot.Collections.Dictionary<StringName, State> states;

  private bool debug = false;

  public override void _Ready() {
    states = new Godot.Collections.Dictionary<StringName, State>();
    foreach (var child in GetChildren()) {
      if (child is State c) {
        states[c.Name.ToString().ToLower()] = c;
        c.Connect(State.SignalName.StateFinished, new Callable(this, MethodName.OnStateFinished));
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

  private void OnStateFinished(State state, StringName newStateName) {
    if (state != currentState)  return; 
    var newState = states?[newStateName.ToString().ToLower()];
    if (newState == null) {
      GD.Print("State does not exist: " + state);
      return;
    }

    // Debugging prints
    var owner = GetParent()?.Name;
    var debugEnt = $"{owner} ENTERING: " + newState.Name;
    var debugExit = $"{owner} EXITING: " + currentState.Name;
    if (debug) GD.Print(debugEnt + "\n" + debugExit);

    // Exit the current state if it exists before entering the new state
    currentState?.ExitState();
    newState.EnterState();
    currentState = newState;
  }

  public State GetState() { return currentState; }
  public Godot.Collections.Dictionary<StringName, State> GetStates() { return states; }
}
