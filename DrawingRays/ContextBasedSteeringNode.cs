using Godot;
using System;

public class ContextBasedSteeringNode : Node2D
{
    public enum NodeType {
        INTEREST = 0,
        DANGER = 1,
        NO_INTEREST = 2
    };

    private NodeType type;

    public ContextBasedSteeringNode() {
        type = NodeType.NO_INTEREST;
    }

    public ContextBasedSteeringNode(NodeType type, Vector2 pos) {
        this.type = type;
        Position = pos;
    }

    public void Init(NodeType type, Vector2 pos, Color c) {
        this.type = type;
        Position = pos;
        Modulate = c;
    }

    public NodeType GetNodeType() {
        return type;
    }
}
