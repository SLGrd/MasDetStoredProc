using Common.Models;
using MinApi.Interfaces;

namespace MinApi;
public static class ApiExtension
{
    public static void ApiMapping(this WebApplication app)
    {
        app.MapGet("/TestConnection", GetPing);                 //  Test (Ping) connection        
        app.MapGet("/GetAll", GetAll);                          //  Get a list of all records                     
        app.MapGet("/GetRecordById/{iNumber}", GetRecordById);  //  Get the record by supplied number
        app.MapPost("/CreateRecord", CreateRecord);             //  Create a new record
        app.MapPut("/UpdateRecord", UpdateRecord);              //  Update an existing record
        app.MapDelete("DeleteRecord/{iNumber}", DeleteRecord);  //  Delete record by supplied umber
    }
    private static IResult GetPing(IInvoice ic)
    {
        try
            { return Results.Ok(ic.GetPing()); }
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
    private static async Task<IResult> GetAll(IInvoice ic)  // Not Implemented
    {   
        try
            { return Results.Ok(await ic.GetAllInvoices()); } 
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
    private static async Task<IResult> GetRecordById(IInvoice ic, int iNumber)
    {
        try
            { return Results.Ok(await ic.GetInvoiceById(iNumber)); }
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
    private static async Task<IResult> CreateRecord(IInvoice ic, Invoice inv)
    {
        try
            { return Results.Ok(await ic.CreateInvoice(inv)); }
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
    private static async Task<IResult> UpdateRecord(IInvoice ic, Invoice cm)
    { 
        try
            { return Results.Ok(await ic.UpdateInvoice(cm)); }
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
    private static async Task<IResult> DeleteRecord(IInvoice ic, int iNumber)
    {
        try
            { return Results.Ok(await ic.DeleteInvoiceById( iNumber)); }
        catch (Exception ex)
            { return Results.Problem($"Erro : {ex.Message} {DateTime.Now:hh:mm:ss} "); }
    }
}