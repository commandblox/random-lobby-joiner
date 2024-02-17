using Splotch;
using Splotch.Event;
using HarmonyLib;

namespace RandomLobbyJoiner
{
    public class RandomLobbyJoiner : SplotchMod
    {
        public override void OnLoad()
        {
            Logger.Log("hefhakwdn iehnfisoie");
            EventManager.RegisterEventListener(typeof(Events));
            Harmony.PatchAll(typeof(HarmonyPatches));
        }
    }

    public static class Events
    {
    }

    public static class HarmonyPatches
    {
}