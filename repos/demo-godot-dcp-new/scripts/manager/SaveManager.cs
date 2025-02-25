using Godot;

namespace DouCardPuzzoom.scripts.manager; 

// 管理游戏存档
public static class SaveManager {
    // public static string SaveName;
    public static readonly Vector2 NullVector = new Vector2(-999, -999);

    public static Vector2 PlayerPositionScene1 = new Vector2(350, 50); // 默认位置：楼梯口
    public static Vector2 PlayerPositionScene2 = new Vector2(350, 50);
    public static Vector2 PlayerPositionScene3 = new Vector2(158, 50);

    public static Vector2 LastPlayerPosition = NullVector;

    public static bool BeforeCardClip = true;

    public static void ResetPosition(int except) {
        switch (except) {
            case 1:
                PlayerPositionScene2 = new Vector2(350, 50);
                PlayerPositionScene3 = new Vector2(158, 50);
                break;
            case 2:
                PlayerPositionScene1 = new Vector2(350, 50);
                PlayerPositionScene3 = new Vector2(158, 50);
                break;
            case 3:
                PlayerPositionScene1 = new Vector2(350, 50);
                PlayerPositionScene2 = new Vector2(350, 50);
                break;
            default:
                break;
        }
    }

    public static Vector2 GetPosition(int i) {
        if (IsLastPositionNull()) {
            switch (i) {
                case 1: return PlayerPositionScene1;
                case 2: return PlayerPositionScene2;
                case 3: return PlayerPositionScene3;
            }
        }
        var newPos = new Vector2(LastPlayerPosition.X, LastPlayerPosition.Y);
        LastPlayerPosition = NullVector;
        return newPos;
    }
    
    public static bool IsLastPositionNull() {
        return LastPlayerPosition == NullVector;
    }

    // 必须记录上一次玩家所在的位置……

    // public static void ChangePositionByLevelKey(string level) {
    //     var levelFirst = level.Split("_")[1];
    //     switch (levelFirst) {
    //         case "1":
    //             
    //     }
    // }
}