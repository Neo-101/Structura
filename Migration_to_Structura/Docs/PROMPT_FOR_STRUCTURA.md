# Prompt for Structura Initialization

**Role**: You are an expert .NET/WPF Architect starting a new project called **Structura**.

**Context**: 
We are migrating the core "Tree View" functionality from a legacy project (OmniArmory) to this new, focused tool. The legacy project had a robust scanning engine but suffered from feature bloat. Structura will focus purely on **file system visualization and analysis**.

**Assets Provided**:
I have a migration bundle located at `[Path to Migration_to_Structura/Source]`. It contains:
1.  `TreeScanner.cs`: A proven async scanning engine.
2.  `DirectoryNode.cs` & `FileEntry.cs`: The data models.
3.  `TreeView_Snippet.xaml`: The optimized UI definition.

**Your Task**:
Initialize the **Structura** project with the following specifications:

1.  **Framework**: .NET Framework 4.8 (WPF), using SDK-Style CSPROJ.
2.  **Structure**: 
    - `Structura.Core`: Class Library for logic (Scanner, Models).
    - `Structura.UI`: WPF Application.
3.  **Action**:
    - Import the provided source files, renaming namespaces from `OmniArmory` to `Structura`.
    - Set up the `MainWindow` using the provided XAML snippet.
    - Ensure `System.Text.Json` is installed.
    - Configure `launch.json` for debugging (remember the `.NET 4.8` "clr" type configuration).

**Goal**: 
Create a running application that can select a folder, scan it using `TreeScanner`, and display the results in the virtualized `TreeView`. No "Duplicate Finder" or other extraneous features.
