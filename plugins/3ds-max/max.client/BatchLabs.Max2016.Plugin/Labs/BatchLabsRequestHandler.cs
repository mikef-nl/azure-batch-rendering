
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using BatchLabs.Max2016.Plugin.Common;
using BatchLabs.Plugin.Common.Code;
using BatchLabs.Plugin.Common.Contract;
using BatchLabs.Plugin.Common.Resources;

namespace BatchLabs.Max2016.Plugin.Labs
{
    public class BatchLabsRequestHandler : ILabsRequestHandler
    {
        private static Guid _sessionId = Guid.NewGuid();

        public void CallBatchLabs(string action, Dictionary<string, string> arguments = null)
        {
            var baseUrl = $"{Constants.BatchLabsBaseUrl}/{action}?session={SessionId}";
            if (arguments != null)
            {
                foreach (var keyValue in arguments)
                {
                    baseUrl = $"{baseUrl}&{keyValue.Key}={Uri.EscapeDataString(keyValue.Value)}";
                }
            }

            try
            {
                Log.Instance.Debug($"Calling BatchLabs with URL: {baseUrl}");
                Process.Start(baseUrl);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{ex.Message}\n{ex}", Strings.BatchLabs_RequestError, true);
                MessageBox.Show($"{Strings.BatchLabs_RequestError}.\n{ex.Message}\n{ex}");
            }
        }

        private static string SessionId => _sessionId.ToString();
    }
}
