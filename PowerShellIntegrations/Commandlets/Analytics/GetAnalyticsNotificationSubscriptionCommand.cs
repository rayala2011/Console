﻿using System.Management.Automation;

namespace Cognifide.PowerShell.PowerShellIntegrations.Commandlets.Analytics
{
    [Cmdlet(VerbsCommon.Get, "AnalyticsNotificationSubscription")]
    [OutputType(new[] {typeof (NotificationSubscriptions)})]
    public class GetAnalyticsNotificationSubscriptionCommand : AnalyticsBaseCommand
    {
        protected override void ProcessRecord()
        {
            PipeQuery(Context.NotificationSubscriptions);
        }
    }
}