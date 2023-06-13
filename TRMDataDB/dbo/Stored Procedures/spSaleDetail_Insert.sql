CREATE PROCEDURE [dbo].[spSaleDetail_Insert]
	@SaleId int,
	@ProductId int,
	@Quantity int,
	@PurchasePrice MONEY,
	@Tax MONEY
AS
begin
	set nocount on;

	insert into dbo.SaleDetail(SaleId, ProductId, Quantity, PurchasePrice, Tax)
	values (@SaleId, @ProductId, @Quantity, @PurchasePrice, @Tax);
end
