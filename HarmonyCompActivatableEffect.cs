﻿using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;

namespace CompActivatableEffect
{
    [StaticConstructorOnStartup]
    static class HarmonyCompActivatableEffect
    {
        static HarmonyCompActivatableEffect()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.comps.activator");

            harmony.Patch(typeof(Pawn).GetMethod("GetGizmos"), null, new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("GetGizmosPrefix")));
            harmony.Patch(typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"), null, new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("DrawEquipmentAimingPostFix")));
            harmony.Patch(typeof(Verb).GetMethod("TryStartCastOn"), new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("TryStartCastOnPrefix")), null);

        }

        

        //=================================== COMPACTIVATABLE

        public static bool TryStartCastOnPrefix(ref bool __result, Verb __instance)
        {
            Pawn pawn = __instance.caster as Pawn;
            if (pawn != null)
            {
                Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
                if (pawn_EquipmentTracker != null)
                {
                    //Log.Message("2");
                    ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                    if (thingWithComps != null)
                    {
                        //Log.Message("3");
                        CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                        if (compActivatableEffect != null)
                        {
                            if (compActivatableEffect.CurrentState != CompActivatableEffect.State.Activated)
                            {
                                if (Find.TickManager.TicksGame % 250 == 0) Messages.Message("DeactivatedWarning".Translate(new object[] {
                                    pawn.Label
                                }), MessageSound.RejectInput);
                                __result = false;
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        ///// <summary>
        ///// Prevents the user from having damage with the verb.
        ///// </summary>
        ///// <param name="__instance"></param>
        ///// <param name="__result"></param>
        ///// <param name="pawn"></param>
        //public static void GetDamageFactorForPostFix(Verb __instance, ref float __result, Pawn pawn)
        //{
        //    Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
        //    if (pawn_EquipmentTracker != null)
        //    {
        //        //Log.Message("2");
        //        ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

        //        if (thingWithComps != null)
        //        {
        //            //Log.Message("3");
        //            CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
        //            if (compActivatableEffect != null)
        //            {
        //                if (compActivatableEffect.CurrentState != CompActivatableEffect.State.Activated)
        //                {
        //                    //Messages.Message("DeactivatedWarning".Translate(), MessageSound.RejectInput);
        //                    __result = 0f;
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Prevents the user from using the verb.
        ///// </summary>
        ///// <param name="__instance"></param>
        ///// <param name="__result"></param>
        ///// <param name="pawn"></param>
        //public static bool IsStillUsableByPreFix(ref bool __result, Verb __instance, Pawn pawn)
        //{
        //    Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
        //    if (pawn_EquipmentTracker != null)
        //    {
        //        //Log.Message("2");
        //        ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

        //        if (thingWithComps != null)
        //        {
        //            //Log.Message("3");
        //            CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
        //            if (compActivatableEffect != null)
        //            {
        //                if (compActivatableEffect.CurrentState != CompActivatableEffect.State.Activated)
        //                {
        //                    //Messages.Message("DeactivatedWarning".Translate(), MessageSound.RejectInput);
        //                    __result = false;
        //                    return false;
        //                }

        //            }
        //        }
        //    }
        //    return true;
        //}


        /// <summary>
        /// Adds another "layer" to the equipment aiming if they have a
        /// weapon with a CompActivatableEffect.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eq"></param>
        /// <param name="drawLoc"></param>
        /// <param name="aimAngle"></param>
        public static void DrawEquipmentAimingPostFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);

            Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                    if (compActivatableEffect != null)
                    {
                        //Log.Message("4");
                        if (compActivatableEffect.Graphic != null)
                        {
                            if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated)
                            {
                                float num = aimAngle - 90f;
                                bool flip = false;
                                
                                if (aimAngle > 20f && aimAngle < 160f)
                                {
                                    //mesh = MeshPool.GridPlaneFlip(thingWithComps.def.graphicData.drawSize);
                                    num += eq.def.equippedAngleOffset;

                                }
                                else if (aimAngle > 200f && aimAngle < 340f)
                                {
                                    //mesh = MeshPool.GridPlane(thingWithComps.def.graphicData.drawSize);
                                    flip = true;
                                    num -= 180f;
                                    num -= eq.def.equippedAngleOffset;
                                }
                                else
                                {
                                    //mesh = MeshPool.GridPlaneFlip(thingWithComps.def.graphicData.drawSize);
                                    num += eq.def.equippedAngleOffset;
                                }

                                ThingWithComps eqComps = eq as ThingWithComps;
                                if (eqComps != null)
                                {
                                    ThingComp deflector = eqComps.AllComps.FirstOrDefault<ThingComp>((ThingComp y) => y.GetType().ToString() == "CompDeflector.CompDeflector");
                                    if (deflector != null)
                                    {
                                        bool isActive = (bool)AccessTools.Property(deflector.GetType(), "IsAnimatingNow").GetValue(deflector, null);
                                        if (isActive)
                                        {
                                            float numMod = (float)((int)AccessTools.Property(deflector.GetType(), "AnimationDeflectionTicks").GetValue(deflector, null));
                                            //float numMod2 = new float();
                                            //numMod2 = numMod;
                                            if (numMod > 0)
                                            {
                                                if (!flip) num += ((numMod + 1) / 2);
                                                else num -= ((numMod + 1) /2);
                                            }
                                        }
                                    }
                                }
                                num %= 360f;

                                //ThingWithComps eqComps = eq as ThingWithComps;
                                //if (eqComps != null)
                                //{
                                //    ThingComp deflector = eqComps.AllComps.FirstOrDefault<ThingComp>((ThingComp y) => y.GetType().ToString() == "CompDeflector.CompDeflector");
                                //    if (deflector != null)
                                //    {
                                //        float numMod = (float)((int)AccessTools.Property(deflector.GetType(), "AnimationDeflectionTicks").GetValue(deflector, null));
                                //        //Log.ErrorOnce("NumMod " + numMod.ToString(), 1239);
                                //numMod = (numMod + 1) / 2;
                                //if (subtract) num -= numMod;
                                //else num += numMod;
                                //    }
                                //}

                                Material matSingle = compActivatableEffect.Graphic.MatSingle;
                                //if (mesh == null) mesh = MeshPool.GridPlane(thingWithComps.def.graphicData.drawSize);

                                Vector3 s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                                Matrix4x4 matrix = default(Matrix4x4);
                                matrix.SetTRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), s);
                                if (!flip) Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0);
                                else Graphics.DrawMesh(MeshPool.plane10Flip, matrix, matSingle, 0);
                                //Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
                                
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Gizmo> gizmoGetter(CompActivatableEffect compActivatableEffect)
        {
            //Log.Message("5");
            if (compActivatableEffect.GizmosOnEquip)
            {
                //Log.Message("6");
                //Iterate EquippedGizmos
                IEnumerator<Gizmo> enumerator = compActivatableEffect.EquippedGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    //Log.Message("7");
                    Gizmo current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmosPrefix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            //Log.Message("1");
            Pawn_EquipmentTracker pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                    if (compActivatableEffect != null)
                    {
                        //Log.Message("4");
                        if (__instance != null)
                        {
                            if (__instance.Faction == Faction.OfPlayer)
                            {
                                __result = __result.Concat<Gizmo>(gizmoGetter(compActivatableEffect));
                            }
                            else
                            {
                                if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Deactivated)
                                {
                                    compActivatableEffect.Activate();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
