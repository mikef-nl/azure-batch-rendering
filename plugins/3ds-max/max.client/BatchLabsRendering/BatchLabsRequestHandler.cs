using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace BatchLabsRendering
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
                Process.Start(baseUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error caught while calling BatchLabs.\n{ex.Message}\n{ex}");
            }
        }

        private static string SessionId => _sessionId.ToString();
    }
}
