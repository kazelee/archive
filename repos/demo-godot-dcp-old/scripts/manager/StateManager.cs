namespace DouCardPuzzoom.scripts.managers; 

public static class StateManager {
    /// <summary>
    /// 出牌时的状态：第一个出牌，没牌可出，跟牌
    /// </summary>
    public enum States {
        First, None, Follow
    }

    public static States CurrentState = States.First;
    public static void Reset() {
        CurrentState = States.First;
    }
}