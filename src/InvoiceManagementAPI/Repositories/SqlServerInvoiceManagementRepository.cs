using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using Pitstop.InvoiceManagementAPI.Repositories;
using Dapper;

namespace Pitstop.WorkshopManagementAPI.Repositories
{
    public class SqlServerInvoiceManagementRepository : IInvoiceManagementRepository
    {
        private static readonly JsonSerializerSettings _serializerSettings;
        private static readonly Dictionary<DateTime, string> _store = new Dictionary<DateTime, string>();
        private string _connectionString;

        static SqlServerInvoiceManagementRepository()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter 
            { 
                NamingStrategy = new CamelCaseNamingStrategy() 
            });
        }

        public SqlServerInvoiceManagementRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void EnsureDatabase()
        {
            // init db
            using (SqlConnection conn = new SqlConnection(_connectionString.Replace("WorkshopManagementEventStore", "master")))
            {
                Console.WriteLine("Ensure database exists");
                
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => 
                        { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                    .Execute(() => conn.Open());

                // create database
                string sql = "if DB_ID('WorkshopManagementEventStore') IS NULL CREATE DATABASE WorkshopManagementEventStore;";
                conn.Execute(sql);

                // create tables
                conn.ChangeDatabase("WorkshopManagementEventStore");
                sql = @" 
                    if OBJECT_ID('WorkshopPlanning') IS NULL 
                    CREATE TABLE WorkshopPlanning (
                        [Id] varchar(50) NOT NULL,
                        [CurrentVersion] int NOT NULL,
                    PRIMARY KEY([Id]));
                   
                    if OBJECT_ID('WorkshopPlanningEvent') IS NULL
                    CREATE TABLE WorkshopPlanningEvent (
                        [Id] varchar(50) NOT NULL REFERENCES WorkshopPlanning([Id]),
                        [Version] int NOT NULL,
                        [Timestamp] datetime2(7) NOT NULL,
                        [MessageType] varchar(75) NOT NULL,
                        [EventData] text,
                    PRIMARY KEY([Id], [Version]));";
                conn.Execute(sql);
            }
        }
    }
}
