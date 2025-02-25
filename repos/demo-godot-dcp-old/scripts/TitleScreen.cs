using Godot;
using System;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class TitleScreen : TextureRect {
    public override void _Ready() {
        var timer = new Timer();
        AddChild(timer);
        timer.OneShot = true;
        timer.Timeout += OnTimeout;
        timer.Start(1.5);
        
        RuleEngine.Start();
    }

    private void OnTimeout() {
        GetTree().ChangeSceneToFile("res://scenes/GameScene.tscn");
    }
}
