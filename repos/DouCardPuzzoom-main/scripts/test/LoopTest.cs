using System.Collections.Generic;
using Godot;

namespace DouCardPuzzoom.scripts.test; 

public static class LoopTest {
    public static string LevelName = "L_1_2";
    public static int Beg = 12000;
    public static int End = 12050;
    public static int Now = 12020;

    public static List<int> NoResList = new List<int>(); 
    
    public static void Start() {
        GameTest.CurrentLevelPath = $"res://levels/{LevelName}/{Now}.json";
        GameTest.InitLevel();
    }

    public static void GoNextLevel(bool noRes = false) {
        if (noRes) {
            NoResList.Add(Now);
        }

        Now += 1;
        if (Now > End) {
            foreach (var res in NoResList) {
                GD.Print(res);
            }

            return;
        }

        GameTest.CurrentLevelPath = $"res://levels/{LevelName}/{Now}.json";
        GameTest.InitLevel();
    }
}