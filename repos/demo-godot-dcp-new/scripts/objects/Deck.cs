using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class Deck : Area2D
{
    public event Action OnInteracted;

    public Sprite2D Sprite2D;
    public Label LevelName;
    public Label Progress;
    
    // 控件的问题，直接选择mouse ignore就行了！！！费那么多事……
    
    public override void _Ready() {
        base._Ready();
        Sprite2D = GetNode<Sprite2D>("Sprite2D");
        LevelName = GetNode<Label>("LevelName");
        Progress = GetNode<Label>("Progress");
        
        MouseEntered += () => {
            Sprite2D.Modulate = Colors.Gray;
            Input.SetCustomMouseCursor(MouseManager.Click);
        };
        MouseExited += () => {
            Sprite2D.Modulate = Colors.White;
            Input.SetCustomMouseCursor(MouseManager.Arrow);
        };
    }
    
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact")) { // Deck 是在UI中的，所以不需要
            return;
        }
        
        OnInteracted?.Invoke();
    }
}
