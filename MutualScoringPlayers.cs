using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace MutualScoringPlayers
{
    public class MutualScoringPlayers : BasePlugin
    {
        public override string ModuleName => "Mutual Scoring Players";
        public override string ModuleVersion => "v1.0";
        public override string ModuleAuthor => "E!N";

        private const int MaxPlayers = 64;
        private int[,] scores = new int[MaxPlayers + 1, MaxPlayers + 1];
        private int[] lastKiller = new int[MaxPlayers + 1];
        private int[,] killStreaks = new int[MaxPlayers + 1, MaxPlayers + 1];

        public override void Load(bool hotReload)
        {
            if (hotReload)
            {
                scores = new int[MaxPlayers + 1, MaxPlayers + 1];
                lastKiller = new int[MaxPlayers + 1];
                killStreaks = new int[MaxPlayers + 1, MaxPlayers + 1];
            }
        }

        [GameEventHandler]
        public HookResult PlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            info.DontBroadcast = true;

            var attacker = @event.Attacker;
            var victim = @event.Userid;

            if (attacker?.UserId == null || victim?.UserId == null || attacker.UserId == victim.UserId) return HookResult.Continue;

            int attackerId = (int)attacker.UserId;
            int victimId = (int)victim.UserId;

            if (lastKiller[victimId] == attackerId)
            {
                killStreaks[attackerId, victimId]++;
            }
            else
            {
                killStreaks[attackerId, victimId] = 1;
            }
            lastKiller[victimId] = attackerId;
            scores[attackerId, victimId]++;

            var currentStreak = killStreaks[attackerId, victimId];
            var attackerScore = scores[attackerId, victimId];
            var victimScore = scores[victimId, attackerId];

            attacker.PrintToChat($"{Localizer["Prefix"]} {Localizer["AttackerMessage", attacker.PlayerName, victim.PlayerName, attackerScore, victimScore]} {Localizer["AttackerStreak", currentStreak]}");
            victim.PrintToChat($"{Localizer["Prefix"]} {Localizer["VictimMessage", victim.PlayerName, attacker.PlayerName, victimScore, attackerScore]} {Localizer["VictimStreak", currentStreak]}");

            return HookResult.Continue;
        }
    }
}