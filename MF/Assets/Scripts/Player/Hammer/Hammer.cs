using Godot;
using Godot.Collections;
using System;
using System.Collections;

public class Hammer : Area2D {
    private Vector2 spawnPosition;
    private float theta;
    private float frequency;
    private float radius;
    private float maxTimeAlive;
    private float rotationAngle;

    private const float BASE_RADIUS = 50;
    private const float RADIUS_GROWTH = 70;
    private const float MAX_SPEED = 12f;
    private const float BASE_TIME_ALIVE = 3f;

    private Dictionary hammerDictionary = new Dictionary();
    private float xOffset = 0f;
    private int xDirection = 1;
    private float yOffset = 0f;
    private int yDirection = 1;
    private float rMultiplier = 1f;

    private void DefineDeaultDictionary() {
        this.hammerDictionary.Add("xOffset", xOffset);
        this.hammerDictionary.Add("xDirection", xDirection);
        this.hammerDictionary.Add("yOffset", yOffset);
        this.hammerDictionary.Add("yDirection", yDirection);
        this.hammerDictionary.Add("rMultiplierX", rMultiplier);
        this.hammerDictionary.Add("rMultiplierY", rMultiplier);
        this.hammerDictionary.Add("baseRadius", BASE_RADIUS);
        this.hammerDictionary.Add("radiusGrowth", RADIUS_GROWTH);
        this.hammerDictionary.Add("maxSpeed", MAX_SPEED);
        this.hammerDictionary.Add("baseTimeAlive", BASE_TIME_ALIVE);
    }
    public void Init(Vector2 SpawnPosition, Dictionary hammerDictionary) {
        DefineDeaultDictionary();
        GD.Print(hammerDictionary);
        if (hammerDictionary.Count == 0) {
            hammerDictionary = this.hammerDictionary;
        }
        this.hammerDictionary = hammerDictionary.Duplicate();
        this.spawnPosition = SpawnPosition;
        Position = SpawnPosition;
        GD.Print("Starting Position: " + Position);
        frequency = (float)hammerDictionary["maxSpeed"];
        radius = (float)hammerDictionary["baseRadius"];
        maxTimeAlive = (float)hammerDictionary["baseTimeAlive"];
        theta = 0; // 0 radians
        rotationAngle = 0; // 0 radians
    }

    public override void _PhysicsProcess(float delta) {
        // Mathematics of a circle: x^2 + y^2 = r, but this is in rectangular coordinates
        // Which makes it a little hard to figure out.  We can convert this to polar coordinates:
        // Look at notes for pictures.
        // x = rcos(theta)
        // y = rsin(theta)
        // Frequency is whatever is in the parenthesis: rcos(theta * frequency) (we can think of it as speed too)

        // Change Theta in respects to time
        theta += Mathf.Pi / 6 * delta;
        //radius += RADIUS_GROWTH * delta;
        rotationAngle += Mathf.Pi / 2.75f * delta;
        //Position = new Vector2(Mathf.Cos(theta * frequency) * radius, Mathf.Sin(theta * frequency) * radius) + spawnPosition;
        Position = new Vector2((float)hammerDictionary["xOffset"] + (float)hammerDictionary["xDirection"] * Mathf.Cos(theta * frequency) * radius * (float)hammerDictionary["rMultiplierX"],
            (float)hammerDictionary["yOffset"] + (float)hammerDictionary["yDirection"] * Mathf.Sin(theta * frequency) * radius * (float)hammerDictionary["rMultiplierY"]) + spawnPosition;
        Rotation = rotationAngle * frequency;
        maxTimeAlive -= delta;
        if (maxTimeAlive <= 0 || Position.x == spawnPosition.x) {
            GD.Print("Queue Free Position: " + Position);
            QueueFree();
        }
    }

}
