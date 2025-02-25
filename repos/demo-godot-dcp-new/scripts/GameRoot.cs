using Godot;
using System;
using DouCardPuzzoom.scripts;

public partial class GameRoot : Node2D
{
    // 参考：[保存游戏(写入文件) - 知乎](https://zhuanlan.zhihu.com/p/62985708)
    // 每隔1min保存一次，每次切换场景（退出对话、GS)时保存数据？

    public const int SaveSeconds = 60;
    public double CurrentTime = 0;
    
    // 每隔1min保存一次数据
    public override void _Process(double delta) {
        CurrentTime += delta;
        if (CurrentTime > SaveSeconds && DataLoader.CurrentSave != null) { // 避免单场景测试出问题（还是耦合性太强）
            DataLoader.StoreCurrentSave();
            CurrentTime = 0;
        }
    }
    
    // public override void _Notification(int what) {
    //     if (what == NotificationWMCloseRequest) {
    //         DataLoader.StoreCurrentSave();
    //         GetTree().Quit(); // default behavior
    //     }
    // }
}
