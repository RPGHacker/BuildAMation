// <copyright file="WinResourceCompilerAction.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>

[assembly: Opus.Core.RegisterAction(typeof(C.WinResourceCompilerAction))]

namespace C
{
    [Opus.Core.PreambleAction]
    public sealed class WinResourceCompilerAction : Opus.Core.IActionWithArguments
    {
        public WinResourceCompilerAction()
        {
            if (!Opus.Core.State.HasCategory("C"))
            {
                Opus.Core.State.AddCategory("C");
            }

            if (!Opus.Core.State.Has("C", "ToolToToolsetName"))
            {
                var map = new System.Collections.Generic.Dictionary<System.Type, string>();
                Opus.Core.State.Add("C", "ToolToToolsetName", map);
            }
        }

        private string WinResourceCompiler
        {
            get;
            set;
        }

        void Opus.Core.IActionWithArguments.AssignArguments(string arguments)
        {
            this.WinResourceCompiler = arguments;
        }

        string Opus.Core.IAction.CommandLineSwitch
        {
            get
            {
                return "-C.RC";
            }
        }

        string Opus.Core.IAction.Description
        {
            get
            {
                return "Assign the Windows resource compiler used.";
            }
        }

        bool Opus.Core.IAction.Execute()
        {
            Opus.Core.Log.DebugMessage("Windows resource compiler is '{0}'", this.WinResourceCompiler);

            var map = Opus.Core.State.Get("C", "ToolToToolsetName") as System.Collections.Generic.Dictionary<System.Type, string>;
            map[typeof(IWinResourceCompilerTool)] = this.WinResourceCompiler;

            return true;
        }
    }
}
