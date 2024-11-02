using Godot;

namespace DouCardPuzzoom.scripts.manager; 

public static class MouseManager {
    public static bool Dragging = false;
    
    // 原则上，还应该针对箭头和手势的不同指向改变实际判定的点，懒得改了
    public static Resource Arrow = ResourceLoader.Load("res://assets/mouse/mouse-arrow.png");
    public static Resource Click = ResourceLoader.Load("res://assets/mouse/mouse-click.png");

    public static bool IsInterAreaAble = true;
}