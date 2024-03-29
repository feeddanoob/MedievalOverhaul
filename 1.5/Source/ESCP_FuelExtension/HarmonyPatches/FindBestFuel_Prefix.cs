﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace ESCP_FuelExtension
{
    [HarmonyPatch(typeof(RefuelWorkGiverUtility))]
    [HarmonyPatch("FindBestFuel")]
    public class FindBestFuelPatch
    {
        [HarmonyPrefix]
        public static bool FindBestFuel_Prefix(Pawn pawn, Thing refuelable, ref Thing __result)
        {
            if (Utility_OnStartup.LWMFuelFilterIsEnabled || !(refuelable is Building))
            {
                return true;
            }
            CompRefuelable comp1 = refuelable.TryGetComp<CompRefuelable>();
            CompStoreFuelThing comp2 = refuelable.TryGetComp<CompStoreFuelThing>();
            if (comp1 != null && comp2 != null)
            {
                ThingFilter filter = refuelable.TryGetComp<CompStoreFuelThing>().AllowedFuelFilter;
                Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && filter.Allows(x);
                __result = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                return false;
            }
            return true;
        }
    }
}