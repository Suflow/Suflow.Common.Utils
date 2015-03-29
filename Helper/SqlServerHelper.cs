////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Suflow.Common.Utils
{
    public class SqlServerHelper
    {
        private static string GetConnectionStringFromServerNameAndDatabaseName(string serverName, string databaseName)
        {
            return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", serverName, databaseName);
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static bool ExecuteNonQuery(string connectionString, string commandToExecute, int timeout = 1)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                using (var myCommand = new SqlCommand(commandToExecute, sqlConnection))
                {
                    sqlConnection.Open();
                    myCommand.CommandTimeout = timeout;
                    myCommand.ExecuteNonQuery();
                    SqlConnection.ClearPool(sqlConnection);
                    sqlConnection.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static IEnumerable<object> ExecuteReader(string connectionString, string commandToExecute)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                using (var myCommand = new SqlCommand(commandToExecute, sqlConnection))
                {

                    sqlConnection.Open();
                    var reader = myCommand.ExecuteReader();
                    var result = new List<object>();
                    while (reader.Read())
                    {
                        if (reader.FieldCount == 1)
                        {
                            result.Add(reader[0]);
                        }
                        else
                        {
                            var row = new List<object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                                row.Add(reader[i]);
                            result.Add(row);
                        }
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string GetServerName(string serverNameAndDatabaseName)
        {
            var separated = serverNameAndDatabaseName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (separated.Count() > 1)
                return separated[1];
            return string.Empty;
        }

        public static string GetDatabaseName(string serverNameAndDatabaseName)
        {
            var separated = serverNameAndDatabaseName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (separated.Count() > 2)
                return separated[2];
            return string.Empty;
        }

        public static bool TestConnection(string serverName, string databaseName)
        {
            var connectionString = GetConnectionStringFromServerNameAndDatabaseName(serverName, databaseName);
            return ExecuteNonQuery(connectionString, "select 1");
        }

        public static bool CreateDatabase(string serverName, string databaseName, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            var masterConnectionString = GetConnectionStringFromServerNameAndDatabaseName(serverName, "master");
            var command = "CREATE DATABASE [" + databaseName + "] ON  PRIMARY " +
                 "( NAME = N'" + databaseName + "', FILENAME = N'" + targetDirectory + databaseName +
                 ".mdf' , SIZE = 3072KB , FILEGROWTH = 1024KB ) " +
                 " LOG ON " +
                 "( NAME = N'" + databaseName + "_log', FILENAME = N'" + targetDirectory + databaseName +
                 "_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%) ";
            return ExecuteNonQuery(masterConnectionString, command);
        }

        public static bool BackupDatabase(string serverName, KeyValuePair<string, string>[] databaseNameAndOutputFilePairs)
        {
            const string query = @" BACKUP DATABASE [{0}] TO DISK = N'{1}' WITH FORMAT";
            foreach (var databaseOutputFile in databaseNameAndOutputFilePairs)
            {
                var directory = Path.GetDirectoryName(databaseOutputFile.Value);
                if (directory == null)
                    return false;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var connectionString = GetConnectionStringFromServerNameAndDatabaseName(serverName, databaseOutputFile.Key);
                var finalQuery = string.Format(query, databaseOutputFile.Key, databaseOutputFile.Value);
                if (!ExecuteNonQuery(connectionString, finalQuery, 600))
                    return false;
            }
            return true;
        }

        public static bool RestoreDatabase(string sqlServerName, KeyValuePair<string, string>[] databaseNameAndInputFilePairs)
        {
            var result = true;
            foreach (var databaseInputFile in databaseNameAndInputFilePairs)
            {
                var currResult = false;
                var directory = Path.GetDirectoryName(databaseInputFile.Value);
                if (directory == null || !Directory.Exists(directory) || !File.Exists(databaseInputFile.Value))
                    return false;
                var database = databaseInputFile.Key;
                var file = databaseInputFile.Value;

                currResult = NewBrokerForDatabase(sqlServerName, database);
                if (currResult)
                {
                    const string query0 = @" RESTORE DATABASE [{0}] FROM  DISK = N'{1}' WITH FILE = 1,  NOUNLOAD,  STATS = 10, REPLACE";
                    var connectionString = GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, "master");//why master? RESTORE cannot process database MEScontrol.netCORE because it is in use by this session. It is recomended that the master database be used when performing this operation.
                    currResult = ExecuteNonQuery(connectionString, string.Format(query0, database, file), 600);
                    if (currResult)
                        currResult = EnableBrokerForDatabase(sqlServerName, database);
                }
                result = result && currResult;
            }
            return result;
        }

        public static bool NewBrokerForDatabase(string sqlServerName, string databaseName)
        {
            //HAS OTHER CONNECTION
            var connectedSessions = GetConnectionsToDatabase(sqlServerName, databaseName);
            if (connectedSessions.Any())
            {
                return false;
            }

            var result = GetDatabasesFromServerName(sqlServerName).Contains(databaseName);
            if (result)
            {
                const string query1 = @" ALTER AUTHORIZATION ON Database::[{0}] TO sa";
                result = ExecuteNonQuery(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, databaseName),
                 string.Format(query1, databaseName));
                if (result)
                {
                    const string query2 = @" ALTER DATABASE [{0}] SET NEW_BROKER";
                    result = ExecuteNonQuery(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, databaseName),
                     string.Format(query2, databaseName), 10);
                }
            }
            else return true; //there is no database, so the broker will be new when it gets created.
            return result;
        }

        public static bool EnableBrokerForDatabase(string sqlServerName, string databaseName)
        {

            //HAS OTHER CONNECTION
            var connectedSessions = GetConnectionsToDatabase(sqlServerName, databaseName);
            if (connectedSessions.Any())
            {
                return false;
            }

            var result = GetDatabasesFromServerName(sqlServerName).Contains(databaseName);
            if (result)
            {
                const string query1 = @" ALTER AUTHORIZATION ON Database::[{0}] TO sa";
                result = ExecuteNonQuery(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, databaseName), string.Format(query1, databaseName));
                if (result)
                {
                    const string query2 = @" ALTER DATABASE [{0}] SET ENABLE_BROKER";
                    result = ExecuteNonQuery(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, databaseName), string.Format(query2, databaseName), 10);
                }
            }
            return result;//If database is doesnot exists, then broker will be new when it is created. 
        }

        public static IEnumerable<string> GetBackupFilesFromDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory, "*.bak");
                return files.Select(Path.GetFileName).ToList();
            }
            return new List<string>();
        }

        public static IEnumerable<string> GetDatabasesFromServerName(string sqlServerName)
        {
            const string query = "SELECT name  FROM master..sysdatabases";
            var executeReaderResult = ExecuteReader(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, "master"), query);
            if (executeReaderResult != null)
                return executeReaderResult.Select(a => a.ToString()).OrderBy(a => a);
            return new List<string>();
        }

        public static IEnumerable<object> GetConnectionsToDatabase(string sqlServerName, string databaseName)
        {
            const string query = @"
           SELECT sp.nt_username,
             sp.program_name,
             sd.name as [DatabaseName] 
           FROM master..sysprocesses sp, 
             master..sysdatabases sd 
           where sp.dbid=sd.dbid and
             sp.spid > 50 and
             sd.name = '{0}'";

            return ExecuteReader(GetConnectionStringFromServerNameAndDatabaseName(sqlServerName, "master"), string.Format(query, databaseName));
        }
    }
}
