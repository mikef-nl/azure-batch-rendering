
using Autodesk.Max;

namespace BatchLabsRendering.Common
{
    /// <summary>
    /// Want to change this to be dependancy injected
    /// </summary>
    public class MaxGlobalInterface
    {
        private static IGlobal _instance;

        private MaxGlobalInterface() { }

        public static IGlobal Instance => 
            _instance ?? (_instance = GetGlobalInterface);

        private static IGlobal GetGlobalInterface => GlobalInterface.Instance != null
            ? GlobalInterface.Instance
            : new FakeGlobalInterface();
    }
}
