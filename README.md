# Unity 工程师笔试 - 背包系统

## 环境

- Unity：2022.3.62f2c1
- 构建平台：Windows 64 位
- 脚本后端：Mono
- UI：uGUI + TextMeshPro
- 资源加载：Addressables
- 本次交付已关闭 HybridCLR 脚本热更新，保证笔试包可以直接构建和运行。

## 运行方式

1. 解压 `Windows运行包.zip`，不要只复制其中的 exe。
2. 运行 `Learn.exe`。
3. 启动流程从 `BootScene` 进入，通过 Addressables 加载 `MainMenuScene`。
4. 背包界面默认显示在主菜单中，可切换分类、翻页并点击物品查看详情。
5. 程序默认以 1280 x 720 窗口运行；点击右上角“退出游戏 Esc”或按 `Esc` 可退出。

## 交付文件

- `Windows运行包.zip`：可直接解压运行的完整 Windows 版本。
- `源码审查包.zip`：保留脚本、场景、背包素材、配置、测试、Packages 与 ProjectSettings；已移除与本题无关的大体积音视频和演示美术。
- `算法题`：两道算法题的 C# 实现与说明。

## 已实现功能

- 物品图片和名称展示
- 全部、装备、消耗品、材料、碎片、其他分类筛选
- 点击物品显示唯一实例 ID、配置 ID、名称和描述
- 背包最大容量 80，非堆叠模式
- 每页最多 28 格，7 列 x 4 行
- 格子对象池复用，最多创建当前页所需格子
- 每帧绑定 7 个物品，一页最多分 4 帧完成
- 分类结果缓存，背包变化时统一失效
- Addressables 异步加载配置表和图标
- 异步图标绑定版本校验，避免对象池复用时显示旧图片
- 领域模型为纯 C#，不依赖 MonoBehaviour
- Windows 窗口模式、主菜单退出按钮和 Esc 快捷退出
- 19 项 EditMode 测试通过

## 代码入口

- 表现层：`Assets/Scripts/GamePlay/Inventory/UI`
- 应用入口：`Assets/Scripts/GamePlay/Inventory/InventorySystem.cs`
- 服务接口：`Assets/Scripts/GamePlay/Inventory/IInventoryService.cs`
- 领域模型：`Assets/Scripts/GamePlay/Inventory/Runtime`
- 配置模型：`Assets/Scripts/GamePlay/Inventory/Config`
- 配置数据：`Assets/ConfigTable/Inventory/ItemConfig.json`
- 测试：`Assets/Tests/Editor/InventoryTests.cs`
- 算法题：`Interview/AlgorithmSolutions.cs`

## 架构说明

当前实现采用“表现层 + 应用入口/服务接口 + 领域模型 + 基础设施”的轻量分层结构，并非为了套用严格 MVC。UI 只依赖 `IInventoryService`，实际业务规则由纯 C# 的 `Inventory` 管理。

架构图源文件：`inventory-current-architecture.drawio`

## 美术资源

背包图标来自 Kenney 的 CC0 素材，许可说明位于：

`Assets/Art/Inventory/LICENSE_KENNEY_CC0.txt`

## 算法复杂度

- 括号匹配：时间 O(n)，空间 O(n)
- 合并两个有序数组：时间 O(n + m)，空间 O(n + m)
