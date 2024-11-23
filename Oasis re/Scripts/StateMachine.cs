using Godot;
using Godot.Collections;
using System.Runtime;
public partial class StateMachine : Node {
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

  // If you need to check if an event happened, i.e. if jump was pressed, then handle it here!
  // However, if you need to poll an input over a certain amount of time, then handle it in _PhysicsProcess!
  // This was the explanation I needed back then.  I don't know why everything I Googled was so poorly explained.
  // It's the difference of polling vs checking the event once.  Holy crap.
  public override void _UnhandledInput(InputEvent @event) {
    currentState?.HandleInput(@event);
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
    GD.Print(debugEnt + "\n" + debugExit);

    // Exit the current state if it exists before entering the new state
    currentState?.ExitState();
    newState.EnterState();
    currentState = newState;
  }

  public State GetState() { return currentState; }
  public Godot.Collections.Dictionary<StringName, State> GetStates() { return states; }
}
