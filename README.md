# Code Metrics Viewer – Visual Studio Extension

This is a Visual Studio Extension (VSIX) that adds a tool window for analyzing code metrics in the current solution or open project. It provides configurable, on-demand analysis of C# files using Roslyn, helping developers quickly identify long methods, complex functions, and keyword-based comments.

## Features

- Displays total lines of code, number of classes, and number of methods from the selected file.
- Highlights long methods based on a configurable line count threshold.
- Calculates cyclomatic complexity and highlights methods exceeding a specified threshold.
- Scans for user-defined keywords (e.g., TODO, FIXME, HACK) in comments.
- Provides method statistics, including average method length.
- Allows users to customize thresholds and keywords via the UI.
- Integrated into Visual Studio as a tool window under "View > Other Windows".

## Installation & Setup

### Prerequisites

- Visual Studio with the Visual Studio extension development workload
- .NET Framework or .NET SDK as required by the project template

### Creating the Project

1. Create a new VSIX project:
   - File > New > Project > VSIX Project

2. Add a Tool Window to the project:
   - Right-click the project in Solution Explorer
   - Select Add > New Item
   - Search for "Tool Window" and add it

   This action generates the following structure:
    ToolWindow1/
    - ToolWindow1.cs
    - ToolWindow1Control.xaml
    - ToolWindow1Control.xaml.cs

3. Add the following NuGet package to enable code parsing:

   - This package is used to analyze C# syntax trees and compute metrics like function length and complexity.

### Running the Extension

1. Build the project in Debug mode.
2. Press F5 to launch the extension in a new Visual Studio experimental instance.
3. Open any C# project in that instance.
4. Go to `View > Other Windows > Code Metrics Viewer` to open the tool window.

### Adding the extension permanently to the Visual Studio
1. Build the project.
2. Navigate to Project folder -> Bin -> Debug
3. Install teh vsix file called 'CodeMetricsViewer'

## Usage

- Input threshold values for function length and cyclomatic complexity.
- Enter comma-separated keywords to scan for (e.g., TODO,FIXME,HACK).
- Click "Analyze" to perform analysis on the currently selected file.

The results will appear in the lower scrollable window, showing:

- Name of the file
- Total lines of code
- Class and method counts
- Average method length
- List of long methods exceeding the line threshold
- List of methods exceeding the complexity threshold
- Keyword hits grouped by file

## Project Structure

### ToolWindow1Control.xaml

Defines the UI layout, which includes:

- Three input fields: function length threshold, complexity threshold, and keyword list
- An "Analyze" button
- A results area inside a scrollable GroupBox

### ToolWindow1Control.xaml.cs

Contains event logic and UI interaction. When "Analyze" is clicked, it calls a helper class to perform the analysis and updates the results box.

Example:

```csharp
private void Analyze_Click(object sender, RoutedEventArgs e)
{
    string code = GetActiveDocumentText();
    if (string.IsNullOrWhiteSpace(code))
    {
        ResultBox.Text = "No code found.";
        return;
    }
    ...
}
```

Example output:

```
Analysis of: Form1.cs
Total Lines: 712
Classes: 3
Methods: 22
Avg Method Length: 28

Long Methods (> 20 lines):
- timer1_Tick (31 lines, CC: 2)
- button1_Click (30 lines, CC: 2)
- simulare_meci_campionnat (113 lines, CC: 23)

Cyclomatic Complexity (CC > 10):
- simulare_meci_campionnat: 23
- simulare_meci: 21
- button6_Click: 36

```

## Future Improvements

Possible enhancements include:

- Exporting results to a file

- Copy-to-clipboard functionality

- Sorting or filtering results

- Highlighting critical methods with color

- Using a TreeView or tabular layout for better structure