using Godot;
using System.Collections.Generic;

public class Hammer : Area2D {

    private const float BASE_RADIUS = 50;
    private const float RADIUS_GROWTH_SPEED = 8f;
    private const float PI_6 = Mathf.Pi / 6f;
    private const float MAX_SPEED = 12f;
    private const float BASE_TIME_ALIVE = 3f;
    private const float BASE_DAMAGE = 50.0f;
    private const float ROTATION_COEFFICIENT = Mathf.Pi / 2.75f;

    private float damage;
    private Node2D source;
    private float phaseShift;
    private float frequency;
    private float radius;
    private float maxTimeAlive;
    private float rotationAngle;
    //private float xOffset;
    //private int xDirection;
    //private float yOffset;
    //private int yDirection;
    //private float amplitudeX;
    //private float amplitudeY;
    private List<Node2D> hitList;
    private AudioStreamPlayer2D audio;
    private Vector2 prevPos = Vector2.Zero;

    public async void Init(Node2D source, float phaseShift, int numHams = 1) {
        radius = 2f;
        this.source = source;
        Position = source.Position;
        this.phaseShift = phaseShift;
        //GD.Print(phaseShift);
        frequency = 15f;
        maxTimeAlive = BASE_TIME_ALIVE;
        rotationAngle = 0;
        // May not need these below stats
        //xOffset = 0f;
        //yOffset = 0f;
        ////xDirection = source.GetAnimatedSprite().FlipH ? -1 : 1;
        //xDirection = 1;
        //yDirection = 1;
        //amplitudeX = 1f;
        //amplitudeY = 1f;
        damage = BASE_DAMAGE;
        hitList = new List<Node2D>();
        audio = GetNode<AudioStreamPlayer2D>("BlessedHammerSound");
        audio.Play();
        //GD.Print(this.positionAngle);
        //GD.Print(Position);

        // Create a timer to kill the hammer so it doesn't spin forever
        await ToSignal(source.GetTree().CreateTimer(maxTimeAlive, false), "timeout");
        QueueFree();
    }

    
    public override void _PhysicsProcess(float delta) {
        prevPos = Position;
        // Mathematics of a circle:
        // x = r * B * cos(phaseShift * frequency)
        // y = r * B * sin(phaseShift * frequency)
        // where B is some constant
        // where frequency is how fast the object is rotating
        // where r is the radius
        // for a growing radius, we can do: (r + growth) * B * cos(frequency)
        // and expanding further, we can do: (r + (growth * delta)) * B * cos(frequency)
        // to get some growth in respects to time

        // Change the phaseShift in respects to time

        // phaseShift = the position at which the hammer is orbiting in a circle
        // rotationAngle = the rotation of the object itself

        // This is fine since this is an Area2D and is not directly interfacing with physics.
        // If this was a kinematic object, then this is not acceptable
        Position += new Vector2(Mathf.Sin(phaseShift) * radius, Mathf.Cos(phaseShift) * radius);

        // This was the reason why the hammers weren't spawning as I expected before.  Needed to modify the phaseShift after calculating the new position, not before it.
        phaseShift += PI_6 * frequency * delta;
        radius += RADIUS_GROWTH_SPEED * delta;
        rotationAngle -= ROTATION_COEFFICIENT * delta;
        Rotation = rotationAngle * frequency;
    }

    public void OnHammerBodyEntered(Node2D body) {
        //body.CallDeferred("TakeDamage", damage);
        if (hitList.Contains(body)) {
            return;
        }
        hitList.Add(body);
        body.EmitSignal("Hit", damage, source);
    }

}
