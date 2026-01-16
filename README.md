# OmniArmory

**A high-performance Windows toolset designed for speed, efficiency, and modern usability.**

## Vision
OmniArmory is built to be a lightweight, non-intrusive, and extremely fast utility suite for Windows power users. It prioritizes performance (non-blocking I/O, low memory footprint) and compatibility across Windows 7, 10, and 11.

## Features
- **High-Performance Scanning**: Utilize advanced non-blocking asynchronous I/O to scan thousands of files in seconds without freezing the UI.
- **Deep-Limited Directory Scanning**: Analyze directory structures with configurable depth limits.
- **Dual-Dimension Telemetry**:
  - **Direct Count**: Items strictly within the current folder.
  - **Deep Count**: Recursive total of all items in the branch (calculated via efficient post-order traversal).
- **Modern WPF UI**: A sleek, dark-themed interface supporting Drag & Drop interactions and virtualized tree views for handling large datasets.
- **Broad Compatibility**: Targeted for .NET Framework 4.8 to ensure native support on Windows 7 while leveraging Fluent Design aesthetics on Windows 11.

## Current Status
- ✅ **Core Engine**: High-performance `TreeScanner` implemented with `DirectoryInfo.EnumerateFileSystemInfos` and smart Reparse Point handling.
- ✅ **GUI**: Fully functional WPF interface with virtualization and async progress reporting.
- 🚧 **JSON Export**: Functionality in progress (data models ready).

## Getting Started
1. Open `OmniArmory.sln` in Visual Studio.
2. Build the solution (Target: `.NET Framework 4.8`).
3. Run `OmniArmory.UI`.
4. Drag and drop any folder onto the window to start analyzing.
