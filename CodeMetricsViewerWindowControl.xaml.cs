using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CodeMetricsViewer
{
    public partial class ToolWindow1Control : UserControl
    {
        private readonly DTE2 _dte;

        public ToolWindow1Control()
        {
            InitializeComponent();
            _dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            string code = GetActiveDocumentText();
            if (string.IsNullOrWhiteSpace(code))
            {
                ResultBox.Text = "No code found.";
                return;
            }

            string fileName = _dte?.ActiveDocument?.Name ?? "Unknown file";

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            int totalLines = code.Split('\n').Length;
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            int classCount = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count();
            int totalMethodLength = methods.Sum(m => m.Body?.ToFullString().Split('\n').Length ?? 0);
            int avgMethodLength = methods.Count > 0 ? totalMethodLength / methods.Count : 0;

            int lengthThreshold = int.TryParse(ThresholdBox.Text, out int lt) ? lt : 20;
            int complexityThreshold = int.TryParse(ComplexityBox.Text, out int ct) ? ct : 10;
            var keywords = KeywordsBox.Text.Split(',').Select(k => k.Trim()).ToArray();

            var sb = new StringBuilder();
            sb.AppendLine($"Analysis of: {fileName}\n");
            sb.AppendLine($"📄 Total Lines: {totalLines}");
            sb.AppendLine($"📦 Classes: {classCount}");
            sb.AppendLine($"🔧 Methods: {methods.Count}");
            sb.AppendLine($"📊 Avg Method Length: {avgMethodLength}");

            sb.AppendLine($"\n⚠️ Long Methods (> {lengthThreshold} lines):");
            foreach (var method in methods)
            {
                int length = method.Body?.ToFullString().Split('\n').Length ?? 0;
                if (length > lengthThreshold)
                {
                    int cc = CalculateCyclomaticComplexity(method);
                    sb.AppendLine($"- {method.Identifier.Text} ({length} lines, CC: {cc})");
                }
            }

            sb.AppendLine($"\n🧠 Cyclomatic Complexity (CC > {complexityThreshold}):");
            foreach (var method in methods)
            {
                int cc = CalculateCyclomaticComplexity(method);
                if (cc > complexityThreshold)
                {
                    sb.AppendLine($"- {method.Identifier.Text}: {cc}");
                }
            }

            sb.AppendLine("\n📝 Keyword Matches:");
            var lines = code.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (keywords.Any(k => lines[i].Contains(k)))
                {
                    sb.AppendLine($"Line {i + 1}: {lines[i].Trim()}");
                }
            }

            ResultBox.Text = sb.ToString();
        }

        private int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
        {
            if (method.Body == null) return 1;

            var nodes = method.Body.DescendantNodes();
            int complexity = 1 +
                nodes.OfType<IfStatementSyntax>().Count() +
                nodes.OfType<ForStatementSyntax>().Count() +
                nodes.OfType<WhileStatementSyntax>().Count() +
                nodes.OfType<DoStatementSyntax>().Count() +
                nodes.OfType<CaseSwitchLabelSyntax>().Count() +
                nodes.OfType<ConditionalExpressionSyntax>().Count() +
                nodes.OfType<BinaryExpressionSyntax>().Count(be =>
                    be.IsKind(SyntaxKind.LogicalAndExpression) || be.IsKind(SyntaxKind.LogicalOrExpression)) +
                nodes.OfType<CatchClauseSyntax>().Count();

            return complexity;
        }

        private string GetActiveDocumentText()
        {
            try
            {
                var doc = _dte.ActiveDocument;
                if (doc?.Object("TextDocument") is TextDocument textDoc)
                {
                    EditPoint start = textDoc.StartPoint.CreateEditPoint();
                    return start.GetText(textDoc.EndPoint);
                }
            }
            catch { }
            return "";
        }
    }
}
