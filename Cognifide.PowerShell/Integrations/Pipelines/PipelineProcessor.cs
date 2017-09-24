﻿using System;
using System.Linq;
using Cognifide.PowerShell.Core.Diagnostics;
using Cognifide.PowerShell.Core.Extensions;
using Cognifide.PowerShell.Core.Host;
using Cognifide.PowerShell.Core.Modules;
using Cognifide.PowerShell.Core.Settings;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Cognifide.PowerShell.Integrations.Pipelines
{
    public abstract class PipelineProcessor<TPipelineArgs> where TPipelineArgs : PipelineArgs
    {
        protected abstract string IntegrationPoint { get; }

        protected void Process(TPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            foreach (var libraryItem in ModuleManager.GetFeatureRoots(IntegrationPoint))
            {
                if (!libraryItem.HasChildren) return;

                foreach (var scriptItem in libraryItem.Children.ToList())
                {
                    if (!scriptItem.IsPowerShellScript())
                    {
                        continue;
                    }
                    using (var session = ScriptSessionManager.NewSession(ApplicationNames.Default, true))
                    {
                        var script = scriptItem.Fields[Templates.Script.Fields.ScriptBody].Value ?? string.Empty;
                        session.SetVariable("pipelineArgs", args);

                        try
                        {
                            session.SetExecutedScript(scriptItem);
                            session.ExecuteScriptPart(script, false);
                        }
                        catch (Exception ex)
                        {
                            PowerShellLog.Error(
                                $"Error while executing script in {GetType().FullName} pipeline processor.", ex);
                        }
                    }
                }
            }
        }
    }
}