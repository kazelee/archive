using Godot;
using System;

public partial class DialoguePanel : Panel {
    public event Action DialogueKeepGoing;

    // input 是全局，会导致按下skip先出发下一个，再进入默认对话
    // gui input则会出现之前的button遮挡问题，导致只有点边缘才能交互，点文字都不行
    // 要不然就在文字上加一个吧……，其他就算了
    public override void _GuiInput(InputEvent @event) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }
        
        DialogueKeepGoing?.Invoke();
    }
}
