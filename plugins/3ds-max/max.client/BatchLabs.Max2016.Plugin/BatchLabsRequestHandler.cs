
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using BatchLabs.Max2016.Plugin.Common;

namespace BatchLabs.Max2016.Plugin
{
    public class BatchLabsRequestHandler
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
                    baseUrl = $"{baseUrl}&{keyValue.Key}={keyValue.Value}";
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
