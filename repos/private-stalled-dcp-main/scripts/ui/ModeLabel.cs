using Godot;
using System;

public partial class ModeLabel : RichTextLabel {
    public Button Button;

    public override void _Ready() {
        Button = GetNode<Button>("Button");
        // button 的大小等于 label 的大小
        Button.Size = Size;
    }
    
    
}
