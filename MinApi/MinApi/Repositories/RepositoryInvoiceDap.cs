using Common.Models;
using Dapper;
using Dapper.Transaction;
using MinApi.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Cg = System.Configuration.ConfigurationManager;

namespace MinApi.Repositories;
public class RepInvoiceDapper : IInvoice
{
    public string GetPing()
    {
        try
        { return $"Pingou (Dapper) : {DateTime.Now:hh:mm:ss}"; }
        catch (Exception ex)
        { return $"Pingou fail (Dapper) : {ex.Message}"; }
    }
    public async Task<IEnumerable<int>> GetAllInvoices()
    {
        throw new NotImplementedException();
    }
    public async Task<Invoice> CreateInvoice(Invoice invoice)
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                using var cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
                cn.Open();

                invoice.Header!.Gid = Guid.NewGuid(); //  Generates unique identifier that will bind invoice Header and its Lines

                string sqlHeader =
                          "DECLARE @NextInvNumber int ; "
                        + "SET @NextInvNumber = NEXT VALUE FOR Hrz.EventCounter; "
                        + "Insert into InvHeader "
                        + "(  InvoiceNumber,    dtInvoice,  Buyer,  Cpf,  Address,  Complto,  Phone,  Gid) "
                        + "output Inserted.InvoiceNumber values "
                        + "( @NextInvNumber, @InvoiceDate, @Buyer, @Cpf, @Address, @Complto, @Phone, @Gid)";                        

                string sqlInvoice =
                          "Insert into InvLines "
                        + "( ItemNumber,  Description,  Qtty,  UnitPrice,  Gid)" + " values "
                        + "(@ItemNumber, @Description, @Qtty, @UnitPrice, @Gid)";


                var InvoiceNumber = await cn.ExecuteScalarAsync(sqlHeader, new
                {
                    invoice.Header!.InvoiceDate,
                    invoice.Header.Buyer,
                    invoice.Header.Cpf,
                    invoice.Header.Address,
                    invoice.Header.Complto,
                    invoice.Header.Phone,
                    invoice.Header.Gid
                });
                invoice.Header.InvoiceNumber = (int)InvoiceNumber;

                int AffectedRows;
                for (int n = 0; n < invoice.Lines!.Count; n++)
                    AffectedRows = await cn.ExecuteAsync(sqlInvoice, new
                    {
                        invoice.Lines[n].ItemNumber,
                        invoice.Lines[n].Description,
                        invoice.Lines[n].Qtty,
                        invoice.Lines[n].UnitPrice,
                        invoice.Header.Gid
                    });

                transaction.Complete();
            }
            catch (Exception) { throw; }  //  Re trhow exception to be treated where we have user interface
        }
        return invoice;
    }
    public async Task<Invoice> GetInvoiceById(int iNumber)
    {
        try
        {
            Invoice inv = new();

            string sqlHeader = "Select * from InvHeader where InvoiceNumber = @INumber";
            using IDbConnection cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
            cn.Open();
            inv.Header = await Task.FromResult(cn.Query<InvoiceHeader>(sqlHeader, new { INumber = iNumber }).First());

            string sqlLines = "Select * from InvLines where Gid = @IGid";
            inv.Lines = await Task.FromResult(cn.Query<InvoiceLine>(sqlLines, new { IGid = inv.Header.Gid }).ToList());

            return inv;
        }
        catch (Exception) { throw; }
    }
    public async Task<string> DeleteInvoiceById(int iNumber)
    {
        try
        {
            string sqlHeader = "Delete from InvHeader where InvoiceNumber = @INUmber";
            IDbConnection cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
            cn.Open();

            var affectedRows = await cn.ExecuteAsync(sqlHeader, new { INumber = iNumber });
            return $"Record {iNumber} deleted !";
        }
        catch (Exception) { throw; }
    }
    public async Task<Invoice> UpdateInvoice(Invoice invoice)
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                using var cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
                cn.Open();

                //  Delete old Invoice record
                string sqlDelete = "Delete from InvHeader where InvoiceNumber = @INUmber";
                var affectedRows = await cn.ExecuteAsync(sqlDelete, new { INumber = invoice.Header!.InvoiceNumber });

                string sqlHeader =
                        "Insert into InvHeader "
                        + "(   InvoiceNumber,    dtInvoice,  Buyer,  Cpf,  Address,  Complto,  Phone,  Gid)"
                        + " output Inserted.InvoiceNumber values "
                        + $"( @InvoiceNumber, @InvoiceDate, @Buyer, @Cpf, @Address, @Complto, @Phone, @Gid)";

                string sqlInvoice =
                          "Insert into InvLines "
                        + "( ItemNumber,  Description,  Qtty,  UnitPrice,  Gid)" + " values "
                        + "(@ItemNumber, @Description, @Qtty, @UnitPrice, @Gid)";

                var InvoiceNumber = await cn.ExecuteScalarAsync(sqlHeader, new
                {
                    invoice.Header.InvoiceNumber,
                    invoice.Header.InvoiceDate,
                    invoice.Header.Buyer,
                    invoice.Header.Cpf,
                    invoice.Header.Address,
                    invoice.Header.Complto,
                    invoice.Header.Phone,
                    invoice.Header.Gid
                });
                invoice.Header.InvoiceNumber = (int)InvoiceNumber;

                int AffectedRows;
                for (int n = 0; n < invoice.Lines!.Count; n++)
                    AffectedRows = await cn.ExecuteAsync(sqlInvoice, new
                    {
                        invoice.Lines[n].ItemNumber,
                        invoice.Lines[n].Description,
                        invoice.Lines[n].Qtty,
                        invoice.Lines[n].UnitPrice,
                        invoice.Header.Gid
                    });

                transaction.Complete();
            }
            catch (Exception ex) { throw ex; }  //  Re trhow exception to be treated where we have user interface
        }
        return invoice;
    }
}

public class RepInvoiceDapperSP : IInvoice
{
    public string GetPing()
    {
        try
        { return $"Pingou (Dapper) : {DateTime.Now:hh:mm:ss}"; }
        catch (Exception ex)
        { return $"Pingou fail (Dapper) : {ex.Message}"; }
    }
    public async Task<IEnumerable<int>> GetAllInvoices()
    {
        throw new NotImplementedException();
    }
    public async Task<Invoice> CreateInvoice(Invoice invoice)
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                using var cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
                cn.Open();

                invoice.Header.Gid = Guid.NewGuid(); //  Generates unique identifier that will bind invoice Header and its Lines

                //  Header recordding
                var p = new DynamicParameters();
                // -- Input paramters
                p.Add("@InvoiceNumber", value: invoice.Header.InvoiceNumber, direction: ParameterDirection.Input);
                p.Add("@InvoiceDate", value: invoice.Header.InvoiceDate, direction: ParameterDirection.Input);
                p.Add("@Buyer", value: invoice.Header.Buyer, direction: ParameterDirection.Input);
                p.Add("@Cpf", value: invoice.Header.Cpf, direction: ParameterDirection.Input);
                p.Add("@Address", value: invoice.Header.Address, direction: ParameterDirection.Input);
                p.Add("@Complto", value: invoice.Header.Complto, direction: ParameterDirection.Input);
                p.Add("@Phone", value: invoice.Header.Phone, direction: ParameterDirection.Input);
                p.Add("@Gid", value: invoice.Header.Gid, direction: ParameterDirection.Input, dbType: DbType.Guid);
                p.Add("@RowId", value: invoice.Header.RowId, direction: ParameterDirection.Input); // Esta linha so serve pra completar a chamada pra a SP
                // -- Output paramters ==> values that we are going to get back from the stored procedure
                p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_identity", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_message", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                var affectedRows = await cn.ExecuteAsync("spSaveInvoiceHeader", param: p, commandType: CommandType.StoredProcedure);
                //  Check for a valid return value
                if (p.Get<int>("return_value") == 1)
                {
                    //  Get Output values and updates invoice data
                    invoice.Header.InvoiceNumber = p.Get<int>("return_code");
                    invoice.Header.RowId = p.Get<int>("return_identity");
                }
                else
                {
                    transaction.Dispose();
                    throw new Exception($"Transaction canceled => {p.Get<int>("return_message")} .");
                }

                foreach ( InvoiceLine line in invoice.Lines)
                {
                    //  Lines recording
                    p = new DynamicParameters();
                    // -- Input paramters
                    p.Add("@ItemNumber", value: line.ItemNumber, direction: ParameterDirection.Input);
                    p.Add("@Description", value: line.Description, direction: ParameterDirection.Input);
                    p.Add("@Qtty", value: line.Qtty, direction: ParameterDirection.Input);
                    p.Add("@UnitPrice", value: line.UnitPrice, direction: ParameterDirection.Input);
                    p.Add("@Gid", value: invoice.Header.Gid, direction: ParameterDirection.Input, dbType: DbType.Guid);
                    p.Add("@RowId", value: line.RowId, direction: ParameterDirection.Input); // Esta linha so serve pra completar a chamada pra a SP
                    // -- Output paramters ==> values that we are going to get back from the stored procedure
                    p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("return_identity", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("return_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    affectedRows = await cn.ExecuteAsync("spSaveInvoiceLine", param: p, commandType: CommandType.StoredProcedure);
                    //  Check for a valid return value
                    if (p.Get<int>("return_value") == 1)
                    {
                        //  Get Output values and updates invoice data                    
                        line.RowId = p.Get<int>("return_identity");
                    }
                    else
                    {
                        transaction.Dispose();
                        throw new Exception($"Transaction canceled => {p.Get<int>("return_message")} .");
                    }
                }
                //  Transaction Commit
                transaction.Complete();
            }
            catch (Exception ex) { throw ex; }  //  Re throw exception to be treated where we have user interface
        }
        return invoice;
    }
    public async Task<Invoice> GetInvoiceById(int iNumber)
    {
        try
        {
            Invoice inv = new();
            using IDbConnection cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
            cn.Open();
            #region GetHeader
            var p = new DynamicParameters();
            p.Add("@InvoiceNumber",  dbType: DbType.Int32, value: iNumber, direction: ParameterDirection.Input);
            //p.Add("@InvoiceNumber", iNumber);
            //p.Add("@InvoiceNumber", iNumber, DbType.Int32, ParameterDirection.Input);

            //  Output parms
            p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
            //p.Add("return_code",, DbType.Int32, ParameterDirection.Output);

            p.Add("return_rowcount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("return_message", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            inv.Header = await Task.FromResult(cn.QueryAsync<InvoiceHeader>("spGetInvoiceHeader", p, commandType: CommandType.StoredProcedure).Result.First());
            //  Get Procedure Outoput values           
            if (p.Get<int>("return_value") != 1)
                throw new Exception( $"Error : {p.Get<int>("return_code")} - {p.Get<string>("return_message")} ");
            #endregion 
            #region GetLines
            p = new DynamicParameters();
            p.Add("@Gid", dbType: DbType.Guid, value: inv.Header.Gid, direction: ParameterDirection.Input);
            //  Outoput parms
            p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("return_rowcount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("return_message", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            inv.Lines = await Task.FromResult(cn.Query<InvoiceLine>("spGetInvoiceLine", p, commandType: CommandType.StoredProcedure).ToList());
            //  Get Procedure Outoput values
            if (p.Get<int>("return_value") != 1)
                throw new Exception($"Error : {p.Get<int>("return_code")} - {p.Get<string>("return_message")} ");
            #endregion
            return inv;
        }
        catch (Exception) { throw; }
    }
    public async Task<string> DeleteInvoiceById(int iNumber)
    {
        try
        {
            IDbConnection cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
            cn.Open();

            var p = new DynamicParameters();
            p.Add("@InvoiceNumber", dbType: DbType.Int32, value: iNumber, direction: ParameterDirection.Input);
            p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("return_rowcount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("return_message", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            var affectedRows = await cn.ExecuteAsync("spDeleteInvoiceHeader", param: p, commandType: CommandType.StoredProcedure);
            //  Get Output values
            var Return_Code = p.Get<int>("return_code");
            int Return_rowcount = p.Get<int>("return_rowcount");
            var Return_message = p.Get<string>("return_message");
            //  Test return value
            if (p.Get<int>("return_value") == 1)
                return $"Record {iNumber} deleted !";
            else
                return Return_message;
        }
        catch (Exception) { throw; }
    }
    public async Task<Invoice> UpdateInvoice(Invoice invoice)
    {
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                using var cn = new SqlConnection(Cg.ConnectionStrings["DBSource"].ConnectionString);
                cn.Open();

                var p = new DynamicParameters();
                p.Add("@InvoiceNumber", dbType: DbType.Int32, direction: ParameterDirection.Input, value: invoice.Header!.InvoiceNumber);
                p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_rowcount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                var affectedRows = await cn.ExecuteAsync("spDeleteInvoiceHeader", param: p, commandType: CommandType.StoredProcedure);
                //  Get Output values
                var Return_Code = p.Get<int>("return_code");
                int Return_rowcount = p.Get<int>("return_rowcount");
                var Return_message = p.Get<string>("return_message");
                //  Test return value
                if (p.Get<int>("return_value") != 1) { transaction.Dispose(); return invoice; };

                //  Header recordding
                p = new DynamicParameters();
                // -- Input paramters
                p.Add("@InvoiceNumber", value: invoice.Header.InvoiceNumber, direction: ParameterDirection.Input);
                p.Add("@InvoiceDate", value: invoice.Header.InvoiceDate, direction: ParameterDirection.Input);
                p.Add("@Buyer", value: invoice.Header.Buyer, direction: ParameterDirection.Input);
                p.Add("@Cpf", value: invoice.Header.Cpf, direction: ParameterDirection.Input);
                p.Add("@Address", value: invoice.Header.Address, direction: ParameterDirection.Input);
                p.Add("@Complto", value: invoice.Header.Complto, direction: ParameterDirection.Input);
                p.Add("@Phone", value: invoice.Header.Phone, direction: ParameterDirection.Input);
                p.Add("@Gid", value: invoice.Header.Gid, direction: ParameterDirection.Input, dbType: DbType.Guid);
                p.Add("@RowId", value: invoice.Header.RowId, direction: ParameterDirection.Input); // Esta linha so serve pra completar a chamada pra a SP
                // -- Output paramters ==> values that we are going to get back from the stored procedure
                p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_identity", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("return_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                affectedRows = await cn.ExecuteAsync("spUpdateInvoiceHeader", param: p, commandType: CommandType.StoredProcedure);
                //  Check for a valid return value
                if (p.Get<int>("return_value") != 1)
                { 
                    transaction.Dispose();
                    throw new Exception($"Transaction canceled => {p.Get<int>("return_message")} .");
                }
                invoice.Header.RowId = p.Get<int>("return_identity");

                foreach (InvoiceLine line in invoice.Lines!)
                {
                    //  Lines recording
                    p = new DynamicParameters();
                    // -- Input paramters
                    p.Add("@ItemNumber", value: line.ItemNumber, direction: ParameterDirection.Input);
                    p.Add("@Description", value: line.Description, direction: ParameterDirection.Input);
                    p.Add("@Qtty", value: line.Qtty, direction: ParameterDirection.Input);
                    p.Add("@UnitPrice", value: line.UnitPrice, direction: ParameterDirection.Input);
                    p.Add("@Gid", value: invoice.Header.Gid, direction: ParameterDirection.Input, dbType: DbType.Guid);
                    p.Add("@RowId", value: line.RowId, direction: ParameterDirection.Input); // Esta linha so serve pra completar a chamada pra a SP
                    // -- Output paramters ==> values that we are going to get back from the stored procedure
                    p.Add("return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("return_identity", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("return_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    p.Add("return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    affectedRows = await cn.ExecuteAsync("spSaveInvoiceLine", param: p, commandType: CommandType.StoredProcedure);
                    //  Check for a valid return value
                    if (p.Get<int>("return_value") != 1)
                    {
                        transaction.Dispose();
                        throw new Exception($"Transaction canceled => {p.Get<int>("return_message")} .");
                    }
                    //  Get Output values and updates invoice data                    
                    line.RowId = p.Get<int>("return_identity");
                }
                //  Transaction Commit
                transaction.Complete();
            }
            catch (Exception ex) { throw ex; }  //  Re throw exception to be treated where we have user interface
        }
        return invoice;
    }
}