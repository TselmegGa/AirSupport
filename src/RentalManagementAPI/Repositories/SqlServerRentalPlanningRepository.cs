using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Dapper;
using Polly;
using AirSupport.Application.RentalManagementAPI.Model;
using Pitstop.Infrastructure.Messaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using AirSupport.Application.RentalManagementAPI.Domain.Entities;

namespace AirSupport.Application.RentalManagementAPI.Repositories
{
    public class SqlServerRentalPlanningRepository : IRentalPlanningRepository
    {
        private static readonly JsonSerializerSettings _serializerSettings;
        private static readonly Dictionary<DateTime, string> _store = new Dictionary<DateTime, string>();
        private string _connectionString;

        static SqlServerRentalPlanningRepository()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter 
            { 
                NamingStrategy = new CamelCaseNamingStrategy() 
            });
        }

        public SqlServerRentalPlanningRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<RentalPlanning> GetRentalPlanningAsync(String location)
        {
            RentalPlanning planning = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // get aggregate
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(5, r => TimeSpan.FromSeconds(5), (ex, ts) => 
                        { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                    .ExecuteAsync(() => conn.OpenAsync());
                
                 var aggregate = await conn
                        .QuerySingleOrDefaultAsync<Aggregate>(
                            "select * from RentalPlanning where Id = @Id", 
                            new { Id = location });
                Console.WriteLine("agg id: " + aggregate.Id);
                if (aggregate == null)
                {
                    return null;
                }

                // get events
                IEnumerable<AggregateEvent> aggregateEvents = await conn
                    .QueryAsync<AggregateEvent>(
                        "select * from RentalPlanningEvent where Id = @Id order by [Version];",
                        new { Id = location });
                Console.WriteLine("aggevents: " + aggregateEvents.AsList());
                List<Event> events = new List<Event>();
                foreach (var aggregateEvent in aggregateEvents)
                {
                    
                    events.Add(DeserializeEventData(aggregateEvent.MessageType, aggregateEvent.EventData));
                }
                planning = new RentalPlanning(location, events);
            }
            return planning;
        }

        public async Task SaveRentalPlanningAsync(string location, int originalVersion, int newVersion, IEnumerable<Event> newEvents)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // update eventstore
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(5, r => TimeSpan.FromSeconds(5), (ex, ts) => 
                        { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                    .ExecuteAsync(() => conn.OpenAsync());

                using (var transaction = conn.BeginTransaction())
                {
                    // store aggregate
                    int affectedRows = 0;
                    var aggregate = await conn
                        .QuerySingleOrDefaultAsync<Aggregate>(
                            "select * from RentalPlanning where Id = @Id", 
                            new { Id = location },
                            transaction);

                    if (aggregate != null)
                    {
                        // update existing aggregate
                        affectedRows = await conn.ExecuteAsync(
                            @"update RentalPlanning
                              set [CurrentVersion] = @NewVersion
                              where [Id] = @Id
                              and [CurrentVersion] = @CurrentVersion;",
                            new { 
                                Id = location, 
                                NewVersion = newVersion,
                                CurrentVersion = originalVersion
                            },
                            transaction);
                    }
                    else
                    {
                        // insert new aggregate
                        affectedRows = await conn.ExecuteAsync(
                            "insert RentalPlanning ([Id], [CurrentVersion]) values (@Id, @CurrentVersion)",
                            new { Id = location, CurrentVersion = newVersion },
                            transaction);
                    }

                    // check concurrency
                    if (affectedRows == 0)
                    {
                        transaction.Rollback();
                        throw new ConcurrencyException();
                    }

                    // store events
                    int eventVersion = originalVersion;
                    foreach (var e in newEvents)
                    {
                        eventVersion++;
                        await conn.ExecuteAsync(
                            @"insert RentalPlanningEvent ([Id], [Version], [Timestamp], [MessageType], [EventData])
                              values (@Id, @NewVersion, @Timestamp, @MessageType,@EventData);",
                            new { 
                                Id = location, 
                                NewVersion = eventVersion,
                                Timestamp = DateTime.Now,
                                MessageType = e.MessageType,
                                EventData = SerializeEventData(e) 
                            }, transaction);
                    }

                    // commit
                    transaction.Commit();
                }
            }
        }

        public void EnsureDatabase()
        {
            // init db
            using (SqlConnection conn = new SqlConnection(_connectionString.Replace("RentalPlanningEventStore", "master")))
            {
                Console.WriteLine("Ensure database exists");
                
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => 
                        { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                    .Execute(() => conn.Open());

                // create database
                string sql = "if DB_ID('RentalPlanningEventStore') IS NULL CREATE DATABASE RentalPlanningEventStore;";
                conn.Execute(sql);

                // create tables
                conn.ChangeDatabase("RentalPlanningEventStore");
                sql = @" 
                    if OBJECT_ID('RentalPlanning') IS NULL 
                    CREATE TABLE RentalPlanning (
                        [Id] varchar(50) NOT NULL,
                        [CurrentVersion] int NOT NULL,
                    PRIMARY KEY([Id]));
                   
                    if OBJECT_ID('RentalPlanningEvent') IS NULL
                    CREATE TABLE RentalPlanningEvent (
                        [Id] varchar(50) NOT NULL REFERENCES RentalPlanning([Id]),
                        [Version] int NOT NULL,
                        [Timestamp] datetime2(7) NOT NULL,
                        [MessageType] varchar(75) NOT NULL,
                        [EventData] text,
                    PRIMARY KEY([Id], [Version]));";
                conn.Execute(sql);
            }
        }

        /// <summary>
        /// Get events for a certain aggregate.
        /// </summary>
        /// <param name="planningId">The id of the planning.</param>
        /// <param name="conn">The SQL connection to use.</param>
        /// <returns></returns>
        private async Task<IEnumerable<Event>> GetAggregateEvents(string planningId, SqlConnection conn)
        {
            IEnumerable<AggregateEvent> aggregateEvents = await conn
                .QueryAsync<AggregateEvent>(
                    "select * from RentalPlanningEvent where Id = @Id order by [Version]",
                    new { Id = planningId });

            List<Event> events = new List<Event>();
            foreach (var aggregateEvent in aggregateEvents)
            {
                events.Add(DeserializeEventData(aggregateEvent.MessageType, aggregateEvent.EventData));
            }
            return events;
        }

        /// <summary>
        /// Serialize event-data to JSON.
        /// </summary>
        /// <param name="eventData">The event-data to serialize.</param>
        private string SerializeEventData(Event eventData)
        {
            return JsonConvert.SerializeObject(eventData, _serializerSettings);
        }

        /// <summary>
        /// Deserialize event-data from JSON.
        /// </summary>
        /// <param name="messageType">The message-type of the event.</param>
        /// <param name="eventData">The event-data JSON to deserialize.</param>
        private Event DeserializeEventData(string messageType, string eventData)
        {
            Type eventType = Type.GetType($"AirSupport.Application.RentalManagementAPI.Events.{messageType}");
            JObject obj = JsonConvert.DeserializeObject<JObject>(eventData, _serializerSettings);
            return obj.ToObject(eventType) as Event;
        }
    }
}
