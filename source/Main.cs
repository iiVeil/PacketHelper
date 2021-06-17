using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;


namespace NetworkHelper
{
    [BepInPlugin(Id, "NetworkHelper", Version)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "$safeprojectname$",
            AUTHOR = "",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;

        #endregion

        public const string Id = "library.iiveil.NetworkHelper";
        public const string Version = "1.0.0";
        public const string Name = "NetworkHelper";

        public Main()
        {
            log = Logger;
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
        }

        public void Start()
        {
        }
    }
}
