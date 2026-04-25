# Addressables 简单案例

这个目录放的是给实习生看的最小案例，目标不是做完整框架，而是把 `加载`、`释放`、`场景切换`、`标签预热` 这四个基础动作讲清楚。

## 文件说明

- `AddressablesPrefabSpawner.cs`
  - 用 `AssetReferenceGameObject` 加载并实例化预制体
  - 适合演示 `InstantiateAsync` 和 `ReleaseInstance`

- `AddressablesSpriteLoader.cs`
  - 用 `AssetReferenceSprite` 加载 Sprite
  - 可以把结果赋给 `UI.Image` 或 `SpriteRenderer`

- `AddressablesSceneLoader.cs`
  - 用 `AssetReference` 加载 Addressable Scene
  - 适合演示 `LoadSceneAsync` / `UnloadSceneAsync`

- `AddressablesLabelPreloadExample.cs`
  - 用标签批量预加载资源
  - 适合讲清楚“按 label 预热一组资源”的思路

## 建议教学顺序

1. 先讲 `AssetReference`
   - 它比手写字符串地址更稳
   - Inspector 可直接拖拽 Addressable 资源

2. 再讲“加载以后一定要释放”
   - 预制体实例用 `Addressables.ReleaseInstance`
   - 资源句柄用 `Addressables.Release`

3. 最后讲场景和标签
   - 场景本质上也是 Addressables 资源
   - Label 适合做关卡前预加载、商店页预热、角色皮肤预热

## 快速使用

1. 把需要测试的资源勾成 Addressable
2. 给资源设置 Address 或 Label
3. 把脚本挂到场景中的空物体上
4. 在 Inspector 中把 Addressable 资源拖到对应字段
5. 运行前执行一次 `Window > Asset Management > Addressables > Groups` 里的构建流程

## 编辑器快捷菜单

- `Tools > Addressables > Setup Lesson Examples`
  - 自动把教学用 prefab、sprite、scene 注册进 Addressables
  - 自动把 `SampleScene` 里的 3 个引用字段回填好

- `Tools > Addressables > Build Lesson Content`
  - 触发一次 Addressables Content Build
  - 适合在演示前手动跑一次

## 可直接拿来做演示的资源建议

- 预制体案例
  - 随便找一个角色或敌人做成 Prefab，再拖到 `prefabReference`

- 图片案例
  - 可以用 `Assets/Art/Ui/Title.png`
  - 或任意已经切成 Sprite 的 UI 图片

- 场景案例
  - 新建一个教学场景，单独标成 Addressable Scene
  - 不建议直接拿主场景做切换演示

- Label 案例
  - 给 2 到 5 个敌人 Prefab 打上 `preload`
  - 运行时观察控制台输出加载数量

## 实习生最容易踩坑的点

- 资源没有勾成 Addressable，却想用 Addressables 加载
- 地址或引用填了，但没有构建 Addressables 内容
- 只加载不释放，导致内存和引用一直堆着
- 场景切换用 `Single` 模式时，把当前教学场景直接顶掉了
