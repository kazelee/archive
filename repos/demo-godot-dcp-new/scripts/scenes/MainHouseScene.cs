using DouCardPuzzoom.scripts.manager;
using Godot;

namespace DouCardPuzzoom.scripts.scenes; 

public partial class MainHouseScene : Sprite2D {
    public Player Player;
    
    // 作为场景的基类，用于转换时的判断
    public override void _Ready() {
        base._Ready();
        // 每次初始化场景时都设置为箭头！
        Input.SetCustomMouseCursor(MouseManager.Arrow);
    }
}