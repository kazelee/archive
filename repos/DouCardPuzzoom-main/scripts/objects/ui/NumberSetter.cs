using Godot;
using System;

public partial class NumberSetter : VBoxContainer {
    public TextureButton ButtonUp;
    public TextureButton ButtonDown;
    public Label NumLabel;
    public override void _Ready() {
        base._Ready();
        ButtonUp = GetNode<TextureButton>("ButtonUp");
        ButtonDown = GetNode<TextureButton>("ButtonDown");
        NumLabel = GetNode<Label>("TextureRect/Label");
        NumLabel.Text = "0";

        ButtonUp.Pressed += () => {
            NumLabel.Text = ((NumLabel.Text.ToInt() + 9) % 10).ToString();
        };

        ButtonDown.Pressed += () => {
            NumLabel.Text = ((NumLabel.Text.ToInt() + 1) % 10).ToString();
        };
    }
}
