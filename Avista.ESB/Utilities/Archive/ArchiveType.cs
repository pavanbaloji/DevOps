using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.Archive
{

    public class ArchiveType
    {
        # region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArchiveType()
        {
            this.Id = 0;
            this.Name = "Unknown";
            this.Active = true;
            this.DefaultExpiry = 10080;
        }

        /// <summary>
        /// Constructor that populates the ArchiveType from a data record.
        /// </summary>
        /// <param name="record">A data record containing data from one row of the ArchiveType table.</param>
        public ArchiveType(IDataRecord record)
        {
            this.Id = (int)record["Id"];
            this.Name = (string)record["Name"];
            this.Active = (bool)record["Active"];
            this.DefaultExpiry = (int)record["DefaultExpiry"];
        }
        #endregion

        # region public properties
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int DefaultExpiry { get; set; }
        #endregion
    }
}
