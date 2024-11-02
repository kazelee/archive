using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class InterArea : Area2D {
    public event Action OnInteracted;

    public Sprite2D Sprite2D;
    
    public override void _Ready() {
        base._Ready();
        Sprite2D = GetNode<Sprite2D>("Sprite2D");
        MouseEntered += () => {
            if (!MouseManager.IsInterAreaAble) return; // 有界面时，禁止交互
            Sprite2D.Modulate = Colors.Gray;
            Input.SetCustomMouseCursor(MouseManager.Click);
        };
        MouseExited += () => {
            Sprite2D.Modulate = Colors.White;
            Input.SetCustomMouseCursor(MouseManager.Arrow);
        };
    }
    
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact") || !MouseManager.IsInterAreaAble) { // 不可交互时不可点击
            return;
        }
        
        OnInteracted?.Invoke();
    }
}
