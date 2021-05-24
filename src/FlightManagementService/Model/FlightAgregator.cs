using AirSupport.Application.FlightManagement.DataAccess;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Model.Domain;
using AirSupport.Application.PassengerManagement.Model.EventModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirSupport.Application.PassengerManagement.Model
{
    public class FlightAgregator
    {
        private static readonly JsonSerializerSettings _serializerSettings;
        private FlightManagementDBContext _context;

        static FlightAgregator()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
        }

        public FlightAgregator(FlightManagementDBContext c)
        {
            _context = c;
        }

        public List<FlightAgregate> BuildAgregate()
        {
            var list = _context.Agregates.OrderByDescending(x => x.TimeStamp);
            List<FlightAgregate> l = list.ToList();
            var first = list.First();
            var events = new string[] { "FlightRegistered", "GateAssigned", "CounterAssigned" }.ToList();
            foreach (FlightEvent e in _context.Events.Where(e => events.Contains(e.MessageType) && e.TimeStamp > first.TimeStamp))
            {
                Event ev = DeserializeEventData(e.MessageType, e.EventData);
                FlightAgregate f = list.FirstOrDefault(y => y.FlightId == e.FlightId);
                if (f == null)
                {
                    f = new FlightAgregate();
                }
                if (f.TimeStamp.Equals(null) || e.TimeStamp > f.TimeStamp)
                {
                    f.TimeStamp = e.TimeStamp;
                }
                switch (e.MessageType)
                {
                    case "FlightRegistered":
                        var reg = (FlightRegistered)ev;
                        f.DepartureTime = reg.DepartureDate;
                        f.Destination = reg.Destination;
                        f.Origin = reg.Origin;
                        f.FlightId = reg.FlightId;
                        break;
                    case "GateAssigned":
                        var ga = (GateAssigned)ev;
                        f.Gate = ga.Gate;
                        f.FlightId = ga.FlightId;
                        break;
                    case "CounterAssigned":
                        var ca = (CounterAssigned)ev;
                        f.Counter = ca.Counter;
                        break;
                }
                _context.Agregates.Update(f);
            }
            _context.SaveChanges();
            return _context.Agregates.ToList();
        }

        public List<FlightAgregate> GetAgregates()
        {
            List<FlightAgregate> flights = _context.Agregates.ToList();
            FlightAgregate first = flights.OrderByDescending(f => f.TimeStamp).First();
            var events = new string[] { "FlightRegistered", "GateAssigned", "CounterAssigned" }.ToList();
            foreach (FlightEvent e in _context.Events.Where(e => events.Contains(e.MessageType) && e.TimeStamp > first.TimeStamp))
            {
                Event ev = DeserializeEventData(e.MessageType, e.EventData);
                FlightAgregate f = flights.FirstOrDefault(y => y.FlightId == e.FlightId);
                int index = flights.IndexOf(f);
                if (f == null)
                {
                    f = new FlightAgregate();
                }
                if (f.TimeStamp.Equals(null) || e.TimeStamp > f.TimeStamp)
                {
                    f.TimeStamp = e.TimeStamp;
                }
                switch (e.MessageType)
                {
                    case "FlightRegistered":
                        var reg = (FlightRegistered)ev;
                        f.DepartureTime = reg.DepartureDate;
                        f.Destination = reg.Destination;
                        f.Origin = reg.Origin;
                        f.FlightId = reg.FlightId;
                        break;
                    case "GateAssigned":
                        var ga = (GateAssigned)ev;
                        f.Gate = ga.Gate;
                        f.FlightId = ga.FlightId;
                        break;
                    case "CounterAssigned":
                        var ca = (CounterAssigned)ev;
                        f.Counter = ca.Counter;
                        break;
                }
                flights[index] = f;
            }
            return flights;
        }

        public FlightAgregate GetAgregateById(int FlightId)
        {
            FlightAgregate f = new FlightAgregate();
            f.FlightId = FlightId;
            DateTime reg = DateTime.MinValue;
            DateTime ga = DateTime.MinValue;
            DateTime ca = DateTime.MinValue;
            var events = new string[] { "FlightRegistered", "GateAssigned", "CounterAssigned" }.ToList();
            foreach (FlightEvent e in _context.Events.Where(e => events.Contains(e.MessageType) && e.FlightId == FlightId))
            {
                Event ev = DeserializeEventData(e.MessageType, e.EventData);
                switch (e.MessageType)
                {
                    case "FlightRegistered":
                        var treg = (FlightRegistered)ev;
                        if (e.TimeStamp > reg)
                        {
                            reg = e.TimeStamp;
                            f.DepartureTime = treg.DepartureDate;
                            f.Destination = treg.Destination;
                            f.Origin = treg.Origin;
                        }
                        
                        break;
                    case "GateAssigned":
                        var tga = (GateAssigned)ev;
                        if (e.TimeStamp > ga)
                        {
                            ga = e.TimeStamp;
                            f.Gate = tga.Gate;
                        }
                        break;
                    case "CounterAssigned":
                        var tca = (CounterAssigned)ev;
                        if (e.TimeStamp > ca)
                        {
                            ca = e.TimeStamp;
                            f.Counter = tca.Counter;
                        }
                        break;
                }
            }
            return f;
        }

        public void AddEvent(Event e, int FlightId)
        {
            FlightEvent ev = new FlightEvent();
            ev.EventData = SerializeEventData(e);
            ev.TimeStamp = DateTime.Now;
            ev.MessageType = e.MessageType;
            ev.Id = e.MessageId;
            ev.FlightId = FlightId;
            _context.Events.Add(ev);
            _context.SaveChanges();
        }

        private Event DeserializeEventData(string messageType, string eventData)
        {
            Type eventType = Type.GetType($"Pitstop.WorkshopManagementAPI.Events.{messageType}");
            JObject obj = JsonConvert.DeserializeObject<JObject>(eventData, _serializerSettings);
            return obj.ToObject(eventType) as Event;
        }

        private string SerializeEventData(Event eventData)
        {
            return JsonConvert.SerializeObject(eventData, _serializerSettings);
        }
    }
}
