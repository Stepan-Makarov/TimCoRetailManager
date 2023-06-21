CREATE PROCEDURE [dbo].[spInventory_GetAll]
	AS
begin
	set nocount on;

	select [i].[ProductId], [p].[ProductName], [i].[Quantity], [i].[PurchasePrice], [i].[PurchaseDate]
	from dbo.Inventory i
	inner join dbo.Product p on i.ProductId = p.Id;
end