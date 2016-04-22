using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace SubspaceBurstTransponder
{
    class Building_SubspaceBurstTransponder : Building
    {

        CompPowerTrader m_Power;

        private static Texture2D UI_CALL;

        private const int MAX_POWER = 100;
        private const int CHARGE_RATE = 1;
        private const int DISCHARGE_RATE = 2;

        private int m_CurrentChargeLevel = 0;

        private enumTransponderMode m_Mode = enumTransponderMode.Charging;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            Building_SubspaceBurstTransponder.UI_CALL = ContentFinder<Texture2D>.Get("CallShip", true);
            this.m_Power = base.GetComp<CompPowerTrader>();
        }

        //Saving game
        public override void ExposeData()
        {
            base.ExposeData();

            // Scribe_Deep.LookDeep(ref shieldField, "shieldField");
            // Scribe_Values.LookValue(ref m_Mode, "m_Mode");
            //Scribe_Collections.LookList<Thing>(ref listOfBufferThings, "listOfBufferThings", LookMode.Deep, (object)null);
            Scribe_Values.LookValue(ref m_CurrentChargeLevel, "m_CurrentChargeLevel");
            Scribe_Values.LookValue(ref m_Mode, "m_Mode");
        }

        public override void TickRare()
        {
            base.TickRare();

            if (this.m_Mode == enumTransponderMode.Charging)
            {
                if (this.m_Power.PowerOn)
                {
                    this.m_CurrentChargeLevel += Building_SubspaceBurstTransponder.CHARGE_RATE;

                    if (this.m_CurrentChargeLevel > Building_SubspaceBurstTransponder.MAX_POWER)
                    {
                        this.m_CurrentChargeLevel = Building_SubspaceBurstTransponder.MAX_POWER;
                        this.m_Mode = enumTransponderMode.Charged;
                    }
                }
            }

            if (this.m_Mode == enumTransponderMode.Transmitting)
            {
                this.m_CurrentChargeLevel -= Building_SubspaceBurstTransponder.DISCHARGE_RATE;

                if (this.m_CurrentChargeLevel <= 0)
                {
                    this.m_CurrentChargeLevel = 0;
                    this.m_Mode = enumTransponderMode.Charged;

                    this.SummonTradeShip();
                }
            }
        }

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
                act.icon = Building_SubspaceBurstTransponder.UI_CALL;
                act.defaultLabel = "Call Ship";
                act.defaultDesc = "Call Ship";
                act.activateSound = SoundDef.Named("Click");
                //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                //act.groupKey = 689736;
                yield return act;
            }

        }

        public override string GetInspectString()
        {

            StringBuilder stringBuilder = new StringBuilder();

            if (this.m_Mode == enumTransponderMode.Charged)
            {
                stringBuilder.AppendLine("Fully Charged");
            }
            else if (this.m_Mode == enumTransponderMode.Charging)
            {
                stringBuilder.AppendLine("Charging: " + this.m_CurrentChargeLevel + " / " + Building_SubspaceBurstTransponder.MAX_POWER);
            }
            else if (this.m_Mode == enumTransponderMode.Transmitting)
            {
                stringBuilder.AppendLine("Transmitting: " + this.m_CurrentChargeLevel);
            }

            stringBuilder.Append(base.GetInspectString());
            return stringBuilder.ToString();
        }

        public void StartTransmit()
        {
            if (this.m_Mode == enumTransponderMode.Charged)
            {
                this.m_Mode = enumTransponderMode.Transmitting;
            }
            else
            {
                Messages.Message("Insufficient Charge Level to Contact Ship.", MessageSound.RejectInput);
            }
        }

        public void SummonTradeShip()
        {
            //QueuedIncident _Temp = new QueuedIncident();
            //_Temp.

            //Verse.Find.Storyteller.incidentQueue.Add(_Temp);

            List<IncidentDef> _DefList = DefDatabase<IncidentDef>.AllDefs.ToList();

            foreach (IncidentDef _Def in _DefList)
            {
                Log.Message(_Def.defName);
            }

            IncidentDef _DefOrbitalTraderArrival = DefDatabase<IncidentDef>.GetNamed("OrbitalTraderArrival");
            IncidentParms _Params = new IncidentParms();
            _Params.forced = true;
            _DefOrbitalTraderArrival.Worker.TryExecute(_Params);
        }

    }

    public enum enumTransponderMode
    {

        Charging,

        Charged,

        Transmitting
    }
}

