using Godot;
using System;
using System.Threading;

public partial class LoadZone : Node2D {
	private SceneManager sceneManager;
	private Hitbox loadZoneHitbox;
	private Player player;
	[Export]
	private string sceneTo;
	[Export]
	private int dir;
	public override void _Ready() {
		loadZoneHitbox = GetNode<Hitbox>("Hitbox");
		//player = (Player)GetParent().GetNode<Node2D>("Player");
		foreach (var actor in GetTree().GetNodesInGroup("Actors")) {
			if (actor is Player) {
				player = (Player)actor;
				break;
			}
		}
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

	}

    public override void _Process(double delta) {
        if (loadZoneHitbox.Intersects(player.Hurtbox, Vector2.Zero) && player.currentState != player.playerSceneTransitionState) {
			player.EmitSignal("LoadZoneTriggered", dir);
			sceneManager.ChangeScene(sceneTo);
			QueueFree();
		}
    }
}
