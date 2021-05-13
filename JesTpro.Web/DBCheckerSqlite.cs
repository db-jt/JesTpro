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

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro
{
    public static class DBCheckerSqlite
    {
        public static void DoCheck(string connectionString)
        {
            
            var createDb = false;
            
            using (var dbconn = new SqliteConnection(connectionString))
            {
                dbconn.Open();

                //check for pre-created database SELECT name FROM sqlite_master WHERE type='table' AND name='sql_migrations';
                using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='sql_migrations'", dbconn))
                {
                    if (long.Parse(cmd.ExecuteScalar().ToString()) == 0)
                    {
                        //missing TABLES
                        createDb = true;
                    }
                }
                
                dbconn.Close();
            }
            using (var dbconn = new SqliteConnection(connectionString))
            {
                dbconn.Open();
                string sqlText = "";
                var fileName = "./SqliteMigrations/CREATE_DB.sql";
                if (createDb)
                {
                    sqlText = File.ReadAllText(fileName);
                    using (var cmd = new SqliteCommand(sqlText, dbconn))
                    {
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
                foreach (string file in (Directory.EnumerateFiles("./SqliteMigrations", "*.sql")).OrderBy(x => x))
                {
                    if (file != null && !file.Contains("CREATE_DB.sql"))
                    {
                        fileName = file.Replace("\\", "/");
                        if (MissingSqlMigration(dbconn, fileName))
                        {
                            sqlText = File.ReadAllText(file);
                            using (var cmd = new SqliteCommand(sqlText, dbconn))
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

        private static void UpdateSqlMigrationTable(SqliteConnection dbconn, string fileName, string error)
        {
            using (var cmd = new SqliteCommand("DELETE FROM sql_migrations WHERE `FileName`= @fileName", dbconn))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SqliteCommand("INSERT INTO sql_migrations (`FileName`, `Status`, `Error`) VALUES (@fileName, @status, @error)", dbconn))
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

        private static bool MissingSqlMigration(SqliteConnection dbconn, string fileName)
        {
            using (SqliteCommand cmd = new SqliteCommand("SELECT COUNT(*) FROM sql_migrations WHERE FileName=@fileName AND Status = 'Completed'", dbconn))
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
