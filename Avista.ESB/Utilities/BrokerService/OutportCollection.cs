using Avista.ESB.Utilities.Logging;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.BrokerService
{
    [Serializable]
    public class OutportCollection
    {
        public int OutPortCount { get; set; }
        public List<string> OutPortList { get; set; }
        public Dictionary<string,string> OutPortDetails { get; set; }

        /// <summary>
        /// COnstructs OutPortCollection
        /// </summary>
        /// <param name="step"></param>
        public OutportCollection(IItineraryStep step)
        {

            Logger.WriteTrace("Preparing OutPort Collection List Started");

            Dictionary<string,string> itineraryStepDictionary = step.PropertyBag;

            List<string> keyList = new List<string>(itineraryStepDictionary.Keys);

            OutPortList = new List<string>();
            OutPortDetails = new Dictionary<string, string>();

            for(int i =0; i<keyList.Count; i++)
            {
                OutPortList.Add(keyList[i]);
                OutPortDetails.Add(keyList[i], itineraryStepDictionary[keyList[i]]);
            }

            this.OutPortCount = OutPortList.Count;
            Logger.WriteTrace("Total OutPorts: " + this.OutPortCount.ToString());
            Logger.WriteTrace("Preparing OutPort Collection List Completed");
        }

    }
}
