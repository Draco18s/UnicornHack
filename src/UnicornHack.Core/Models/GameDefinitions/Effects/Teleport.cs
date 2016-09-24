using CSharpSerialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnicornHack.Models.GameDefinitions.Effects
{
    public class Teleport : AbilityEffect, ICSScriptSerializable
    {
        // Teleports items
        private static readonly CSScriptSerializer Serializer = new ConstructorCSScriptSerializer<Teleport>();
        public ExpressionSyntax GetCreation() => Serializer.GetCreation(this);
    }
}