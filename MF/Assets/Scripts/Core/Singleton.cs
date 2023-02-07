using Godot;
using System;

public class Singleton : Node
{
    public string mainMenuPath = "res://Scenes/MainMenu.tscn";
    private static Singleton instance;
    public static Singleton Instance {
        get {
            if (instance == null) instance = new Singleton();
            return instance;
        }
    }

}
