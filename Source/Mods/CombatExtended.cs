using System;

using HarmonyLib;

using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Combat Extended by NoImageAvailable</summary>
    /// <remarks>Incomplete</remarks>
    /// <see href="https://steamcommunity.com/workshop/filedetails/?id=1631756268"/>
    /// <see href="https://github.com/NoImageAvailable/CombatExtended"/>
    [MpCompatFor("Combat Extended")]
    public class CombatExtendedCompat
    {
        public CombatExtendedCompat(ModContentPack mod)
        {
            LongEventHandler.ExecuteWhenFinished(LateLoad);
        }

        static void LateLoad()
        {
            Type type;

            {
                type = AccessTools.TypeByName("CombatExtended.JobGiver_TakeAndEquip");
                MP.RegisterSyncMethod(type, "TryGiveJob");

                type = AccessTools.TypeByName("CombatExtended.CompAmmoUser");
                MP.RegisterSyncMethod(type, "TryStartReload");

                type = AccessTools.TypeByName("CombatExtended.Command_VerbTarget");
                MP.RegisterSyncMethod(type, "ProcessInput");

                type = AccessTools.TypeByName("CombatExtended.JobGiver_UpdateLoadout");
                MP.RegisterSyncMethod(type, "TryGiveJob");

                type = AccessTools.TypeByName("CombatExtended.Building_TurretGunCE");
                MP.RegisterSyncMethod(type, "TryStartShootSomething");

                type = AccessTools.TypeByName("CombatExtended.CompFireModes");

                var methods = new[]
                {
                    "ToggleAimMode",
                    "ToggleFireMode",
                    "ResetModes"
                };

                foreach (var method in methods)
                {
                    MP.RegisterSyncMethod(type, method);
                }

            }

        

        // RNG prevention
            {
                var methods = new[] {
                    AccessTools.Method("CombatExtended.CompSuppressable:AddSuppression"),
                    AccessTools.Method("CombatExtended.CompSuppressable:CompTick"),
                    AccessTools.Method("CombatExtended.Verb_ShootCE:TryCastShot"),
                    AccessTools.Method("CombatExtended.Verb_ShootCE:WarmupComplete"),
                    AccessTools.Method("CombatExtended.HediffComp_InfecterCE:CheckMakeInfection"),
                    AccessTools.Method("CombatExtended.ArmorUtilityCE:ApplyParryDamage"),
                    AccessTools.Method("CombatExtended.ProjectileCE:TryCollideWith"),
                    AccessTools.Method("CombatExtended.Verb_LaunchProjectileCE:WarmupComplete"),
                    AccessTools.Method("CombatExtended.WeatherTracker:MapComponentTick"),
                    //Adding this seems to cause desyncs?
                    //AccessTools.Method("CombatExtended.JobGiver_TakeAndEquip:TryGiveJob"),
                };

                foreach(var method in methods) {
                    MpCompat.harmony.Patch(method,
                        prefix: new HarmonyMethod(typeof(CombatExtendedCompat), nameof(FixRNGPre)),
                        postfix: new HarmonyMethod(typeof(CombatExtendedCompat), nameof(FixRNGPost))
                    );
                }
            }
        }
        static void FixRNGPre()
        {
            Rand.PushState(Find.TickManager.TicksAbs);
        }
        static void FixRNGPost()
        {
            Rand.PopState();
        }
    }
}