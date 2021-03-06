﻿using System;
using System.Linq;
using System.Reflection;

using Harmony;
using Verse;

namespace Multiplayer.Compat
{

    /// <summary>Stuffed Floors by Fluffy</summary>
    /// <see href="https://github.com/FluffierThanThou/StuffedFloors"/>
    [MpCompatFor("Stuffed Floors")]
    class StuffedFloorsCompat
    {
        static MethodInfo defDatabaseAddMethod;

        public StuffedFloorsCompat(ModContentPack mod)
        {
            Type[] generic = { typeof(BuildableDef) };

            defDatabaseAddMethod = AccessTools.Method(typeof(DefDatabase<>).MakeGenericType(generic), "Add", generic);

            MpCompat.harmony.Patch(
                AccessTools.Method("StuffedFloors.FloorTypeDef:GetStuffedTerrainDef"),
                postfix: new HarmonyMethod(typeof(StuffedFloorsCompat), nameof(GetStuffedTerrainDefPosfix))
                );
        }

        static void GetStuffedTerrainDefPosfix(TerrainDef __result)
        {
            defDatabaseAddMethod.Invoke(null, new object[] { __result });
        }
    }
}
