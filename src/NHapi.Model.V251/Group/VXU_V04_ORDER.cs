using System;
using System.Collections.Generic;

using NHapi.Base;
using NHapi.Base.Log;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V251.Segment;

namespace NHapi.Model.V251.Group
{
    ///<summary>
    ///Represents the VXU_V04_ORDER Group.  A Group is an ordered collection of message 
    /// segments that can repeat together or be optionally in/excluded together.
    /// This Group contains the following elements: 
    ///<ol>
    ///<li>0: ORC (Common Order) </li>
    ///<li>1: VXU_V04_TIMING (a Group object) optional repeating</li>
    ///<li>2: RXA (Pharmacy/Treatment Administration) </li>
    ///<li>3: RXR (Pharmacy/Treatment Route) optional </li>
    ///<li>4: VXU_V04_OBSERVATION (a Group object) optional repeating</li>
    ///</ol>
    ///</summary>
    [Serializable]
    public class VXU_V04_ORDER : AbstractGroup
    {

        ///<summary> 
        /// Creates a new VXU_V04_ORDER Group.
        ///</summary>
        public VXU_V04_ORDER(IGroup parent, IModelClassFactory factory) : base(parent, factory)
        {
            try
            {
                this.add(typeof(PID), true, false);
                this.add(typeof(ORC), true, false);
                this.add(typeof(VXU_V04_TIMING), false, true);
                this.add(typeof(RXA), true, false);
                this.add(typeof(RXR), false, false);
                this.add(typeof(VXU_V04_OBSERVATION), false, true);
            }
            catch (HL7Exception e)
            {
                HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error creating VXU_V04_ORDER - this is probably a bug in the source code generator.", e);
            }
        }
        public PID PID
        {
            get
            {
                PID ret = null;
                try
                {
                    ret = (PID)this.GetStructure("PID");
                }
                catch (HL7Exception e)
                {
                    HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                    throw new System.Exception("An unexpected error ocurred", e);
                }
                return ret;
            }
        }

        ///<summary>
        /// Returns ORC (Common Order) - creates it if necessary
        ///</summary>
        public ORC ORC
        {
            get
            {
                ORC ret = null;
                try
                {
                    ret = (ORC)this.GetStructure("ORC");
                }
                catch (HL7Exception e)
                {
                    HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                    throw new System.Exception("An unexpected error ocurred", e);
                }
                return ret;
            }
        }

        ///<summary>
        /// Returns  first repetition of VXU_V04_TIMING (a Group object) - creates it if necessary
        ///</summary>
        public VXU_V04_TIMING GetTIMING()
        {
            VXU_V04_TIMING ret = null;
            try
            {
                ret = (VXU_V04_TIMING)this.GetStructure("TIMING");
            }
            catch (HL7Exception e)
            {
                HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                throw new System.Exception("An unexpected error ocurred", e);
            }
            return ret;
        }

        ///<summary>
        ///Returns a specific repetition of VXU_V04_TIMING
        /// * (a Group object) - creates it if necessary
        /// throws HL7Exception if the repetition requested is more than one 
        ///     greater than the number of existing repetitions.
        ///</summary>
        public VXU_V04_TIMING GetTIMING(int rep)
        {
            return (VXU_V04_TIMING)this.GetStructure("TIMING", rep);
        }

        /** 
         * Returns the number of existing repetitions of VXU_V04_TIMING 
         */
        public int TIMINGRepetitionsUsed
        {
            get
            {
                int reps = -1;
                try
                {
                    reps = this.GetAll("TIMING").Length;
                }
                catch (HL7Exception e)
                {
                    string message = "Unexpected error accessing data - this is probably a bug in the source code generator.";
                    HapiLogFactory.GetHapiLog(GetType()).Error(message, e);
                    throw new System.Exception(message);
                }
                return reps;
            }
        }

        /** 
         * Enumerate over the VXU_V04_TIMING results 
         */
        public IEnumerable<VXU_V04_TIMING> TIMINGs
        {
            get
            {
                for (int rep = 0; rep < TIMINGRepetitionsUsed; rep++)
                {
                    yield return (VXU_V04_TIMING)this.GetStructure("TIMING", rep);
                }
            }
        }

        ///<summary>
        ///Adds a new VXU_V04_TIMING
        ///</summary>
        public VXU_V04_TIMING AddTIMING()
        {
            return this.AddStructure("TIMING") as VXU_V04_TIMING;
        }

        ///<summary>
        ///Removes the given VXU_V04_TIMING
        ///</summary>
        public void RemoveTIMING(VXU_V04_TIMING toRemove)
        {
            this.RemoveStructure("TIMING", toRemove);
        }

        ///<summary>
        ///Removes the VXU_V04_TIMING at the given index
        ///</summary>
        public void RemoveTIMINGAt(int index)
        {
            this.RemoveRepetition("TIMING", index);
        }

        ///<summary>
        /// Returns RXA (Pharmacy/Treatment Administration) - creates it if necessary
        ///</summary>
        public RXA RXA
        {
            get
            {
                RXA ret = null;
                try
                {
                    ret = (RXA)this.GetStructure("RXA");
                }
                catch (HL7Exception e)
                {
                    HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                    throw new System.Exception("An unexpected error ocurred", e);
                }
                return ret;
            }
        }

        ///<summary>
        /// Returns RXR (Pharmacy/Treatment Route) - creates it if necessary
        ///</summary>
        public RXR RXR
        {
            get
            {
                RXR ret = null;
                try
                {
                    ret = (RXR)this.GetStructure("RXR");
                }
                catch (HL7Exception e)
                {
                    HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                    throw new System.Exception("An unexpected error ocurred", e);
                }
                return ret;
            }
        }

        ///<summary>
        /// Returns  first repetition of VXU_V04_OBSERVATION (a Group object) - creates it if necessary
        ///</summary>
        public VXU_V04_OBSERVATION GetOBSERVATION()
        {
            VXU_V04_OBSERVATION ret = null;
            try
            {
                ret = (VXU_V04_OBSERVATION)this.GetStructure("OBSERVATION");
            }
            catch (HL7Exception e)
            {
                HapiLogFactory.GetHapiLog(GetType()).Error("Unexpected error accessing data - this is probably a bug in the source code generator.", e);
                throw new System.Exception("An unexpected error ocurred", e);
            }
            return ret;
        }

        ///<summary>
        ///Returns a specific repetition of VXU_V04_OBSERVATION
        /// * (a Group object) - creates it if necessary
        /// throws HL7Exception if the repetition requested is more than one 
        ///     greater than the number of existing repetitions.
        ///</summary>
        public VXU_V04_OBSERVATION GetOBSERVATION(int rep)
        {
            return (VXU_V04_OBSERVATION)this.GetStructure("OBSERVATION", rep);
        }

        /** 
         * Returns the number of existing repetitions of VXU_V04_OBSERVATION 
         */
        public int OBSERVATIONRepetitionsUsed
        {
            get
            {
                int reps = -1;
                try
                {
                    reps = this.GetAll("OBSERVATION").Length;
                }
                catch (HL7Exception e)
                {
                    string message = "Unexpected error accessing data - this is probably a bug in the source code generator.";
                    HapiLogFactory.GetHapiLog(GetType()).Error(message, e);
                    throw new System.Exception(message);
                }
                return reps;
            }
        }

        /** 
         * Enumerate over the VXU_V04_OBSERVATION results 
         */
        public IEnumerable<VXU_V04_OBSERVATION> OBSERVATIONs
        {
            get
            {
                for (int rep = 0; rep < OBSERVATIONRepetitionsUsed; rep++)
                {
                    yield return (VXU_V04_OBSERVATION)this.GetStructure("OBSERVATION", rep);
                }
            }
        }

        ///<summary>
        ///Adds a new VXU_V04_OBSERVATION
        ///</summary>
        public VXU_V04_OBSERVATION AddOBSERVATION()
        {
            return this.AddStructure("OBSERVATION") as VXU_V04_OBSERVATION;
        }

        ///<summary>
        ///Removes the given VXU_V04_OBSERVATION
        ///</summary>
        public void RemoveOBSERVATION(VXU_V04_OBSERVATION toRemove)
        {
            this.RemoveStructure("OBSERVATION", toRemove);
        }

        ///<summary>
        ///Removes the VXU_V04_OBSERVATION at the given index
        ///</summary>
        public void RemoveOBSERVATIONAt(int index)
        {
            this.RemoveRepetition("OBSERVATION", index);
        }

    }
}
