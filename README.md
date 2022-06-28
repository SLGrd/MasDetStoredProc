# MasterDetail

Este é o codigo das stored procedures para ser usado SSMS ou Azure Data Studio

Stored Procedures

1. Seleciona o Header de um pedido pelo numero do pedido

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sylvio Giraldes>
-- Create date: <23/06/2022>
-- Description:	<Get invoice Header record>
-- =============================================
ALTER PROCEDURE [dbo].[spGetInvoiceHeader]
	-- Input Parameters - Invoice Header
	@InvoiceNumber		int					= 0, 
	-- Output Parametrs - Return code and message
	@return_code		int = 0					output,
	@return_rowcount	int = 0					output,
	@return_message		nvarchar(50) = ''		output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT OFF;
	
	--	Get Row and Invoice identifiers values		
	Begin try		
		Select * from InvHeader where InvoiceNumber = @InvoiceNumber  	
            set @return_code = 0;	 
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'OK';
			Return 1;				
	end try
	begin catch
            set @return_code = @@ERROR;
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'Record selection failed.';
			Return -1
	end catch
END
GO

2. Seleciona as linhas do pedido com o mesmo identificador do Header ( Invoice.Header.Gid)

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sylvio Giraldes>
-- Create date: <23/06/2022>
-- Description:	<Get invoice Line record>
-- =============================================
CREATE PROCEDURE [dbo].[spGetInvoiceLine]
	-- Input Parameters - Invoice Header
	@Gid				uniqueidentifier	= null, 
	-- Output Parametrs - Return code and message
	@return_code		int					output,
	@return_rowcount	int					output,
	@return_message		nvarchar(50)		output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT OFF;
	
	--	Get Row and Invoice identifiers values		
	Begin try		
		Select * from InvLines where Gid = @Gid  		 
            set @return_code = 0;	 
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'OK';
			Return 1;				
	end try
	begin catch
            set @return_code = @@ERROR;
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'Record selection failed.';
			Return -1
	end catch
END
GO

3. Delete invoice pelo nuemro do invoice

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ====================================================================
-- Author:		<Sylvio Giraldes
-- Create date: <23/06/2022>
-- Description:	<Dele invoice record - cascade delete to Invoice Lines>
-- ====================================================================
CREATE PROCEDURE [dbo].[spDeleteInvoiceHeader]
	-- Input Parameters - Invoice Header
	@InvoiceNumber		int					= 0, 
	-- Output Parametrs - Return code and message
	@return_code		int = 0				output,
	@return_rowcount	int = 0				output,
	@return_message		nvarchar(50) = ''	output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT OFF;
	
	--	Get Row and Invoice identifiers values		
	Begin try		
		Delete from InvHeader where InvoiceNumber = @InvoiceNumber  	
            set @return_code = 0;	 
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'Record ' + STR( @InvoiceNumber) + ' DELETED.';
			Return 1;				
	end try
	begin catch
            set @return_code = @@ERROR;
			set @return_rowcount = @@ROWCOUNT;
            set @return_message = 'Record delete : ' + STR( @InvoiceNumber) + ' failed.';
			Return -1
	end catch
END
GO

4. Save invoice header 

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sylvio Giraldes>
-- Create date: <23/06/2022>
-- Description:	<Save invoice Header record>
-- =============================================
CREATE PROCEDURE [dbo].[spSaveInvoiceHeader]
	-- Input Parameters - Invoice Header
	@InvoiceNumber		int					= 0, 
	@InvoiceDate		smalldatetime		= null,
	@Buyer				nvarchar(50)		= null,
		-- Address data
	@Cpf				nchar(11)			= null,
	@Address			nvarchar(40)		= null,
	@Complto			nvarchar(24)		= null,	
	@Phone				nvarchar(24)		= null,
		-- Invoice Unique Identifier
	@Gid				uniqueidentifier	= null,
		-- Row unique identifier
	@RowId				int					= 0,
	-- Output Parametrs - Return code and message
	@return_code		int 		= 0		output,	
	@return_identity	int			= 0		output,
	@return_message		nvarchar(50)		output

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT OFF;	
	--	Set Row and Invoice identifiers values
	SET @RowId = 0;
	Declare @NextInvNumber int;
	-- Get seuqence counter next value (See Programmability => Sequences)
	SET		@NextInvNumber = NEXT VALUE FOR Hrz.EventCounter; 

	-- Insert record		
	Begin try		
		INSERT into InvHeader
			(    InvoiceNumber,  InvoiceDate,   Buyer,  Cpf,  Address,  Complto,  Phone,  Gid)
			values
			(	@NextInvNumber, @InvoiceDate,  @Buyer, @Cpf, @Address, @Complto, @Phone, @Gid) 
			-- If insert execution succeeds
			set @return_code     = @NextInvNumber
			set @return_identity = @@Identity							
			set @return_message  = 'Registro ID :  incluido OK'
			Return 1				
	End try
		begin catch
			set @return_code     = ERROR_NUMBER()
		 	set @return_identity = 0           			
			set @return_message  = ERROR_MESSAGE()  
			Return -1
		end catch	
END
GO

5. Save invoice line

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sylvio Giraldes
-- Create date: <23/06/2022>
-- Description:	<Save invoice Header record>
-- =============================================
CREATE PROCEDURE [dbo].[spSaveInvoiceLine]
	-- Input Parameters - Invoice Line	
	@ItemNumber			int					= 0, 
	@Description		nvarchar(40)		= null,
	@Qtty				int					= 0,
	@UnitPrice			money				= 0,
		-- Invoice Unique Identifier
	@Gid				uniqueidentifier	= null,
		-- Row unique identifier
	@RowId				int					= 0,
	-- Output Parametrs - Return code and message
	@return_code		int					output,
	@return_identity	int					output,
	@return_message		nvarchar(50)		output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT OFF;	
	--	Set Row and Invoice identifiers values
	SET @RowId = 0;

	-- Insert record		
	Begin try		
		INSERT into InvLines
			(  ItemNumber,  Description,  Qtty,  UnitPrice,  Gid)
			values
			( @ItemNumber, @Description, @Qtty, @UnitPrice, @Gid) 
			-- If insert execution succeeds
			set @return_identity = @@Identity					
			set @return_code     = 0 
			set @return_message  = 'Registro ID incluido OK'
			Return 1				
	End try
		begin catch
			set @return_identity = @@Identity
			set @return_code     = ERROR_NUMBER()
			set @return_message  = ERROR_MESSAGE()  
			Return -1
		end catch	
END
GO