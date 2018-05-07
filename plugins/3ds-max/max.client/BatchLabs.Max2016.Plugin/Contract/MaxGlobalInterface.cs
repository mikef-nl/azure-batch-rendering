
using Autodesk.Max;

#if (DEBUG)
using BatchLabs.Max2016.Plugin.GlobalInterface;
#endif

namespace BatchLabs.Max2016.Plugin.Contract
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

        private static IGlobal GetGlobalInterface
        {
            get
            {
#if (DEBUG)
                return Autodesk.Max.GlobalInterface.Instance != null
                    ? Autodesk.Max.GlobalInterface.Instance
                    : new FakeGlobalInterface();
#else
                return Autodesk.Max.GlobalInterface.Instance;
#endif
            }
        }
    }
}
