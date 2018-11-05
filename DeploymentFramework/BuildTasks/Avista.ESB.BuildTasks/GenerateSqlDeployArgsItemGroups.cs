using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;

namespace Avista.ESB.BuildTasks
{
    public class GenerateSqlDeployArgsItemGroups : Task
    {
        private ITaskItem[] _sourceSqlPackagesItemGroup;
        private ITaskItem[] _sqlPublishArgsItemGroup;

        [Output]
        public ITaskItem[] SqlPublishArgsItemGroup
        {
            get { return _sqlPublishArgsItemGroup; }
        }

        [Required]
        public ITaskItem[] SourceSqlPackages
        {
            get { return _sourceSqlPackagesItemGroup; }
            set { _sourceSqlPackagesItemGroup = value; }
        }

        public override bool Execute()
        {
            List<TaskItem> sqlDeployArgs = new List<TaskItem>();

            foreach (ITaskItem sourceAssembly in _sourceSqlPackagesItemGroup)
            {
                //for(int i=0; i< sourceAssembly.MetadataCount ; i++)
                //{
                //    this.Log.LogMessage(MessageImportance.Normal, "Name: {0}, Value: {1}", sourceAssembly.MetadataNames[i]);
                //}

                foreach(var it in sourceAssembly.MetadataNames)
                {
                    this.Log.LogMessage(MessageImportance.Normal, "Name: {0}, Value: {1}", it.ToString(), sourceAssembly.GetMetadata(it.ToString()));
                }

                string packagepath = sourceAssembly.GetMetadata("");
                string profilePath = Path.Combine(Path.GetDirectoryName(sourceAssembly.ItemSpec), Path.GetFileNameWithoutExtension(sourceAssembly.ItemSpec) + ".publish.xml");

                string args = string.Format("/Action:Publish /SourceFile:{0} /Profile:{1}", packagepath, profilePath);

                TaskItem dti = new TaskItem(args);
                sqlDeployArgs.Add(dti);

                this.Log.LogMessage(MessageImportance.Normal, "Prepared args for Sql deployment '{0}'.", dti.ItemSpec);
            }
            _sqlPublishArgsItemGroup = sqlDeployArgs.ToArray();
            return true;
        }
    }
}
