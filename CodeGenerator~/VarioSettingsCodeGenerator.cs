using Microsoft.CodeAnalysis;

namespace Damdor.Vario.CodeGenerator
{
    [Generator]
    public class VarioSettingsCodeGenerator : ClassRegisterViaAttributeCodeGenerator
    {
        public const string NumericOperationsAttribute = "NumericOperations";
        public const string VarioVariableAttribute = "VarioVariable";

        public override string[] Attributes { get; } = { NumericOperationsAttribute, VarioVariableAttribute };

        public override void Execute(GeneratorExecutionContext context)
        {
            GenerateForAttribute(context, NumericOperationsAttribute, (sb, type) =>
            {
                sb.AppendLine($"        global::Damdor.Vario.VarioSettings.RegisterNumericOperations(new {type}());");
            });
            GenerateForAttribute(context, VarioVariableAttribute, (sb, type) =>
            {
                sb.AppendLine($"        global::Damdor.Vario.VarioSettings.RegisterVariableType(typeof({type}), typeof({type}).GetCustomAttribute<Damdor.Vario.VarioVariableAttribute>().Name);");
            });
        }

        
        
    }
}