using Common.Models;

namespace Common.Data;

public class SampleData
{
    private static InvoiceHeader Header = new()
    {
        InvoiceNumber = 1,
        InvoiceDate = new DateTime(2020, 05, 01),
        Buyer       = "Tome de Souza",
        Cpf         = "12345678909",
        Address     = "Salvador",
        Complto     = "1234",
        Phone       = "123-455-789",

        Gid         = Guid.NewGuid(),
        RowId       = 1
    };

    private static  InvoiceLine InvLine11 = new( Header.Gid)
    {
        ItemNumber  = 31461,
        Description = "Description Item 31461",
        Qtty        = 2,
        UnitPrice   = 199.0m,
        
        Gid         = Header.Gid,
        RowId       = 1
    };

    private static InvoiceLine InvLine12 = new( Header.Gid)
    {
        ItemNumber  = 98782,
        Description = "Description Item 98782",
        Qtty        = 100,
        UnitPrice   = 10.0m,
        
        Gid         = Header.Gid,
        RowId       = 2
    };
    public static Invoice CreateInvoice()
    {
        //  Generates and links to new Invoice unique identifier
        Header.Gid    = Guid.NewGuid();
        InvLine11.Gid = Header.Gid;
        InvLine12.Gid = Header.Gid;
        //  Builds Invoice
        List<InvoiceLine> Lines = new()
        {
            InvLine11,
            InvLine12,
        };
       return new( Header, Lines);
    }
}