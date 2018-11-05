using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.Archive
{
    public class ArchiveLookup
    {
        private static Dictionary<string, ArchiveType> archiveTypeDictionary = null;
        private static Dictionary<string, Endpoint> endpointDictionary = null;

        static ArchiveLookup()
        {
            ObjectCache cache = MemoryCache.Default;
            string archiveTypeCacheItemName="archiveTypeDictionary";
            string endpointCacheItemName="endpointDictionary";

            archiveTypeDictionary = (Dictionary<string, ArchiveType>)cache[archiveTypeCacheItemName];
            if(archiveTypeDictionary == null)
            {
                System.Diagnostics.Debug.WriteLine("**************************LOADING ARCHIVE TYPE DICTIONARY FROM DATABSE**********************************");
                archiveTypeDictionary = LoadArchiveTypeDictionary();
                CacheItemPolicy archiveTypePolicy = new CacheItemPolicy();
                archiveTypePolicy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(24);
                cache.Set(archiveTypeCacheItemName, archiveTypeDictionary, archiveTypePolicy);
            }

            endpointDictionary = (Dictionary<string, Endpoint>)cache[endpointCacheItemName];
            if(endpointDictionary == null)
            {
                System.Diagnostics.Debug.WriteLine("**************************LOADING ENDPOINT DICTIONARY FROM DATABSE**********************************");
                endpointDictionary = LoadEndpointDictionary();
                CacheItemPolicy endpointPolicy = new CacheItemPolicy();
                endpointPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(24);
                cache.Set(endpointCacheItemName, endpointDictionary, endpointPolicy);
            }
        }

        private static Dictionary<string, Endpoint> LoadEndpointDictionary()
        {
            SqlServerConnection connection = null;
            Dictionary<string, Endpoint> endpoints =new Dictionary<string, Endpoint>(StringComparer.InvariantCultureIgnoreCase);
            try
            {
                // Connect to the database to load settings.
                string sql = null;
                SqlCommand sqlCommand = null;
                connection = new SqlServerConnection("MessageArchive");
                connection.RefreshConfiguration();
                connection.Open();

                // Load the dictionary of Endpoint records.
                sql = "SELECT [Id], [Name] " +
                        "FROM [MessageArchive].[dbo].[Endpoint]";
                sqlCommand = new SqlCommand(sql);
                using (SqlDataReader reader = connection.ExecuteReader(sqlCommand))
                {
                    while (reader.Read())
                    {
                        IDataRecord record = (IDataRecord)reader;
                        Endpoint endpoint = new Endpoint(record);
                        endpoints.Add(endpoint.Name, endpoint);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteWarning("Error while loading Tag data." + ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }

            return endpoints;
        }

        private static Dictionary<string, ArchiveType> LoadArchiveTypeDictionary()
        {
            SqlServerConnection connection = null;
            Dictionary<string, ArchiveType>  archiveTypes = new Dictionary<string, ArchiveType>(StringComparer.InvariantCultureIgnoreCase);
            try
            {
                // Connect to the database to load settings.
                string sql = null;
                SqlCommand sqlCommand = null;
                connection = new SqlServerConnection("MessageArchive");
                connection.RefreshConfiguration();
                connection.Open();
                // Load the dictionary of ArchiveType records.
                sql = "SELECT [Id], [Name], [Active], [DefaultExpiry] " +
                        "FROM [MessageArchive].[dbo].[ArchiveType]";
                sqlCommand = new SqlCommand(sql);
                using (SqlDataReader reader = connection.ExecuteReader(sqlCommand))
                {
                    while (reader.Read())
                    {
                        IDataRecord record = (IDataRecord)reader;
                        ArchiveType archiveType = new ArchiveType(record);
                        archiveTypes.Add(archiveType.Name, archiveType);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteWarning("Error while loading Tag data." + ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
            return archiveTypes;
        }

        /// <summary>
        /// Gets the archiveType record for a given archiveType name.
        /// </summary>
        /// <param name="name">The name of the archiveType to look up.</param>
        /// <returns>The matching archive type or null if it is not found.</returns>
        public static ArchiveType GetArchiveType(string name)
        {
            ArchiveType archiveType = null;
            if (name == "NoValue" || String.IsNullOrEmpty(name))
            {
                archiveType = new ArchiveType();
                archiveType.Id = -1;
                return archiveType;
            }
            if (!archiveTypeDictionary.TryGetValue(name, out archiveType))
            {
                if (!archiveTypeDictionary.TryGetValue("Unknown", out archiveType))
                {
                    archiveType = new ArchiveType();

                }
            }
            return archiveType;
        }

        /// <summary>
        /// Gets the endpoint record for a given endpoint name.
        /// </summary>
        /// <param name="name">The endpoint name to look for.</param>
        /// <returns>The matching endpoint record or bull if it is not found.</returns>
        public static Endpoint GetEndpoint(string name)
        {
            Endpoint endpoint = null;
            if (name == "NoValue" || String.IsNullOrEmpty(name))
            {
                endpoint = new Endpoint();
                endpoint.Id = -1;
                return endpoint;
            }

            if (!endpointDictionary.TryGetValue(name, out endpoint))
            {
                if (!endpointDictionary.TryGetValue("Unknown", out endpoint))
                {
                    endpoint = new Endpoint();
                }
            }
            return endpoint;
        }    

    }
}
