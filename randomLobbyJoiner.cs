using Splotch;
using Splotch.Event;
using HarmonyLib;
using Steamworks.Data;
using Steamworks;
using System.Reflection;
using JetBrains.Annotations;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RandomLobbyJoiner
{
    public class RandomLobbyJoiner : SplotchMod
    {
        public static Random Random = new Random();
        public override void OnLoad()
        {
            Logger.Log("hefhakwdn iehnfisoie");
            EventManager.RegisterEventListener(typeof(Events));
            Harmony.PatchAll(typeof(HarmonyPatches));

            JoinRandomLobby();
        }

        public static async void JoinRandomLobby()
        {
            var validLobbies = await SteamMatchmaking.LobbyList.WithKeyValue("ownerNameDataString", "Zapray").RequestAsync();
            if (validLobbies != null && validLobbies.Length == 1)
            {
                JoinLobby(validLobbies[0]);
            }
            else
            {
                JoinRandomLobby();
            }
            return;
            /*var validLobbies = await GetValidLobbies();
            if (validLobbies != null)
            {
                Lobby selectedLobby = validLobbies[Random.Next(validLobbies.Length)];
                JoinLobby(selectedLobby);
            } else
            {
                Lobby? lobby = await SteamMatchmaking.CreateLobbyAsync(4);
                if (!lobby.HasValue)
                {
                    Logger.Error("Lobby created but not correctly instantiated");
                    throw new Exception();
                }
                SteamManager.instance.currentLobby = lobby.Value;
                SteamManager.instance.currentLobby.SetData("isFriendLobby", "FALSE");

                SteamManager.instance.currentLobby.SetData("canBeJoinedByMatchMaking", "true");
                SteamManager.instance.currentLobby.SetData("ownerNameDataString", SteamClient.Name);

                SteamManager.instance.currentLobby.SetPublic();


                GameLobby.nrOfAbilities = Settings.Get().NumberOfAbilities;
            }*/
        }

        public static async Task<Lobby[]> GetValidLobbies()
        {
            var query = SteamMatchmaking.LobbyList.WithKeyValue("canBeJoinedByMatchMaking", "true");
            var matchmakinglobbies = await query.RequestAsync();
            if (matchmakinglobbies == null || !matchmakinglobbies.Any()) return null;
            
            var threeplayerlobbies = await query.WithSlotsAvailable(1).RequestAsync();
            if (threeplayerlobbies.Any()) return threeplayerlobbies;

            var twoplayerlobbies = await query.WithSlotsAvailable(2).RequestAsync();
            if (twoplayerlobbies.Any()) return twoplayerlobbies;

            var oneplayerlobbies = await query.WithSlotsAvailable(3).RequestAsync();
            if (oneplayerlobbies.Any()) return oneplayerlobbies;
            return null;
        }

        public static void JoinLobby(Lobby lobby)
        {
            MethodInfo joinLobby = AccessTools.Method(typeof(SteamManager), "JoinLobby");
            joinLobby.Invoke(SteamManager.instance, new object[] { lobby });
        }
    }

    public static class Events
    {
        
    }

    public static class HarmonyPatches
    {
        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.LeaveLobby))]
        [HarmonyPostfix]
        public static void OnLobbyLeave()
        {
            RandomLobbyJoiner.JoinRandomLobby();
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.StartHostedGame))]
        [HarmonyPostfix]
        public static void StartGame()
        {
            SteamManager.instance.currentLobby.SetData("canBeJoinedByMatchMaking", "false");
        }
    }
}