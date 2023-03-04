using Godot;
using System;

public class Main : Node {
    [Export]
    public PackedScene RockManScene;
    private Vector2 cameraBounds;
    private int enemies;
    private PlayerBody player;
    private Camera2D mainCamera;
    private Control userInterface;
    private bool isSpawningEnemy;

    // Called when the node enters the scene tree for the first time.
    // Think of this class as a taskmaster.
    // Main will divvy up all the tasks, such as connecting instanced enemy signals, to whatever might need to observe those signals.
    public override void _Ready() {
        cameraBounds = new Vector2(50, 50);
        enemies = 0;
        player = (PlayerBody)GetNode<KinematicBody2D>("Player");
        mainCamera = player.GetNode<Camera2D>("Camera2D");
        userInterface = GetNode<Control>("CanvasLayer/UI");
        isSpawningEnemy = false;
        // Is this a better way of doing things?
        EventBus.Instance.Connect(nameof(EventBus.PlayerHealthChanged), userInterface, "OnPlayerHealthChanged");
        EventBus.Instance.Connect(nameof(EventBus.PlayerVelocityChanged), userInterface, "OnPlayerVelocityChanged");
        EventBus.Instance.Connect(nameof(EventBus.InitializePlayer), player, "InitPlayerStats");
        EventBus.Instance.Connect(nameof(EventBus.PlayerDied), this, nameof(OnPlayerDied));
        EventBus.Instance.Connect(nameof(EventBus.EnemySpawned), this, nameof(OnEnemySpawned));
        //EventBus.Instance.Connect(nameof(EventBus.PlayerDied), this, nameof(OnPlayerDied));
        EventBus.Instance.EmitSignal(nameof(EventBus.InitializePlayer));

        //player.InitHealth(); // <- is this the right way to do things?


    }

    public async override void _PhysicsProcess(float delta) {
        await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
        var rockMan = (RockMan)RockManScene.Instance();
        var enemySpawnLocation = GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");
        enemySpawnLocation.Offset = GD.Randi();
        rockMan.Position = enemySpawnLocation.Position;
        rockMan.Spawn(player);
        AddChild(rockMan);
        // Programmatically connect this signal since our enemy is added programmatically and not packed as a scene
        rockMan.Connect(nameof(Enemy.Died), userInterface, "OnEnemyDied");
    }

    private async void OnPlayerDied() {
        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
        GetTree().ChangeScene(Singleton.Instance.mainMenuPath);
    }

    private void OnEnemySpawned() {
        enemies++;
    }
}
