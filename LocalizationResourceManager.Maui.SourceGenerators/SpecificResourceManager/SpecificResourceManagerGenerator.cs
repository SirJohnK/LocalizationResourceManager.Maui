using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LocalizationResourceManager.Maui.SourceGenerators;

/// <summary>
/// Source Generator to generate the ISpecificResourceManager implementation for classes with the SpecificResourceManagerAttribute.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class SpecificResourceManagerGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initialize the Source Generator.
    /// </summary>
    /// <param name="context">Source generator context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif

        var classes = context.SyntaxProvider.CreateSyntaxProvider(PredicateStage, TransformStage)
            .Where(typeInfo => typeInfo is not null)
            .Collect();

        context.RegisterSourceOutput(classes, AddSpecificResourceManager);
    }

    /// <summary>
    /// Add the SpecificResourceManager implementation to the source.
    /// </summary>
    /// <param name="context">Source Production Context.</param>
    /// <param name="array">Array of type information.</param>
    private void AddSpecificResourceManager(SourceProductionContext context, ImmutableArray<ITypeSymbol?> array)
    {
        //Found any Specific Resource Manager Attributes?
        if (!array.IsDefaultOrEmpty)
        {
            foreach (var typeInfo in array)
            {
                //Get type information
                if (typeInfo is ITypeSymbol typeSymbol)
                {
                    //Check if ISpecificResourceManager interface is already implemented!
                    if (typeSymbol.AllInterfaces.Any(@interface => @interface.Name == "ISpecificResourceManager"))
                        continue;

                    //Get the SpecificResourceManagerAttribute
                    var attribute = typeSymbol.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass?.Name.Contains("SpecificResourceManager") ?? false);
                    if (attribute is not null)
                    {
                        //Get the ResourceManager Constructor Argument
                        var resourceManager = attribute.ConstructorArguments.FirstOrDefault().Value as string;
                        if (!string.IsNullOrWhiteSpace(resourceManager))
                        {
                            //Init
                            StringBuilder builder = new();
                            var namespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace ? "" : typeInfo.ContainingNamespace.ToString();

                            //Build Output
                            builder.AppendLine("using LocalizationResourceManager.Maui;");
                            builder.AppendLine($"namespace {namespaceName} {{");
                            builder.AppendLine($"public partial class {typeInfo.Name} : ISpecificResourceManager {{");
                            builder.AppendLine($"public string ResourceManager => \"{resourceManager}\";");
                            builder.AppendLine("}}");

                            //Add Source
                            context.AddSource($"{namespaceName}.{typeInfo.Name}.g.cs", builder.ToString());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Predicate Stage to determine if the class has the SpecificResourceManagerAttribute and should be transformed.
    /// </summary>
    /// <param name="node">Syntax Node to evaluate.</param>
    /// <param name="token">Cancellation Token.</param>
    /// <returns></returns>
    private bool PredicateStage(SyntaxNode node, CancellationToken token)
    {
        //Should node be transformed result
        var result = false;

        //Verify if node is a class
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            //Verify if class has the SpecificResourceManagerAttribute
            result = classDeclaration.AttributeLists.SelectMany(attributes => attributes.Attributes).Any(attribute => attribute.Name.NormalizeWhitespace().ToFullString().Contains("SpecificResourceManager"));
        }

        //Return result
        return result;
    }

    /// <summary>
    /// Transform Stage to get the class type information.
    /// </summary>
    /// <param name="context">Node context to analyze.</param>
    /// <param name="token">Cancellation Token.</param>
    /// <returns></returns>
    private ITypeSymbol? TransformStage(GeneratorSyntaxContext context, CancellationToken token)
    {
        //Get the class type information
        if (context.Node is ClassDeclarationSyntax classDeclaration)
            return context.SemanticModel.GetDeclaredSymbol(classDeclaration) as ITypeSymbol;
        else
            return null;
    }
}