<!--
## 代码项目研究

经过Godot解包工具，在GodotSteam 3.4.4版本中打开游戏解包文件，主要脚本：
- ButtonsMenu：管理所有UI按钮
	- 基本都继承自TT Button
- TT Button：定义了通用按钮的具体实现

吐槽：游戏源码耦合性太高，不易阅读维护，大部分逻辑都在主函数和主要功能脚本中，用大量if语句判断，MOD Wiki也问题重重，不足以支撑较为复杂的MOD制作

-->
游戏Wiki：[Luck be a Landlord Wiki | Fandom](https://luck-be-a-landlord.fandom.com/wiki/Luck_be_a_Landlord_Wiki)

## MOD制作

MOD Wiki：[Home · TrampolineTales/LBAL-Modding-Docs Wiki (github.com)](https://github.com/TrampolineTales/LBAL-Modding-Docs/wiki)

### Wiki研究

#### 1. LBAL-Sandbox-Data.save位置：

> Windows: `%USERPROFILE%/AppData/Roaming/Godot/app_userdata/Luck be a Landlord/LBAL-Sandbox-Data.save`
> 
> macOS: `~/Library/Application Support/Godot/app_userdata/Luck be a Landlord/LBAL-Sandbox-Data.save`
> 
> Linux: `~/.local/share/godot/app_userdata/Luck be a Landlord/LBAL-Sandbox-Data.save`

[Tutorial 1.3: Testing in the Sandbox · TrampolineTales/LBAL-Modding-Docs Wiki (github.com)](https://github.com/TrampolineTales/LBAL-Modding-Docs/wiki/Tutorial-1.3:-Testing-in-the-Sandbox)

#### 2. 相邻动画效果

> The Coins are animating instead of Cool Symbol! But actually, this makes sense based on this effect:
> 
> `{"effect_type": "adjacent_symbols", "comparisons": [{"a": "type", "b": "coin"}], "value_to_change": "destroyed", "diff": true, "anim": "bounce"}`
> 
> Since we have the `effect_type` of `"adjacent_symbols"`, the `anim` is being applied to to the Coin, just like how the `value_to_change` is.
> 
> A simple workaround in this situation is to use `anim_targets`:
> 
> `{"effect_type": "adjacent_symbols", "comparisons": [{"a": "type", "b": "coin"}], "value_to_change": "destroyed", "diff": true, "anim": "bounce", "anim_targets": "adjacent_symbol"}`
> 
> Having the variable be `"adjacent_symbol"` will make the Coin and Cool Symbol bounce in tandem, just what we wanted!

[Tutorial 1.5: Effects (Advanced Usage) · TrampolineTales/LBAL-Modding-Docs Wiki](https://github.com/TrampolineTales/LBAL-Modding-Docs/wiki/Tutorial-1.5:-Effects-(Advanced-Usage))

### 开发工具MOD

直接通过MOD接口很难做成类似杀戮尖塔的调试工具，所以需要用MOD支持的功能“曲线救国”，达到类似的效果。

初步思路：
1. 创造一个物品，数量999，消除后添加所有符号；
2. 创造一个物品，数量999，消除后添加所有物品；
3. 创造一个物品，数量999，消除后增加1000消除符号；
4. 创造一个物品，数量999，消除后增加1000重置符号（原则上不需要）
5. 创造一个物品，数量999，消除后获得10000金币（兜底）

问题：因为与原版关卡不同，所以需要额外搞一个关卡层级，但这似乎又不符合玩家利用该MOD刷成就的目的（新开关卡和原版关卡不同，费力又达不到目的）

改进思路：
1. 新物品：超级信用卡 Super Credit 每回合在所有元素中任选 普通
2. 新物品：超级物品包 Super Bag 每回合在所有物品中添加 普通
3. 新元素：金条 Gold Bar 价值1,000 普通
4. 新元素：亿万支票 Cheque 价值100,000,000 普通
5. 新物品：重置炸弹 Reset Bomb 消除后，提供1000重置货币 普通
6. 新物品：消除炸弹 Clear Bomb 消除后，提供1000消除货币 普通

精简：改金币直接用CE就行了，没必要专门设计相应的物品和元素

结果如下：
1. 新物品：工具包 Toolbox 消除后，提供100重置货币和100消除货币
2. 新物品：字典 Dictionary 每回合额外一次在所有元素中任选，在所有物品中任选
3. 新元素：作弊扭蛋 Cheat Egg 添加99个工具包和字典

考虑到必须用元素添加物品（否则物品摇上来的机会太少），而且每回合添加的机制不如消除添加的方式更合理，所以物品应该都改成消除添加的模式

修改如下：
1. 新元素：作弊扭蛋 Cheat Capsule 添加99个下述物品
2. 新物品：重置盒子 Reroll Box 消除后，添加100个重置货币
3. 新物品：消除盒子 Removal Box 消除后，添加100个消除货币
4. 新物品：符号盒子 Symbol Box 消除后，从所有符号中选择一个
5. 新物品：物品盒子 Item Box 消除后，从所有物品中选择一个

优化：数量改成99没必要，也不好计算，改成50更合适（大于20即可）

---

经过对源码的分析，发现item的destoryed为true后，系统并不会处理emails_to_add的逻辑，只有items_to_add和symbols_to_add的逻辑是可以正常处理的（要么就效仿作者的另一个MOD：Single Spin Puzzles，重新创造20个关卡，然后改变邮件样式；要么就采取折中的方式，使下一次spin的数量增加；再或者，可以一次性添加所有符号/物品，供玩家自行删除/禁用，但这样体验不好，故不考虑此方法）

更正：extra是旋转后邮件的数量，不是邮件提供的符号/物品数量！

现有解决方案/问题汇总：
1. 通过Destroy物品获得添加符号/物品（不可行，Item的Destroy逻辑太少）
2. 从Floor开始重构整个关卡，效仿作者MOD编写（太复杂/成就可行性未知）
3. 使用绝对可行的items_to_add（mod脚本编写麻烦/玩家禁用麻烦）
4. 不destroy直接添加emails，数量超过3个会出错（通过Item添加的email有数量限制，超过3个就会出错；但官方MOD可行，尚不明确具体原因与解决办法）
5. 或许可以通过MOD脚本编写_ready函数等实现直接操控（注入）核心代码，构建命令行界面方便调试（但实现难度更大，且命令行操作并不方便，故不考虑）
6. 覆盖符号炸弹和方便面精华（添加符号/物品；可行性未知，编写复杂）

MOD读取时也做了一定的保护措施，导致通过MOD脚本直接注入源程序新增Node也不行（或许可以设法突破，但大概率不能走创意工坊的MOD上传形式，不考虑了）

经测试，只要有多于3个的元素选择，哪怕mod脚本限制为0个，通过常规方式（如信用卡）得到的添加机制，结果一样会报错（即非常不安全的机制，而且不是破坏触发的方式）；多设计一些盒子分担符号固然可行，但工作量也非常大，难以实现

不借助创意工坊的方式，可以做一个编辑本地save文件的第三方修改工具，加入可视化符号选择的功能替代直接编辑文本，参考：[in-depth game file editing guide : r/LuckBeALandlord](https://www.reddit.com/r/LuckBeALandlord/comments/mpbvtz/indepth_game_file_editing_guide/)（当然肯定不考虑这样搞，意义不大，也与最初想法相悖）

---

勘误：错怪开发者了，是测试mod时一直使用解包后的程序测试，而解包程序可能有未知问题（解包纰漏或未知修改）导致多符号添加时会出错，实际上使用Steam本体就可以正常调试……

发现原因：作者的MOD移动到本地mods文件夹下也出现相同问题，同理可得本地的问题如果使用Steam版本也不会出错（归根究底还是在作者的MOD基础上照猫画虎）

总结一下坑点和特性：
1. 脚本编写不能有注释，art文件必须带有STEAM_ID号
2. 多个物品消除1个时，获得的数值型增益所有的物品都累计（譬如10个物品，消除可以获得100金币，那么消除1个就是1000金币，第2个消除给900，以此类推）
3. 胶囊effect只对diff数值有效，添加物品/符号的逻辑需要自己复制才能实现
4. Item的destroy无法触发emails_to_add，必须是旋转触发，故新编写一个email用于添加符号（原本symbols_to_choose_from是145-3=142个，考虑到增加了10个作弊扭蛋，所以改成152个；直接emails_to_add添加add_tile类型可行性未知）
5. 物品添加会出现每转不停增加的bug，且无法像符号一样全部稀有度都可以添加，故采用了破坏物品添加所有物品/精华，由玩家手动禁用的机制
6. mod uploader上传mod要求mod文件与exe同磁盘，而且实际测试仍然不行（最终使用steamcmd辅助上传文件）
7. 考虑到tags使用steamcmd无法添加（暂时没有找到方法），故使用mod uploader先占位并添加tags，再通过steamcmd根据已经确定的mod号更新内容
8. 创意工坊mod测试时，必须删除本地mods文件夹的mod，才能正常，原因不详

### PVZ MOD（仅构思）

用PVZ的各种元素填充，例如：
0. 每转根据难度不同添加不同僵尸（可以摧毁植物/占位置的无收益符号）
1. 向日葵：每转提供1-2枚硬币
2. 豌豆射手：每转添加1棵豌豆，可以攻击横行的僵尸，豌豆消耗，获得一定硬币
3. 火爆辣椒：清除一行（或十字）的僵尸，消耗
4. 爆炸樱桃：清除周围8格内的僵尸，消耗
5. 毁灭菇：清除场地上所有的僵尸，消耗，生成一个坑洞，20转后消失

然而，这些设想很多远远超过了MOD所能实现的范畴，必须修改源码才能完成，这就背离了制作该MOD的初衷，且源码混乱重复，修改工作量太大，收益太小

幸运房东开创了一套相对成熟的DBG机制，将老虎机的元素与“卡牌-遗物”的设计相结合；但深度稍欠缺，随机性太强，加之MOD制作的难度较大，社区也极其不活跃，很多创意工坊的物品也都是蜻蜓点水，没有那种可以赋予游戏第二春的MOD出现

既视感：和流氓软件一样同为Godot引擎，都是通过代码的形式添加MOD，游戏本身都具有极大的拓展潜力，但实际开发都不是太容易，社区都不算太活跃（可惜了）