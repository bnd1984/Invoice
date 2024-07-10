using InvoiceSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace InvoiceSystem.Services
{
    public class InvoiceService
    {
        private const string FilePath = "invoices.json";
        private List<Invoice> invoices;
        private int nextId;

        public InvoiceService()
        {
            invoices = LoadInvoices();
            nextId = invoices.Any() ? invoices.Max(i => i.Id) + 1 : 1;
        }

        private List<Invoice> LoadInvoices()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Invoice>();
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Invoice>>(json) ?? new List<Invoice>();
        }

        private void SaveInvoices()
        {
            var json = JsonSerializer.Serialize(invoices);
            File.WriteAllText(FilePath, json);
        }

        public Invoice CreateInvoice(decimal amount, DateTime dueDate)
        {
            var invoice = new Invoice
            {
                Id = nextId++,
                Amount = amount,
                PaidAmount = 0,
                DueDate = dueDate,
                Status = "pending"
            };
            invoices.Add(invoice);
            SaveInvoices();
            return invoice;
        }

        public List<Invoice> GetInvoices()
        {
            return invoices;
        }

        public Invoice GetInvoiceById(int id)
        {
            return invoices.FirstOrDefault(i => i.Id == id);
        }

        public void PayInvoice(int id, decimal amount)
        {
            var invoice = GetInvoiceById(id);
            if (invoice == null) throw new Exception("Invoice not found");

            invoice.PaidAmount += amount;
            if (invoice.PaidAmount >= invoice.Amount)
            {
                invoice.Status = "paid";
            }
            SaveInvoices();
        }

        public void ProcessOverdueInvoices(decimal lateFee, int overdueDays)
        {
            var now = DateTime.Now;
            var overdueInvoices = invoices
                .Where(i => i.Status == "pending" && (now - i.DueDate).TotalDays > overdueDays)
                .ToList();

            foreach (var invoice in overdueInvoices)
            {
                if (invoice.PaidAmount > 0)
                {
                    var remainingAmount = invoice.Amount - invoice.PaidAmount + lateFee;
                    invoice.Status = "paid";
                    CreateInvoice(remainingAmount, now.AddDays(overdueDays));
                }
                else
                {
                    invoice.Status = "void";
                    CreateInvoice(invoice.Amount + lateFee, now.AddDays(overdueDays));
                }
            }
            SaveInvoices();
        }
    }
}
