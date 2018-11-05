using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;

namespace HP.Practices.Security
{
    public static class SecurityManager
    {
        /// <summary>
        /// Grant permissions to a user to access a specified file or directory.
        /// </summary>
        /// <param name="userId">The user id to be granted permission.</param>
        /// <param name="path">The path of the directory or file to be granted permission.</param>
        /// <param name="permissionList">The comma delimited list of permissions to be granted.</param>
        public static void AddFilePermission(string userId, string path, string permissionList)
        {
            try
            {
                // Convert the permissionList to a rights mask.
                FileSystemRights rights = 0;
                char[] delimiters = { ',' };
                string[] permissions = permissionList.Split(delimiters);
                foreach (string permission in permissions)
                {
                    FileSystemRights right = (FileSystemRights)Enum.Parse(typeof(FileSystemRights), permission, true);
                    rights = rights | right;
                }
                // Add security based on whether we are dealing with a directory or a file.
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    AddDirectorySecurity(path, userId, rights, AccessControlType.Allow);
                }
                else
                {
                    AddFileSecurity(path, userId, rights, AccessControlType.Allow);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to grant " + permissionList + " to " + userId + " for " + path + ". " + e.Message);
            }
        }

        /// <summary>
        /// Adds an ACL entry on the specified directory for the specified account. 
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        public static void AddDirectorySecurity(string directoryName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(new FileSystemAccessRule(account, rights, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, controlType));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        /// <summary>
        /// Adds an ACL entry on the specified file for the specified account.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        public static void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FileSecurity fileSecurity = fileInfo.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));
            fileInfo.SetAccessControl(fileSecurity);
        }

        /// <summary>
        /// Removes an ACL entry on the specified file for the specified account.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        public static void RemoveFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FileSecurity fileSecurity = fileInfo.GetAccessControl();
            fileSecurity.RemoveAccessRule(new FileSystemAccessRule(account, rights, controlType));
            fileInfo.SetAccessControl(fileSecurity);
        }

    }
}
