# Omni-Armory Development Standards

## 1. GUI Interface Standards
- **Framework**: WPF (Windows Presentation Foundation).
- **Theme**: Modern Dark Theme (Fluent Design for Win11, Classic fallback for Win7).
- **Interaction**: Drag-and-drop support; Right-click context menu integration.

## 2. Coding Principles for LLM/IDE
- **Performance First**: All filesystem operations MUST use non-blocking asynchronous methods. Avoid `Get-ChildItem` in C#; use `DirectoryEnumerator` or P/Invoke FindFirstFileEx for maximum speed.
- **Error Handling**: Use a "Silent Recovery" pattern. Log errors to a hidden view but do not interrupt the main scan.
- **Extensibility**: All tools must inherit from `IBaseTool` interface. New tools (e.g., duplicate finder, file renamer) should be added as new classes without modifying the core UI logic.

## 3. Documentation Strategy
- **README.md**: User-facing manual (Installation, Features).
- **ARCH.md**: Developer-facing architecture (Class diagrams, Data flow).
- **CHANGELOG.md**: Track OS compatibility fixes.

## 4. Multi-OS Compatibility Logic
- Always check `Environment.OSVersion` before calling OS-specific Shell APIs.
- Use conditional symbols `WINDOWS_7_COMPAT` for legacy feature support.