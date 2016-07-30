using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace EnhancedDevelopment.SubspaceBurstTransponder
{
    [StaticConstructorOnStartup]
    class Building_SubspaceBurstTransponder : Building
    {

        CompPowerTrader m_Power;

        private static Texture2D UI_CALL;
        private static Texture2D UI_CALL_DISABLED;
        private static Texture2D UI_SHIP;
        private static Texture2D UI_CARAVAN;

        private const int MAX_CHARGELEVEL_SHIP = 1000;
        private const int MAX_CHARGELEVEL_CARAVAN = 500;
        private const int CHARGE_RATE = 1;
        private const int DISCHARGE_RATE = 2;

        private int m_CurrentChargeLevel = 0;

        private enumTransponderStatus m_Status = enumTransponderStatus.Charging;
        private emumTransponderMode m_Mode = emumTransponderMode.OrbitalTrader;

        //Constructor
        static Building_SubspaceBurstTransponder()
        {
            Building_SubspaceBurstTransponder.UI_CALL = ContentFinder<Texture2D>.Get("Call", true);
            Building_SubspaceBurstTransponder.UI_CALL_DISABLED = ContentFinder<Texture2D>.Get("CallDisabled", true);
            Building_SubspaceBurstTransponder.UI_SHIP = ContentFinder<Texture2D>.Get("Ship", true);
            Building_SubspaceBurstTransponder.UI_CARAVAN = ContentFinder<Texture2D>.Get("Caravan", true);
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            this.m_Power = base.GetComp<CompPowerTrader>();
        }

        private int CurrentMaxChargeLevel
        {
            get
            {
                if (this.m_Mode == emumTransponderMode.TraderCaravanArrival)
                {
                    return MAX_CHARGELEVEL_CARAVAN;
                }

                //Else it should be the ship mode.
                return MAX_CHARGELEVEL_SHIP;
            }
        }

        //Saving game
        public override void ExposeData()
        {
            base.ExposeData();

            // Scribe_Deep.LookDeep(ref shieldField, "shieldField");
            // Scribe_Values.LookValue(ref m_Mode, "m_Mode");
            //Scribe_Collections.LookList<Thing>(ref listOfBufferThings, "listOfBufferThings", LookMode.Deep, (object)null);
            Scribe_Values.LookValue(ref m_CurrentChargeLevel, "m_CurrentChargeLevel");
            Scribe_Values.LookValue(ref m_Status, "m_Status");
            Scribe_Values.LookValue(ref m_Mode, "m_Mode");

        }

        public override void TickRare()
        {
            base.TickRare();

            if (this.m_Status == enumTransponderStatus.Charging)
            {
                if (this.m_Power.PowerOn)
                {
                    if (DebugSettings.unlimitedPower)
                    {
                        this.m_CurrentChargeLevel += Building_SubspaceBurstTransponder.CHARGE_RATE * 20;
                    }
                    else
                    {
                        this.m_CurrentChargeLevel += Building_SubspaceBurstTransponder.CHARGE_RATE;
                    }

                    if (this.m_CurrentChargeLevel > this.CurrentMaxChargeLevel)
                    {
                        this.m_CurrentChargeLevel = this.CurrentMaxChargeLevel;
                        this.m_Status = enumTransponderStatus.Charged;
                    }
                }
            }

            if (this.m_Status == enumTransponderStatus.Transmitting)
            {
                if (DebugSettings.unlimitedPower)
                {
                    this.m_CurrentChargeLevel -= Building_SubspaceBurstTransponder.DISCHARGE_RATE * 20;
                }
                else
                {
                    this.m_CurrentChargeLevel -= Building_SubspaceBurstTransponder.DISCHARGE_RATE;
                }

                if (this.m_CurrentChargeLevel <= 0)
                {
                    this.m_CurrentChargeLevel = 0;
                    this.m_Status = enumTransponderStatus.Charging;

                    this.SummonTrader();
                }
            }
        }

        #region UI

        public override IEnumerable<Gizmo> GetGizmos()
        {
            //Add the stock Gizmoes
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            if (true)
            {
                Command_Action act = new Command_Action();
                //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                act.action = () => this.StartTransmit();

                if (this.m_Status == enumTransponderStatus.Charged)
                {
                    act.icon = Building_SubspaceBurstTransponder.UI_CALL;
                }
                else
                {
                    act.icon = Building_SubspaceBurstTransponder.UI_CALL_DISABLED;
                }

                act.defaultLabel = "Call";
                act.defaultDesc = "Call";
                act.activateSound = SoundDef.Named("Click");
                //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                //act.groupKey = 689736;
                yield return act;
            }

            if (true)
            {
                Command_Action act = new Command_Action();
                //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                act.action = () => this.SwitchMode();
                if (this.m_Mode == emumTransponderMode.OrbitalTrader)
                {
                    act.icon = Building_SubspaceBurstTransponder.UI_SHIP;
                    act.defaultLabel = "Ship";
                    act.defaultDesc = "Ship";
                }
                else if (this.m_Mode == emumTransponderMode.TraderCaravanArrival)
                {
                    act.icon = Building_SubspaceBurstTransponder.UI_CARAVAN;
                    act.defaultLabel = "Caravan";
                    act.defaultDesc = "Caravan";
                }
                else
                {
                    // This should never happen, but just incase.

                    act.icon = Building_SubspaceBurstTransponder.UI_CALL;
                    act.defaultLabel = "ERROR";
                    act.defaultDesc = "ERROR";
                }

                act.activateSound = SoundDef.Named("Click");
                //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                //act.groupKey = 689736;
                yield return act;
            }

        }

        public override string GetInspectString()
        {

            StringBuilder stringBuilder = new StringBuilder();

            if (this.m_Status == enumTransponderStatus.Charged)
            {
                stringBuilder.AppendLine("Fully Charged");
            }
            else if (this.m_Status == enumTransponderStatus.Charging)
            {
                stringBuilder.AppendLine("Charging: " + this.m_CurrentChargeLevel + " / " + this.CurrentMaxChargeLevel);
            }
            else if (this.m_Status == enumTransponderStatus.Transmitting)
            {
                stringBuilder.AppendLine("Transmitting: " + this.m_CurrentChargeLevel);
            }

            stringBuilder.Append(base.GetInspectString());
            return stringBuilder.ToString();
        }

        #endregion

        #region Commands

        public void StartTransmit()
        {
            if (this.m_Status == enumTransponderStatus.Charged)
            {
                if (!this.AnyOtherTransmitionOnSameMode())
                {
                    this.m_Status = enumTransponderStatus.Transmitting;
                }
                else
                {
                    Messages.Message("Only One Transponder Can Transmit at a Time.", MessageSound.Negative);
                }
            }
            else
            {
                Messages.Message("Insufficient Charge Level to Contact Ship.", MessageSound.RejectInput);
            }
        }
        public void SummonTrader()
        {
            //QueuedIncident _Temp = new QueuedIncident();
            //_Temp.

            //Verse.Find.Storyteller.incidentQueue.Add(_Temp);

            List<IncidentDef> _DefList = DefDatabase<IncidentDef>.AllDefs.ToList();

            /*foreach (IncidentDef _Def in _DefList)
            {
                Log.Message(_Def.defName);
            }*/

            if (this.m_Mode == emumTransponderMode.OrbitalTrader)
            {
                IncidentDef _DefOrbitalTraderArrival = DefDatabase<IncidentDef>.GetNamed("OrbitalTraderArrival");
                IncidentParms _Params = new IncidentParms();
                _Params.forced = true;
                _DefOrbitalTraderArrival.Worker.TryExecute(_Params);
            }
            else
            {
                IncidentDef _DefOrbitalTraderArrival = DefDatabase<IncidentDef>.GetNamed("TraderCaravanArrival");
                IncidentParms _Params = new IncidentParms();
                _Params.forced = true;
                _DefOrbitalTraderArrival.Worker.TryExecute(_Params);
            }
        }
        public void SwitchMode()
        {
            this.m_Status = enumTransponderStatus.Charging;
            //this.m_CurrentChargeLevel = 0;

            if (this.m_Mode == emumTransponderMode.OrbitalTrader)
            {
                this.m_Mode = emumTransponderMode.TraderCaravanArrival;
            }
            else
            {
                this.m_Mode = emumTransponderMode.OrbitalTrader;
            }
        }

        #endregion //Commands

        public bool AnyOtherTransmitionOnSameMode()
        {
            IEnumerable<Building> _SubspaceBurstTransponderBuildings = Find.ListerBuildings.allBuildingsColonist.Where<Building>(t => t.def.defName == "Buildings_SubspaceBurstTransponder");

            if (_SubspaceBurstTransponderBuildings != null)
            {
                //List<Thing> fireTo
                foreach (Building_SubspaceBurstTransponder _CurrentBuilding in _SubspaceBurstTransponderBuildings.ToList())
                {
                    if (_CurrentBuilding != this)
                    {
                        if (_CurrentBuilding.m_Status == enumTransponderStatus.Transmitting)
                        {
                            if (_CurrentBuilding.m_Mode == this.m_Mode)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

    }

    public enum enumTransponderStatus
    {

        Charging,

        Charged,

        Transmitting
    }

    public enum emumTransponderMode
    {
        OrbitalTrader,
        TraderCaravanArrival
    }
}

