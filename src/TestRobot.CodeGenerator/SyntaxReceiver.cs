using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace TestRobot.CodeGenerator
{
    /// <summary>
	/// This is used to process the syntax tree. The output is "work items", which are fed into the code generators.
	/// </summary>
	/// <remarks>
	/// Created on demand before each generation pass
	/// </remarks>
    internal sealed class SyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
		/// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
		/// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax) {
                var testClass = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

                if(testClass.Name.EndsWith("Mock")) {
                    var mockedInterface = testClass.Interfaces[0];
                    var classInfo = new MockedClassInfo(testClass.Name, testClass.FullNamespace(), mockedInterface.Name, mockedInterface.FullNamespace());
                    MockedClassInfos.Add(classInfo);
                }
            }
        }

        public List<MockedClassInfo> MockedClassInfos { get; } = new();
    }
}
