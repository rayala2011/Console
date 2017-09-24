﻿using Cognifide.PowerShell.Core.Extensions;
using Cognifide.PowerShell.Core.Utility;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace Cognifide.PowerShell.Client.Controls
{
    public class IseContextPanel : IseContextPanelBase
    {
        protected override Item Button1 => Factory.GetDatabase("core").GetItem("{C733DE04-FFA2-4DCB-8D18-18EB1CB898A3}");
        protected override Item Button2 => Factory.GetDatabase("core").GetItem("{0C784F54-2B46-4EE2-B0BA-72384125E123}");
        protected override string Label1 => ContextItem != null ? ContextItem.GetProviderPath().EllipsisString(50) : Texts.IseContextPanel_Render_none;
        protected override string Icon1 => ContextItem != null ? ContextItem.Appearance.Icon : Button1.Appearance.Icon;
        protected override string Label2 => CommandContext.Parameters["currentSessionName"];
        protected override string Icon2 => string.Empty;

    }
}