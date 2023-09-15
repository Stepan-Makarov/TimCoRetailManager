CREATE PROCEDURE [dbo].[spUser_Insert]
	@Id NVARCHAR(128),
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@EmailAddress NVARCHAR(256)
AS
begin
	set nocount on;

	insert into dbo.[User](Id, FirstName, LastName, EmailAddress)
	values (@Id, @FirstName, @LastName, @EmailAddress);

end

