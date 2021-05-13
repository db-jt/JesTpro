// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro
{
    public static class DBChecker
    {
        public static void DoCheck(string connectionString)
        {
            var connStringValues = connectionString.Split(";");
            var conn = "";
            var dbName = "";
            foreach (var s in connStringValues)
            {
                if (s.StartsWith("database", StringComparison.InvariantCultureIgnoreCase))
                {
                    dbName = s.Substring(s.IndexOf("=") + 1);
                }
                else
                {
                    conn += $"{s};";
                }
            }
            var createDb = false;
            using (MySqlConnection dbconn = new MySqlConnection(conn))
            {
                dbconn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.schemata WHERE SCHEMA_NAME=@dbName", dbconn))
                {
                    cmd.Parameters.AddWithValue("@dbName", dbName);
                    if (long.Parse(cmd.ExecuteScalar().ToString()) == 0)
                    {
                        //missing DB
                        createDb = true;
                    }
                }
                if (createDb)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {dbName};", dbconn))
                    {
                        //cmd.Parameters.AddWithValue("@dbName", dbName);
                        cmd.ExecuteNonQuery();
                    }
                } 
                else
                {
                    //check for pre-created database
                    using (MySqlCommand cmd = new MySqlCommand("SELECT EXISTS(SELECT * FROM information_schema.tables WHERE table_schema=@dbName AND table_name = 'sql_migrations')", dbconn))
                    {
                        cmd.Parameters.AddWithValue("@dbName", dbName);
                        if (long.Parse(cmd.ExecuteScalar().ToString()) == 0)
                        {
                            //missing TABLES
                            createDb = true;
                        }
                    }
                }
                dbconn.Close();
            }
            using (MySqlConnection dbconn = new MySqlConnection(connectionString))
            {
                dbconn.Open();
                string sqlText = "";
                var fileName = "./SqlMigrations/CREATE_DB.sql";
                if (createDb)
                {
                    sqlText = File.ReadAllText(fileName);
                    using (MySqlCommand cmd = new MySqlCommand(sqlText, dbconn))
                    {
                        var error = "";
                        try
                        {
                            cmd.ExecuteNonQuery();
                            UpdateSqlMigrationTable(dbconn, fileName, error);
                        }
                        catch (Exception ex)
                        {
                            error = ex.Message + " - Stack:" + ex.StackTrace;
                        }

                        if (!string.IsNullOrEmpty(error))
                        {
                            throw new Exception($"Sql script [{fileName}] has failed! START ABORTED! Error: {error}");
                        }
                    }
                }
                foreach (string file in (Directory.EnumerateFiles("./SqlMigrations", "*.sql")).OrderBy(x => x))
                {
                    if (file != null && !file.Contains("CREATE_DB.sql"))
                    {
                        fileName = file.Replace("\\", "/");
                        if (MissingSqlMigration(dbconn, fileName))
                        {
                            sqlText = File.ReadAllText(file);
                            using (MySqlCommand cmd = new MySqlCommand(sqlText, dbconn))
                            {
                                //dbconn.Open();
                                var error = "";
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    error = ex.Message + " - Stack:" + ex.StackTrace;
                                }

                                UpdateSqlMigrationTable(dbconn, fileName, error);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    throw new Exception($"Sql script [{fileName}] has failed! START ABORTED! Error: {error}");
                                }
                            }
                        }

                    }
                }
                dbconn.Close();
            }
        }

        private static void UpdateSqlMigrationTable(MySqlConnection dbconn, string fileName, string error)
        {
            using (MySqlCommand cmd = new MySqlCommand("DELETE FROM sql_migrations WHERE `FileName`= @fileName", dbconn))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.ExecuteNonQuery();
            }
            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO sql_migrations (`FileName`, `Status`, `Error`) VALUES (@fileName, @status, @error)", dbconn))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(error) ? "Completed" : "Failed");
                cmd.Parameters.AddWithValue("@error", error);
                //dbconn.Open();
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Unable to add path sql_migration. START ABORTED");
                }
            }
        }

        private static bool MissingSqlMigration(MySqlConnection dbconn, string fileName)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM sql_migrations WHERE FileName=@fileName AND Status = 'Completed'", dbconn))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                if (long.Parse(cmd.ExecuteScalar().ToString()) != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
