﻿CREATE PROCEDURE [dbo].[spUser_LookUp]
	@Id nvarchar(128)
AS
begin
	set nocount on;

	select Id, FirstName, LastName, EmailAddress, CreateDate
	from dbo.[User]
	where Id = @Id;
end
