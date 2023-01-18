using Godot;
using System;

public class Main : Node
{
    [Export]
    public PackedScene RockManScene;

    private Camera2D mainCamera;
    private Node2D follow;
    private Vector2 cameraBounds;
    private float enemySpawnTimer = 0;
    private Node2D player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mainCamera = GetNode<Camera2D>("Camera2D");
        cameraBounds = new Vector2(50, 50);
        enemySpawnTimer = 3f;
        player = GetNode<Node2D>("Player/PlayerBody");
        follow = player;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _PhysicsProcess(float delta) {
        mainCamera.Align();
        mainCamera.Position = follow.Position;
        enemySpawnTimer -= delta;
        if (enemySpawnTimer <= 0) {
            enemySpawnTimer = 3f;
            var rockMan = (RockMan)RockManScene.Instance();
            var enemySpawnLocation = GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");
            enemySpawnLocation.Offset = GD.Randi();
            rockMan.Position = enemySpawnLocation.Position;
            rockMan.Spawn(player);
            AddChild(rockMan);
        }
    }

}
