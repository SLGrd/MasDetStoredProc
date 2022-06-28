using Common.Models;

namespace MinApi.Interfaces;
public interface IInvoice
{
    string GetPing();
    Task<IEnumerable<int>> GetAllInvoices();
    Task<Invoice> CreateInvoice(Invoice invoice);
    Task<Invoice> GetInvoiceById(int iNumber);
    Task<string> DeleteInvoiceById(int iNUmber);
    Task<Invoice> UpdateInvoice(Invoice invoice);
}