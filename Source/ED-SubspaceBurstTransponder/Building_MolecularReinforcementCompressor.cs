using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using RimWorld;
using Verse;

namespace ED_ReinforcedStuff
{
    public class Building_MolecularReinforcementCompressor : Building
    {
        const int STUFF_AMMOUNT_REQUIRED = 10;
        
        CompPowerTrader m_Power;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            this.m_Power = base.GetComp<CompPowerTrader>();
        }

        public override void TickRare()
        {
           // Log.Message("TickRare");

            base.TickRare();

            if (this.m_Power.PowerOn)
            {
                this.TrySpawnReinforcedStuff();
            }
        }

        private void TrySpawnReinforcedStuff()
        {

            //Thing _NewResource = ThingMaker.MakeThing(ThingDef.Named("PlasteelReinforced"));
            Thing _OldResourceThing = this.GetValidThingStack();
            if (_OldResourceThing == null) { return; }

            ThingDef _NewResourceDef = this.GetReinforcedVersion(_OldResourceThing);
            if (_NewResourceDef == null) { return; }

            _OldResourceThing.SplitOff(10);

            Thing _NewResource = ThingMaker.MakeThing(_NewResourceDef);
            GenPlace.TryPlaceThing(_NewResource, this.InteractionCell, ThingPlaceMode.Near);

        }

        private Thing GetValidThingStack()
        {
            List<IntVec3> _Cells = Enumerable.ToList<IntVec3>(Enumerable.Where<IntVec3>(GenAdj.CellsAdjacentCardinal((Thing)this), (Func<IntVec3, bool>)(c => GenGrid.InBounds(c))));

            List<Thing> _closeThings = new List<Thing>();

            foreach (IntVec3 _Cell in _Cells)
            {
                _closeThings.AddRange(GridsUtility.GetThingList(_Cell));
            }

            foreach (Thing _TempThing in _closeThings)
            {
                if (_TempThing.stackCount >= Building_MolecularReinforcementCompressor.STUFF_AMMOUNT_REQUIRED)
                {
                    ThingDef _ReinforcedVersion = this.GetReinforcedVersion(_TempThing);

                    if (_ReinforcedVersion != null)
                    {
                        return _TempThing;
                    }
                }
            }

            return null;
        }


        ThingDef GetReinforcedVersion(Thing sourceStuff)
        {
            //Log.Message("GetReinforcedVersion checking: " + sourceStuff.def.defName + " - " + sourceStuff.Stuff);

            //SteelReinforced
            if (sourceStuff.def == ThingDefOf.Steel)
            {
                return ThingDef.Named("SteelReinforced");
            }

            //PlasteelReinforced
            if (sourceStuff.def == ThingDefOf.Plasteel)
            {
                return ThingDef.Named("PlasteelReinforced");
            }

            //WoodLogReinforced
            if (sourceStuff.def == ThingDefOf.WoodLog)
            {
                return ThingDef.Named("WoodLogReinforced");
            }

            //BlocksSandstoneReinforced
            if (sourceStuff.def == ThingDef.Named("BlocksSandstone"))
            {
                return ThingDef.Named("BlocksSandstoneReinforced");
            }

            //BlocksGraniteReinforced
            if (sourceStuff.def == ThingDef.Named("BlocksGranite"))
            {
                return ThingDef.Named("BlocksGraniteReinforced");
            }

            //BlocksLimestoneReinforced
            if (sourceStuff.def == ThingDef.Named("BlocksLimestone"))
            {
                return ThingDef.Named("BlocksLimestoneReinforced");
            }

            //BlocksSlateReinforced
            if (sourceStuff.def == ThingDef.Named("BlocksSlate"))
            {
                return ThingDef.Named("BlocksSlateReinforced");
            }

            //BlocksMarbleReinforced
            if (sourceStuff.def == ThingDef.Named("BlocksMarble"))
            {
                return ThingDef.Named("BlocksMarbleReinforced");
            }

            return null;
        }

    }
}



//private List<Thing> FindValidStuffNearBuilding(Thing centerBuilding, int radius)
//{

//    //IEnumerable<Thing> _closeThings = GenRadial.RadialDistinctThingsAround(centerBuilding.Position, radius, true);

//    List<Thing> _closeThings = new List<Thing>();

//    List<IntVec3> _Cells = Enumerable.ToList<IntVec3>(Enumerable.Where<IntVec3>(GenAdj.CellsAdjacentCardinal((Thing)this), (Func<IntVec3, bool>)(c => GenGrid.InBounds(c))));

//    foreach (IntVec3 _Cell in _Cells)
//    {
//        _closeThings.AddRange(GridsUtility.GetThingList(_Cell));
//    }

//    List<Thing> _ValidCloseThings = new List<Thing>();

//    foreach (Thing _TempThing in _closeThings)
//    {
//        if (_TempThing.stackCount > Building_MolecularReinforcmentCompressor.STUFF_AMMOUNT_REQUIRED)
//        {
//            ThingDef _ReinforcedVersion = this.GetReinforcedVersion(_TempThing);

//            if (_ReinforcedVersion != null)
//            {
//                _ValidCloseThings.Add(_TempThing);
//                _TempThing.stackCount -= Building_MolecularReinforcmentCompressor.STUFF_AMMOUNT_REQUIRED;
//            }
//        }
//        //if (tempThing.def.category == ThingCategory.Item)
//        //{
//        //    validCloseThings.Add(tempThing);
//        //}
//    }
//    return _ValidCloseThings;
//}