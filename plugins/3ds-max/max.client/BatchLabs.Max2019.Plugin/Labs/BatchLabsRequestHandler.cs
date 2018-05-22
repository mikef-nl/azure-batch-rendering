
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Plugin.Common.Contract;

namespace BatchLabs.Max2019.Plugin.Labs
{
    public class BatchLabsRequestHandler : ILabsRequestHandler
    {
        private static Guid _sessionId = Guid.NewGuid();

        private const string BatchLabsBaseUrl = "ms-batchlabs://route";

        public void CallBatchLabs(string action, Dictionary<string, string> arguments = null)
        {
            var baseUrl = $"{BatchLabsBaseUrl}/{action}?session={SessionId}";
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
                Log.Instance.Error($"{ex.Message}\n{ex}", "Error caught while calling BatchLabs", true);
                MessageBox.Show($"Error caught while calling BatchLabs.\n{ex.Message}\n{ex}");
            }
        }

        private static string SessionId => _sessionId.ToString();
    }
}
