using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using Pitstop.InvoiceService.CommunicationChannels;
using Pitstop.InvoiceService.DataAccess;
using Pitstop.InvoiceService.Events;
using Pitstop.InvoiceService.Model;
using Serilog;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pitstop.InvoiceService
{
    public class InvoiceManager : IHostedService, IMessageHandlerCallback
    {
        private IMessageHandler _messageHandler;
        private IEmailCommunicator _emailCommunicator;
        private InvoiceServiceDBContext _dbContext;
        private InvoiceEventStoreDBContext _dbContextEventStore;

        public InvoiceManager(IMessageHandler messageHandler, IEmailCommunicator emailCommunicator, InvoiceServiceDBContext dbContext, InvoiceEventStoreDBContext dbContextEventStore)
        {
            _messageHandler = messageHandler;
            _emailCommunicator = emailCommunicator;
            _dbContext = dbContext;
            _dbContextEventStore = dbContextEventStore;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Start(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Stop();
            return Task.CompletedTask;
        }

        public async Task<bool> HandleMessageAsync(string messageType, string message)
        {
            try
            {
                JObject messageObject = new JObject();
                if (message != null || message != "")
                {
                    messageObject = MessageSerializer.Deserialize(message);
                }
                
                switch (messageType)
                {
                    case "DayHasPassed":
                        await HandleAsync();
                        break;
                    case "RentalRegistered":
                        await HandleAsync(messageObject.ToObject<RentalRegistered>());
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return true;
        }

        private async Task HandleAsync(RentalRegistered rentalRegistered)
        {
            if ((rentalRegistered.Id != null || rentalRegistered.Id != "") && (rentalRegistered.Name != null || rentalRegistered.Name != ""))
            {
                Renter renterToInsert = new Renter();
                renterToInsert.RenterId = rentalRegistered.Id;
                renterToInsert.Name = rentalRegistered.Name;
                renterToInsert.Address = "Evert van de Beekstraat 202";
                renterToInsert.PostalCode = "1118 CP";
                renterToInsert.City = "Amsterdam";
                renterToInsert.Email = rentalRegistered.Name + "@gmail.com";
                renterToInsert.RentStarted = DateTime.Now;

                await InsertRenter(renterToInsert);
                await InsertRentalRegisteredInEventStore(rentalRegistered);
            }
        }

        private async Task InsertRentalRegisteredInEventStore(RentalRegistered rentalRegistered)
        {
            EventStore eventStore = new EventStore();
            eventStore.Event = "RentalRegistered";
            Log.Information(rentalRegistered.ToString());
            eventStore.EventBody = "Id: " + rentalRegistered.Id + ", Name: " + rentalRegistered.Name;
            eventStore.EventDate = DateTime.Now;
            await _dbContextEventStore.Events.AddAsync(eventStore);
            await _dbContextEventStore.SaveChangesAsync();
        }

        private async Task InsertDayHasPassedInEventStore()
        {
            EventStore eventStore = new EventStore();
            eventStore.Event = "DayHasPassed";
            eventStore.EventDate = DateTime.Now;
            await _dbContextEventStore.Events.AddAsync(eventStore);
            await _dbContextEventStore.SaveChangesAsync();
        }

        private async Task HandleAsync()
        {
            // ophalen renters en hierdoorheen lopen
            // ophalen invoices per renter bestaat er geen dan kijken of de datum van nu 1 maand later is dan aangemelde datum
            // anders de laatste invoice pakken en hier kijken of de datum van nu 1 maand later is dan laatse verstuurde datum

            await InsertDayHasPassedInEventStore();
            var renters = _dbContext.Renters.ToList();
            foreach (var renter in renters)
            {
                var invoices = _dbContext.Invoices.Where(x => x.RenterId.Equals(renter.RenterId)).ToList();
                var dateFrom = DateTime.Now.AddDays(-1);
                var dateTo = DateTime.Now;
                if (invoices.Count() >= 1)
                {
                    if (invoices.Last().InvoiceDate.AddMonths(1) >= dateFrom && invoices.Last().InvoiceDate.AddMonths(1) <= dateTo)
                    {
                        var invoice = CreateInvoice(renter);
                        await SendInvoice(renter, invoice);
                        await InsertInvoice(invoice);
                    }
                }
                else
                {
                    if (renter.RentStarted.AddMonths(1) >= dateFrom && renter.RentStarted <= dateTo)
                    {
                        var invoice = CreateInvoice(renter);
                        await SendInvoice(renter, invoice);
                        await InsertInvoice(invoice);
                    }
                }
            }
        }

        private async Task InsertInvoice(Invoice invoice)
        {
            if (invoice != null)
            {
                await _dbContext.Invoices.AddAsync(invoice);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task InsertRenter(Renter renter)
        {
            if (renter != null)
            {
                await _dbContext.Renters.AddAsync(renter);
                await _dbContext.SaveChangesAsync();
            }
        }
        private Invoice CreateInvoice(Renter renter) {
            DateTime invoiceDate = DateTime.Now;
            Invoice invoice = new Invoice
            {
                InvoiceId = $"{invoiceDate.ToString("yyyyMMddhhmmss")}-{renter.RenterId}",
                InvoiceDate = invoiceDate.Date,
                RenterId = renter.RenterId
            };

            invoice.Amount = 1000;
            return invoice;
        }

        private async Task SendInvoice(Renter renter, Invoice invoice)
        {
            StringBuilder body = new StringBuilder();

            // top banner
            body.AppendLine("<htm><body style='width: 1150px; font-family: Arial;'>");
            body.AppendLine("<image src='cid:banner.jpg'>");

            body.AppendLine("<table style='width: 100%; border: 0px; font-size: 25pt;'><tr>");
            body.AppendLine("<td>Airsupport invoicing Department</td>");
            body.AppendLine("<td style='text-align: right;'>INVOICE</td>");
            body.AppendLine("</tr></table>");

            body.AppendLine("<hr>");

            // invoice and customer details
            body.AppendLine("<table style='width: 100%; border: 0px;'><tr>");

            body.AppendLine("<td width='150px' valign='top'>");
            body.AppendLine("Invoice reference<br/>");
            body.AppendLine("Invoice date<br/>");
            body.AppendLine("Amount<br/>");
            body.AppendLine("Payment due by<br/>");
            body.AppendLine("</td>");

            body.AppendLine("<td valign='top'>");
            body.AppendLine($": {invoice.InvoiceId}<br/>");
            body.AppendLine($": {invoice.InvoiceDate.ToString("dd-MM-yyyy")}<br/>");
            body.AppendLine($": &euro; {invoice.Amount}<br/>");
            body.AppendLine($": {invoice.InvoiceDate.AddDays(30).ToString("dd-MM-yyyy")}<br/>");
            body.AppendLine("</td>");

            body.AppendLine("<td width='50px' valign='top'>");
            body.AppendLine("To:");
            body.AppendLine("</td>");

            body.AppendLine("<td valign='top'>");
            body.AppendLine($"{renter.Name}<br/>");
            body.AppendLine($"{renter.Address}<br/>");
            body.AppendLine($"{renter.PostalCode}<br/>");
            body.AppendLine($"{renter.City}<br/>");
            body.AppendLine("</td>");

            body.AppendLine("</tr></table>");

            body.AppendLine("<hr><br/>");

            // body
            body.AppendLine($"Dear {renter.Name},<br/><br/>");
            body.AppendLine("Hereby we send you an invoice for the rent of last month:<br/>");

            body.AppendLine($"Total amount : &euro; {invoice.Amount}<br/><br/>");

            body.AppendLine("Payment terms : Payment within 30 days of invoice date.<br/><br/>");

            // payment details
            body.AppendLine("Payment details<br/><br/>");

            body.AppendLine("<table style='width: 100%; border: 0px;'><tr>");

            body.AppendLine("<td width='120px' valign='top'>");
            body.AppendLine("Bank<br/>");
            body.AppendLine("Name<br/>");
            body.AppendLine("IBAN<br/>");
            body.AppendLine($"Reference<br/>");
            body.AppendLine("</td>");

            body.AppendLine("<td valign='top'>");
            body.AppendLine(": ING<br/>");
            body.AppendLine(": Airsupport invoicing<br/>");
            body.AppendLine(": NL20INGB0001234567<br/>");
            body.AppendLine($": {invoice.InvoiceId}<br/>");
            body.AppendLine("</td>");

            body.AppendLine("</tr></table><br/>");

            // greetings
            body.AppendLine("Greetings,<br/><br/>");
            body.AppendLine("The Airsupport invoicing crew<br/>");

            body.AppendLine("</htm></body>");

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("invoicing@airsupport.com"),
                Subject = $"Airsupport invoicing Invoice #{invoice.InvoiceId}"
            };
            mailMessage.To.Add("airsupport@prestoprint.nl");

            mailMessage.Body = body.ToString();
            mailMessage.IsBodyHtml = true;

            Attachment bannerImage = new Attachment(@"Assets/banner.jpg");
            string contentID = "banner.jpg";
            bannerImage.ContentId = contentID;
            bannerImage.ContentDisposition.Inline = true;
            bannerImage.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            mailMessage.Attachments.Add(bannerImage);

            await _emailCommunicator.SendEmailAsync(mailMessage);
        }
    }
}
