using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneManager : Node {
    private static AnimationPlayer animationPlayer;
    private static Player player;
    public override void _Ready() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        player = (Player)GetParent().GetNode("Game/Player");
    }

    public async void ChangeScene(string scenePath) {
        animationPlayer.Play("dissolve");
        await ToSignal(animationPlayer, "animation_finished");
        GetTree().ChangeSceneToFile(scenePath);
        animationPlayer.PlayBackwards("dissolve");
        await ToSignal(animationPlayer, "animation_finished");
        player.currentState = player.playerIdleState;
    }
}
