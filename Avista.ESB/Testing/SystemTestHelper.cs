using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Avista.ESB.Admin.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avista.ESB.Testing
{
    public class SystemTestHelper : TestHelper
    {


        /// <summary>
        /// WriteTestFile
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContents"></param>
        public void WriteTestFile(string filePath, string fileContents)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine(fileContents);
                streamWriter.Flush();
            }
        }

        /// <summary>
        /// Wait
        /// </summary>
        /// <param name="seconds"></param>
        public void Wait(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        /// <summary>
        /// Deletes the old files under the directory
        /// </summary>
        public void ClearFiles(string directoryName)
        {
            DirectoryInfo outputMessageInfo = new DirectoryInfo(directoryName);

            foreach (FileInfo file in outputMessageInfo.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Finds the file name starts with required pattern
        /// </summary>
        /// <returns></returns>
        public string GetFileName(string directory, string pattternName)
        {
            var matches = Directory.GetFiles(directory)
                .Where(path => Regex.Match(path, pattternName).Success);
            string fileName = string.Empty;
            foreach (string file in matches)
            {
                fileName = System.IO.Path.GetFileName(file);
            }
            return fileName;
        }
    }
}