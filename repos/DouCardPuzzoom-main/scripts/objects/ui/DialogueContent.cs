using Godot;
using System;

public partial class DialogueContent : RichTextLabel
{
    public event Action DialogueKeepGoing;
    
    // 实现动画未结束时点击，直接结束动画，需要另外传递参数/委托，太恶心了，算了
    
    public override void _GuiInput(InputEvent @event) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }
        
        DialogueKeepGoing?.Invoke();
    }
}
