# godot4-action-rpg-demo
Godot 4 version game-demo duplicating "Action RPG" in uheartbeast's youtube-tutorials.

使用 Godot 4 版本复刻 uheartbeast YouTube 教程中的 “Action RPG”，链接：[Make an Action RPG in Godot 3.2 - YouTube](https://www.youtube.com/watch?v=mAbG8Oi-SvQ&list=PL9FzW-m48fn2SlrW0KoLT4n5egNdX-W9a)

B站转载链接：[【中字】Godot 3.2像素风ARPG制作教程（全集）\_哔哩哔哩\_bilibili](https://www.bilibili.com/video/BV15D4y1U7j5/)

原项目的 Github 链接：[youtube-tutorials/Action RPG at master · uheartbeast/youtube-tutorials](https://github.com/uheartbeast/youtube-tutorials/tree/master/Action%20RPG)

由于原项目的 Godot 版本较旧（3.2），无法直接移植到 Godot 4 中，需要稍作修改。这里将自己学习该教程，并用 Godot 4 复刻的游戏项目上传，不完全符合教程，也没有做什么优化，只是个人备份用，仅供参考。

除了一些新旧语法的变动（如场景初始化的函数由 `instance()` 改成了 `instantiate()`）外，项目修改的部分包括但不限于：

- 使用 CanvasGroup 代替旧版本的 Y-Sort 将同类物体整理到一块
- 将 ATTACK 和 ROLL 动画播完更改 state 的部分，修改为使用 Timer 倒计时实现，Timer 的时长即动画播放的时长
- HurtBox 设置无敌时间开始和结束的函数，直接调用 `set_deferred("monitoring", true)` 和 `set_deferred("monitoring", false)`
- 使用 CliffTileMap 的最远左上点和最远右下点来限制 Camera2D 镜头的范围
