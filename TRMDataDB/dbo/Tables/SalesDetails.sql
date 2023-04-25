CREATE TABLE [dbo].[SalesDetails]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [SaleId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [PurchasePrice] MONEY NOT NULL, 
    [Tax] MONEY NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_SalesDetails_Sales] FOREIGN KEY (SaleId) REFERENCES  Sales(Id), 
    CONSTRAINT [FK_SalesDetails_Products] FOREIGN KEY (ProductId) REFERENCES Products(Id)
)
