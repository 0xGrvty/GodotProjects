using Godot;
using System;

public partial class CameraShake : Camera2D {
    // Inspector exports
    [Export]
    private double amplitude = 2f;
    [Export]
    private float duration = 0.8f;
    [Export(PropertyHint.ExpEasing)]
    private float DAMP_EASING = 1.0f;
    [Export]
    private bool shake = false;

    // Private variables
    private Timer timer;
    private bool enabled = false;

    public bool Shake { get => shake; set => SetShake(value); }
    public float Duration { get => duration; set => SetDuration(value); }

    public override void _Ready() {
        GD.Randomize();
        SetProcess(false);
        timer = GetNode<Timer>("ShakeTimer");
        
        ConnectToShakers();
    }

    public override void _Process(double delta) {
        var damping = (float)Mathf.Ease(timer.TimeLeft / timer.WaitTime, DAMP_EASING);
        Offset = new Vector2(
            (float)GD.RandRange(amplitude, -amplitude) * damping,
            (float)GD.RandRange(amplitude, -amplitude) * damping
            );
    }

    public void SetShake(bool shake) {
        this.shake = shake;
        SetProcess(this.shake);
        Offset = new Vector2();
        if (this.shake) timer.Start();
    }

    public void SetDuration(float duration) {
        this.duration = duration;
        timer.WaitTime = this.duration;
    }

    public void OnShakeTimerTimeout() {
        SetShake(false);
    }

    public void HandleShake(bool enabled) {
        this.enabled = enabled;
        if (!this.enabled) return;
        SetShake(this.enabled);
    }

    private void ConnectToShakers() {
        foreach (var shaker in GetTree().GetNodesInGroup("CameraShakers")) {
            shaker.Connect("ShakeCamera", new Callable(this, nameof(HandleShake)));
        }
    }

}
