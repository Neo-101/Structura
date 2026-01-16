#### 1. 项目愿景 (Project Vision)
*   **定位**：轻量级、极速、无侵入式的 Windows 增强工具箱。
*   **核心理念**：
    *   **性能至上**：扫描 100,000 个文件，UI 不得卡顿，内存占用需控制在 50MB 以内。
    - **极简 GUI**：一键触达，拒绝繁琐的配置项。
    - **向下兼容**：一份代码通过条件编译或动态检测，完美适配 Windows 7/10/11。

#### 2. 技术栈架构选择 (Technical Stack)
*   **核心引擎 (Core)**: C# / .NET 8 (或 .NET Framework 4.8 兼容 Win7)。
    *   *理由*：C# 对 Windows 原生 API (Win32, Shell API, NTFS) 的调用效率最高，且 WPF 框架能提供极高性能的 GUI 渲染。
*   **异步模型**: 基于 `async/await` 和 `Dataflow` 的并行扫描，确保统计数据与 UI 刷新完全解耦。

#### 3. 基础功能模块规范 (Standard Modules)
*   **Module A: Directory Analyzer (即你目前的工具)**
    *   提供可视化树状图。
    *   提供双维度遥测（Direct vs Deep）。
    *   支持 JSON 导出与右键集成。
*   **Module B: Logic Dispatcher (逻辑分发器)**
    *   自动识别 OS 版本。针对 Win7 自动回退到传统的 API，针对 Win11 调用最新的 MFT 或性能计数器。