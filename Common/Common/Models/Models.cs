using static Common.Glb;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class InvoiceHeader
    {
        public int InvoiceNumber { get; set; }

        [Range(typeof(DateTime), "12/2/2004", "28/4/2024", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Buyer name must have at least 4 chars")]
        [MaxLength(50, ErrorMessage = "Buyer name must have max 50 chars")]
        public string Buyer { get; set; }

        [Required]
        [MaxLength(14, ErrorMessage = "CPF must have max 14 chars including points and hifen")]
        [ValidChars(ValidChars: "-.0123456789", ErrorMessage = "Only .- and numbers are allowed")]
        [ValidCpf(ErrorMessage = "Invalid CPF format")] // URL video : https://www.youtube.com/watch?v=sti2TJy_Zvw
        public string Cpf { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Address must have at least 4 chars")]
        [MaxLength(40, ErrorMessage = "Address must have max 40 chars")]
        public string Address { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Complemento must have at least 4 chars")]
        [MaxLength(24, ErrorMessage = "Complemento must have max 24 chars")]
        public string Complto { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Phone have at least 4 chars")]
        [MaxLength(24, ErrorMessage = "Phone must have max 24 chars")]
        [ValidChars(ValidChars: "()+- 0123456789", ErrorMessage = "Only ()- or numbers are allowed")]
        public string Phone { get; set; }

        [Required]
        public Guid Gid { get; set; }   // Unique Identifier

        [Editable(false)]
        public int RowId { get; set; }

        public InvoiceHeader()
        {
            InvoiceNumber = 0;
            InvoiceDate = new DateTime(2020, 05, 01);
            Buyer = "";
            Cpf = "";
            Address = "";
            Complto = "";
            Phone = "";
            Gid = new Guid();
            RowId = 0;
        }
        public InvoiceHeader(int invoiceNumber, DateTime invoiceDate, string buyer, string cpf,
                             string address, string complto, string phone, Guid gid, int rowId)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Buyer = buyer;
            Cpf = cpf;
            Address = address;
            Complto = complto;
            Phone = phone;
            Gid = gid;
            RowId = rowId;
        }
    }
    public class InvoiceLine
    {
        [Required]
        public int ItemNumber { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Nome must have at least 4 chars")]
        [MaxLength(40, ErrorMessage = "Nome must have max 40 chars")]
        public string Description { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Please enter qtty between 1 and 100")]
        public int Qtty { get; set; }

        [Required]
        [Range(0.1, 1000.0, ErrorMessage = "Please enter unit price between 0.1 and 1000.0")]
        public decimal UnitPrice { get; set; }

        [Required]
        public Guid Gid { get; set; }

        [Editable(false)]
        public int RowId { get; set; }
        public InvoiceLine()
        {
            ItemNumber = 0;
            Description = "";
            Qtty = 0;
            UnitPrice = 0;
            Gid = new Guid();
            RowId = 0;
        }
        public InvoiceLine(Guid gid)
        {
            ItemNumber = 0;
            Description = "";
            Qtty = 0;
            UnitPrice = 0;
            Gid = gid;
            RowId = 0;
        }
        public InvoiceLine(int itemNumber, string description, int qtty, decimal unitPrice, Guid gid, int rowId)
        {
            ItemNumber = itemNumber;
            Description = description;
            Qtty = qtty;
            UnitPrice = unitPrice;
            Gid = gid;
            RowId = rowId;
        }
    }
    public class Invoice
    {
        public InvoiceHeader? Header { get; set; }
        public List<InvoiceLine>? Lines { get; set; }
        public Invoice(InvoiceHeader invHdr, List<InvoiceLine> invLines)
        {
            Header = invHdr;
            Lines = invLines;
        }
        public Invoice() { }    //  Empty constructor used by API to deserialize Json string
    }
}
//  DataValidators extensions
public class ValidChars : ValidationAttribute
{
    private readonly string valChars;
    public ValidChars(string ValidChars) => valChars = ValidChars;
 
    public override bool IsValid(object? value)
    {
        if (value == null) return false; 

        string w = value.ToString()!;

        for (int i = 0; i < w.Length; i++)                              //  Checks for chars not contained in the permitted chars list
          if (!valChars.Contains(w[i])) return false;  

        return true;                                                    //  No invalid chars have been found
    }
}
public class ValidCpf : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return false;

        string w = value.ToString()!.Replace(".", "").Replace("-", "");  //  Clear points and hifens

        if (!w.All(char.IsDigit)) return false;                         //  Somente numeros ?
        if (w.Length != 11) return false;                               //  Numero de caracteres valido ?

        //--------------------- Primeiro digito de controle ------------------------------------------------------------
        if (w[9] != CalculaDigito(w, 9)) return false;                  //  Primeiro digito de controle valido ?

        //--------------------- Segundo digito de controle --------------------------------------------------------------
        if (w[10] != CalculaDigito(w, 10)) return false;                //  Segundo digito de controle valido ?  

        return true;                                                    //  Verification digits matched                                  
    }
    public static char CalculaDigito(string w, int DigitNumber)
    {
        int Digit = 0;
        for (int i = 0; i < DigitNumber; i++)                           //  Transforma char em binario                                                                         
        { Digit += (w[i] - '0') * (DigitNumber + 1 - i); }              //  w[i] - '0' =  Convert.ToInt32(w[i].ToString()) 

        Digit = (11 - (Digit %= 11)) > 9 ? 0 : (11 - (Digit %= 11));    //  11 - Resto da divisão por 11. Se for maior que 9 ==> digito = 0

        return (char)(Digit + '0');                                     //  Transforma em Ascii : ex 0 em Ascii é 48 binario
    }
}