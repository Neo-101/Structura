# Migration Guide: OmniArmory -> Structura

## 1. Project Overview
**Structura** is the spiritual successor to OmniArmory, focusing purely on high-performance directory tree analysis and visualization. This migration bundle contains the battle-tested scanning engine and data models from OmniArmory V1.1-Beta.

## 2. Included Assets (Source Folder)
These files are copied from the stable `OmniArmory.Core` and `OmniArmory.Shared` libraries:

### Core Logic
- `TreeScanner.cs`: The asynchronous scanning engine. Uses `DirectoryInfo.EnumerateFileSystemInfos` with a post-order traversal for bottom-up statistics aggregation.
- `TreeModifier.cs`: Helper for modifying the tree structure (e.g., deleting files) and bubbling up stats updates.
- `ScanConfig.cs`: Configuration object (MaxDepth, IncludeHidden, etc.).

### Data Models
- `DirectoryNode.cs` / `IDirectoryNode.cs`: The main recursive tree structure.
- `FileEntry.cs`: Represents leaf nodes (files).
- `FileStatModel.cs`: Holds the statistical data (FileCount, Size) for each node.

### UI Assets
- `TreeView_Snippet.xaml`: The XAML code for the `HierarchicalDataTemplate` and `TreeView` style. **Crucial**: Keeps the virtualization settings (`VirtualizingStackPanel.IsVirtualizing="True"`).

## 3. Integration Steps for Structura
1.  **Create New Project**:
    - Target: .NET Framework 4.8 (WPF App).
    - Recommendation: Use SDK-Style `.csproj` for easier management.

2.  **Import Code**:
    - Copy the `Source` folder contents into your new solution.
    - Namespace Recommendation: Rename `OmniArmory.*` to `Structura.*` (Search & Replace).

3.  **Dependencies**:
    - Install `System.Text.Json` (v9.0.1 or later) via NuGet.

4.  **UI Setup**:
    - Paste the contents of `TreeView_Snippet.xaml` into your `MainWindow.xaml` (or separate resource dictionary).
    - Ensure your MainViewModel binds a collection of `DirectoryNode` to the TreeView's `ItemsSource`.

## 4. Key Lessons from OmniArmory (Do Not Repeat)
- **Virtualization is King**: Never disable UI virtualization on the TreeView. Without it, 10,000 nodes will freeze the app.
- **Async Only**: Scanning must run on a background thread (`Task.Run`).
- **Access Denied**: The `TreeScanner` has built-in `try-catch` blocks for `UnauthorizedAccessException`. Do not remove them; otherwise, the scan will abort on system folders.
