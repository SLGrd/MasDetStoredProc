namespace Common;
public static class Glb
{
    //  Data access related variables
    //public static string CnnString = @"Data Source=CINZA\sqlexpress;Initial Catalog = Hrz; Integrated Security = True";

    public static string ApiBaseAddress = "";

    public static readonly string apiKey = @"apiKey=5c2cfa5a9fe6395d6ea0";

    public enum Days { Sun, Mon, tue, Wed, thu, Fri, Sat };
    public enum FamStatus { Marido, Esposa, Filho, Neto };
    public enum Departments { Fin, Com, Eng }
}

public static class ExtensionMethods
{
    public static string ToCpf( this string cpf) 
    {
        string s = $"{cpf[0..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..11]}";
        return s;
    }
}