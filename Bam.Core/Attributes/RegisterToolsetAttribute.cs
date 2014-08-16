// <copyright file="RegisterToolsetAttribute.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Bam.Core
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class RegisterToolsetAttribute : System.Attribute
    {
#if DEBUG
        private string name;
#endif

        public RegisterToolsetAttribute(string name, System.Type toolsetType)
        {
#if DEBUG
            this.name = name;
#endif

            State.Add("Toolset", name, ToolsetFactory.GetInstance(toolsetType));
        }

        public static void RegisterAll()
        {
            // need to use inheritence here as the base class is abstract
            var array = State.ScriptAssembly.GetCustomAttributes(typeof(RegisterToolsetAttribute),
true);
            if (null == array || 0 == array.Length)
            {
                throw new Exception("No toolchains were registered");
            }

#if DEBUG
            foreach (var a in array)
            {
                Log.DebugMessage("Registered toolset '{0}'", (a as RegisterToolsetAttribute).name);
            }
#endif
        }
    }
}