using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;
using Godot;

public partial class GameScene : Sprite2D {
    public Button ModeLabelButton;
    
    public override void _Ready() {
        // 显示关卡类型的 可点击文字 button
        ModeLabelButton = GetNode<Button>("ModeLabel/Button");
        ModeLabelButton.Pressed += OnModeLabelButtonPressed;
        
        StateManager.Reset();
    }

    public void OnModeLabelButtonPressed() {
        GD.Print("Pressed!");
    }
    
}
