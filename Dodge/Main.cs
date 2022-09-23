using Godot;
using System;

public class Main : Node {
#pragma warning disable 649

    [Export]
    public PackedScene MobScene;
    [Export]
    public PackedScene PlayerScene;
#pragma warning restore 649

    [Signal]
    public delegate void ScoreUpdated();

    public int Score;
    private bool IsGiftAwarded = false;
    private const int SCORE_REQUIREMENT_FOR_GIFT = 5;
    private const float FASTER_ATTACK_SPEED = 0.90f;
    private int ScoreSinceGift = 0;
    private int NumGifts = 0;
    Player player;

    public override void _Ready() {
        GD.Randomize();
        player = GetNode<Player>("Player");
        player.SetDisableInput(true);
        //NewGame();
    }

    public void GameOver() {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame() {
        Score = 0;
        ScoreSinceGift = 0;
        NumGifts = 0;

        //var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);
        player.SetDisableInput(false);
        player.ResetFireSpeed();
        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(Score);
        hud.ShowMessage("Get Ready!");

        // When calling Godot-named functions, need to use
        // snake_case since that's their coding style.
        // I am assuming I can call my own methods here,
        // in which case I'm using PascalCase for my functions
        GetTree().CallGroup("mobs", "queue_free");

        GetNode<AudioStreamPlayer>("Music").Play();
    }

    public void OnScoreTimerTimeout() {
        Score++;
        GetNode<HUD>("HUD").UpdateScore(Score);
        //if (Score % SCORE_REQUIREMENT_FOR_GIFT == 0) {
        //    Console.WriteLine("Modulo'd");
        //    EmitSignal(nameof(ScoreUpdated));
        //}
        EmitSignal(nameof(ScoreUpdated));
    }

    public void OnStartTimerTimeout() {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void OnMobTimerTimeout() {

        // Create a new instance of the Mob scene
        var mob = (Mob)MobScene.Instance();

        // Choose a random location on Path2D
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.Offset = GD.Randi();

        // SEt the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // The mob announced it, so we need to make sure we listen.
        mob.Connect("Died", this, "OnMobDied");

        // Spawn the mob by adding it to the Main Scene
        AddChild(mob);
    }

    // Connect the signal
    public void OnMobDied() {
        Score += 10;
        GetNode<HUD>("HUD").UpdateScore(Score);
        EmitSignal(nameof(ScoreUpdated));
    }

    private void OnMainScoreUpdated() {
        var temp = Score - SCORE_REQUIREMENT_FOR_GIFT;
        // Give all the gifts all at once, not sure if this is a good way of doing it.  Will need to research
        while (!player.GetMaxFireDelayReached() && temp >= ScoreSinceGift) {
            NumGifts++;
            ScoreSinceGift = Math.Min(NumGifts * SCORE_REQUIREMENT_FOR_GIFT, Score);
            Console.WriteLine("NumGifts = " + NumGifts);
            Console.WriteLine("ScoreSinceGift = " + ScoreSinceGift);
            //EmitSignal(nameof(ScoreUpdated));
            player.ReduceFiringSpeed(FASTER_ATTACK_SPEED);
        }
    }
}
