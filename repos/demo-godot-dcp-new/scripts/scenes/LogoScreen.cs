using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.manager;

public partial class LogoScreen : Sprite2D
{
    public override void _Ready() {
        // 初始化更改鼠标样式
        Input.SetCustomMouseCursor(MouseManager.Arrow);
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlaySoundEffects("soft-piano-logo");
        
        var timer = new Timer();
        AddChild(timer);
        timer.OneShot = true;
        timer.Timeout += OnTimeout;
        timer.Start(0.5); // 2 by default, 0.5 for test
		
        // 加载规则数据字典
        DataLoader.Ready();
    }
    
    private void OnTimeout() {
        var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
        sceneChanger.ChangeScene("res://scenes/TitleScreen.tscn");
    }
}
