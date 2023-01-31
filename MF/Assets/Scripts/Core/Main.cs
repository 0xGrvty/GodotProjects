using Godot;
using System;

public class Main : Node
{
    [Signal]
    private delegate void EnemyDied();
    [Export]
    public PackedScene RockManScene;
    private Node2D follow;
    private Vector2 cameraBounds;
    private float enemySpawnTimer = 0;
    private Node2D player;
    private Camera2D mainCamera;
    private Control UI;

    // Called when the node enters the scene tree for the first time.
    // Think of this class as a taskmaster.
    // Main will divvy up all the tasks, such as connecting instanced enemy signals, to whatever might need to observe those signals.
    public override void _Ready()
    {
        cameraBounds = new Vector2(50, 50);
        enemySpawnTimer = 3f;
        player = GetNode<KinematicBody2D>("Player");
        follow = player;
        mainCamera = player.GetNode<Camera2D>("Camera2D");
        UI = GetNode<Control>("CanvasLayer/UI");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _PhysicsProcess(float delta) {
        enemySpawnTimer -= delta;
        if (enemySpawnTimer <= 0) {
            enemySpawnTimer = 3f;
            var rockMan = (RockMan)RockManScene.Instance();
            var enemySpawnLocation = GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");
            enemySpawnLocation.Offset = GD.Randi();
            rockMan.Position = enemySpawnLocation.Position;
            rockMan.Spawn(player);
            AddChild(rockMan);
            // Programmatically connect this signal since our enemy is added programmatically and not packed as a scene
            rockMan.Connect(nameof(Enemy.Died), UI, "OnEnemyDied");
        }
    }

}
