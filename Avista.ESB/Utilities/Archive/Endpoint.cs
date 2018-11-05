using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.Archive
{

    public class Endpoint
    {
        # region Constructor
        public Endpoint()
        {
            this.Id = 0;
            this.Name = "Unknown";
        }

        /// <summary>
        /// Constructor that populates the Endpoint from a data record.
        /// </summary>
        /// <param name="record">A data record containing data from one row of the Endpoint table.</param>
        public Endpoint(IDataRecord record)
        {
            this.Id = (int)record["Id"];
            this.Name = (string)record["Name"];
        }
        #endregion

        # region public properties
        public int Id { get; set; }
        public string Name { get; set; }
        #endregion
    }
}
