using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FindCaseStatementWithoutDefault
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FindCaseStatementWithoutDefaultAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FindCaseStatementWithoutDefault";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatementWithEnum, SyntaxKind.SwitchStatement);
        }


        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext obj)
        {
            var switchStatement = obj.Node as SwitchStatementSyntax;

            bool hasDefault = switchStatement.DescendantNodes().OfType<DefaultSwitchLabelSyntax>().Count() > 0;

            if (!hasDefault)
            {
                var diagnostic = Diagnostic.Create(Rule, switchStatement.GetLocation(), "switch");

                obj.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeSwitchStatementWithEnum(SyntaxNodeAnalysisContext swtichStatementNode)
        {
            var switchStatement = swtichStatementNode.Node as SwitchStatementSyntax;

            var switchVariableIdentifier = switchStatement.ChildNodes().OfType<IdentifierNameSyntax>().First();

            var switchVariableType = swtichStatementNode.SemanticModel.GetTypeInfo(switchVariableIdentifier);

            var isEnum = switchVariableType.Type.TypeKind == TypeKind.Enum;

            bool hasDefault = switchStatement.DescendantNodes().OfType<DefaultSwitchLabelSyntax>().Count() > 0;

            if (!hasDefault && isEnum)
            {
                var diagnostic = Diagnostic.Create(Rule, switchStatement.GetLocation(), "switch");

                swtichStatementNode.ReportDiagnostic(diagnostic);
            }
        }
    }
}
