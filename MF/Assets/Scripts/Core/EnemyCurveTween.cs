using Godot;
using System;
using System.Diagnostics;

public class EnemyCurveTween : Tween
{
    [Signal]
    public delegate void CurveTween(float sat);

    [Export]
    public Curve curve;

    private float start = 0.0f;
    private float end = 1.0f;

    public override void _Ready()
    {
        
    }

    public void Play(float duration = 1.0f, float startIn = 0.0f, float endIn = 0.0f) {
        Debug.Assert(curve.Equals(null), "This CurveTween needs a curve added in the inspector");
        start = startIn;
        end = endIn;
        InterpolateMethod(this, "Interp", 0.0, 1.0, duration, Tween.TransitionType.Linear, Tween.EaseType.In);
        Start();
    }

    public void Interp (float sat) {
        EmitSignal("CurveTween", start + ((end - start) * curve.Interpolate(sat)));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
