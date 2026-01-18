# Structura

[![Download Latest EXE](https://img.shields.io/github/v/release/Neo-101/Structura?label=Download%20Latest%20EXE)](https://github.com/Neo-101/Structura/releases/latest)

[English](#structura) | [中文说明](#structura-中文说明)

Structura is a high-performance directory structure analysis tool designed specifically for **Obsidian** users. It rapidly scans your vault, generates a clear tree view, and exports a JSON format optimized for LLMs (like ChatGPT, DeepSeek).

Whether your knowledge base is chaotic or you just want to reorganize your notes, Structura helps you extract the "skeleton" of your vault so AI can help you plan a better structure.

## 🎯 Use Cases

*   **Obsidian Vault Refactoring**: When your notes become messy, Structura provides the most accurate "map" for AI to help you re-categorize and restructure.

## 🚀 Usage

1.  **Download**: Click the **"Download Latest EXE"** badge above to get the latest `Structura.exe`.
2.  **Run**: Double-click to launch (Single-file portable app, no installation required).
3.  **Scan**:
    *   Click the `...` button next to the path bar and select your Obsidian vault folder.
    *   Click **Scan**.
4.  **Export for AI**:
    *   Click **Export & Copy JSON**.
    *   The tool will generate the structure JSON and **automatically copy it to your clipboard**.
    *   Paste it into ChatGPT, Claude, or DeepSeek along with your refactoring request.

## 🔮 Roadmap

We are committed to making Structura the best AI-native file management assistant.

*   **Obsidian Deep Integration**
    *   Smart recognition of `.obsidian` config and plugin structures.
    *   Analyze attachment associations, calculate "backlink density" and "tag distribution", merging this metadata into the structure tree.
*   **🛡️ Privacy Mode**
    *   One-click "desensitization".
    *   Automatically replace sensitive folder/file names with placeholders like `Folder_A` before exporting to LLMs.
*   **Future Expansions**
    *   **Code Project Analysis**: Analyze project structures, file distributions, and nesting depths.
    *   **General File Organization**: Statistics on file counts and identifying "bloated" or "empty" directories.

## 💻 Compatibility

*   **OS**: Windows 10 / Windows 11 (Supports x64, x86, ARM64)
*   **Requirement**: .NET Framework 4.8 (Pre-installed on most modern Windows systems)
*   **Languages**: Auto-detects system language for prompts (English, Chinese, Japanese, Spanish, French).

## 🛠️ Tech Stack
*   .NET Framework 4.8
*   WPF (Windows Presentation Foundation)
*   Fody & Costura (Single-file bundling)

---

# Structura (中文说明)

**Structura** 是一个专为 **Obsidian** 用户设计的高性能目录结构分析工具。它能飞速扫描你的知识库，生成清晰的树状结构视图，并导出 LLM（如 ChatGPT、DeepSeek）可读的 JSON 格式。

无论你是想重构混乱的 Obsidian 知识库，还是清理陈旧的笔记，Structura 都能帮你把“目录结构”提取出来，让 AI 帮你出谋划策。

## 🎯 适用场景 (Use Cases)

*   **Obsidian 知识库重构**：当你的笔记库变得杂乱无章，想让 AI 帮忙重新规划分类时，Structura 能提供最精准的“地图”。

## 🚀 使用方法 (Usage)

1.  **下载**: 点击页面顶部的 **"Download Latest EXE"** 徽章，下载最新版本的 `Structura.exe`。
2.  **运行**: 双击运行程序（单文件绿色版，无需安装）。
3.  **扫描**:
    *   点击路径栏旁边的 `...` 按钮，选择你的 Obsidian 库根目录。
    *   点击 **Scan** 按钮开始分析。
4.  **导出给 AI**:
    *   点击 **Export & Copy JSON** 按钮。
    *   程序会自动生成目录结构的 JSON 数据，并**自动复制到你的剪贴板**。
    *   打开 ChatGPT、Claude 或 DeepSeek，直接 **粘贴**，并附上你的重构需求。

## 🔮 未来计划 (Roadmap)

我们致力于让 Structura 成为 AI 时代的最佳文件管理助手。

*   **Obsidian 深度集成**
    *   智能识别 `.obsidian` 配置文件夹及插件结构。
    *   分析附件关联情况，统计“双向链接密度”和“标签分布”，并将这些元数据合并到结构树中，让 AI 更懂你的知识网络。
*   **🛡️ 隐私保护模式 (Privacy Mode)**
    *   支持一键“脱敏”功能。
    *   在导出给 LLM 分析前，自动将敏感的文件夹或文件名替换为 `Folder_A`, `Folder_B` 等代号，确保你的隐私数据安全不外泄。
*   **更多场景扩展**
    *   **代码项目分析**：快速了解一个陌生项目的目录结构、文件分布和层级深度。
    *   **文件整理辅助**：直观地统计文件夹下的文件数量，快速找出那些“臃肿”堆积或“空置”的角落。

## 💻 系统兼容性 (Compatibility)

*   **操作系统**: Windows 10 / Windows 11 (支持 x64, x86, ARM64)
*   **运行环境**: .NET Framework 4.8 (现代 Windows 系统通常已预装)
*   **多语言支持**: 自动根据系统语言切换提示词 (支持 英/中/日/西/法)。
