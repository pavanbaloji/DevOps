using System;
using System.Data;

namespace Avista.ESB.Utilities.Archive
{
    public class ArchiveTag
    {
        # region Private variables
        private string tag = "";
        private ArchiveType archiveType = null;
        private Endpoint sourceSystem = null;
        private Endpoint targetSystem = null;
        private string description = null;
        #endregion

        # region Constructor
        public ArchiveTag()
        {
            tag = "Unknown";
            archiveType = ArchiveLookup.GetArchiveType("Unknown");
            sourceSystem = ArchiveLookup.GetEndpoint("Unknown");
            targetSystem = ArchiveLookup.GetEndpoint("Unknown");
            description = "Unknown";
        }

        /// <summary>
        /// Constructor that populates the ArchiveTag from a tag string.
        /// </summary>
        /// <param name="tag">
        /// A tag string containing either a simple tag, or a vertical bar delimited list of data
        /// including the tag, archive type, source system, target system, and description.
        /// </param>
        public ArchiveTag(string tag)
        {
            if (tag != null)
            {
                if (tag.Contains("|"))
                {
                    string[] fields = tag.Split('|');
                    this.tag = fields[0];
                    if (fields.Length >= 2)
                    {
                        archiveType = ArchiveLookup.GetArchiveType(fields[1]);
                        if (fields.Length >= 3)
                        {
                            sourceSystem = ArchiveLookup.GetEndpoint(fields[2]);
                            if (fields.Length >= 4)
                            {
                                targetSystem = ArchiveLookup.GetEndpoint(fields[3]);
                                if (fields.Length >= 5)
                                {
                                    description = fields[4];
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.tag = tag;
                }
            }
            if (String.IsNullOrEmpty(tag))
            {
                this.tag = Guid.NewGuid().ToString();
            }
            //b meek - per new requirements, we no longer default to 'Unknown' 
            if (archiveType == null)
            {
                archiveType = ArchiveLookup.GetArchiveType("NoValue");
            }
            if (sourceSystem == null)
            {
                sourceSystem = ArchiveLookup.GetEndpoint("NoValue");
            }
            if (targetSystem == null)
            {
                targetSystem = ArchiveLookup.GetEndpoint("NoValue");
            }
            if (description == null)
            {
                description = "";
            }
        }
        #endregion

        # region public properties

        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }
        public ArchiveType ArchiveType
        {
            get
            {
                return archiveType;
            }
            set
            {
                archiveType = value;
            }
        }
        public Endpoint SourceSystem
        {
            get
            {
                return sourceSystem;
            }
            set
            {
                sourceSystem = value;
            }
        }
        public Endpoint TargetSystem
        {
            get
            {
                return targetSystem;
            }
            set
            {
                targetSystem = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        #endregion
    }


}
