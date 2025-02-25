# Godot 斗地主游戏实现

初学 Godot 并自己尝试实现了可以单机和联机的斗地主，但功能非常简陋且不完整。玩家的AI只是简单设立了两条规则：

- 当自己第一个出牌时，默认出倒数第一张
- 当自己跟牌时，按照提示的第一个选项出牌

只是自己初学时摸索的项目之一，现在回看有诸多不足，bug很多，各组件代码也非常混乱，没有很好地解耦和细分。但毕竟是一份能跑的项目，而且基本上是自己独立完成的，于是决定还是公开在 Github 上。

## 参考项目

- [onestraw/doudizhu: :spades:斗地主引擎 (github.com)](https://github.com/onestraw/doudizhu)（使用该项目跑了 data.json 和 type_cards.json 文件，并参考了部分代码实现判断牌型合法性和提示的逻辑）
- [mailgyc/doudizhu: html5 斗地主游戏 (github.com)](https://github.com/mailgyc/doudizhu)（使用了该项目的素材）
