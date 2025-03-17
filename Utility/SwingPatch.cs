using HarmonyLib;
using SDG.Unturned;
using static TNTVehicleSystem.Main.Plugin;

namespace TNTVehicleSystem.Utility
{
    [HarmonyPatch(typeof(UseableMelee))]
    [HarmonyPatch("swing")]
    public class SwingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(UseableMelee __instance)
        {
            HandleSwing(__instance);
        }
    }
}