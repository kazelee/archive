using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class HangTagArea : Area2D
{
    public event Action OnHangTagTouched;

    public Sprite2D HangTagSprite;
    public Label Label;
    
    public override void _Ready() {
        base._Ready();
        HangTagSprite = GetNode<Sprite2D>("Sprite2D");
        Label = GetNode<Label>("Sprite2D/Label");
        MouseEntered += () => {
            if (!MouseManager.IsInterAreaAble) return; // 有界面时，禁止交互
            HangTagSprite.Modulate = Colors.Gray;
            Input.SetCustomMouseCursor(MouseManager.Click);
        };
        MouseExited += () => {
            HangTagSprite.Modulate = Colors.White;
            Input.SetCustomMouseCursor(MouseManager.Arrow);
        };
    }
    
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }

        if (!MouseManager.IsInterAreaAble) return; // 防止不能点击时也点击发出声音
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlayAreaEffects("high-click");
        // soundManager.AreaSFXPlayer.Play(0.5f);
        OnHangTagTouched?.Invoke();
    }
}
