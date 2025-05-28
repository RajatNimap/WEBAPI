use Product


select * from ListOfProduct
select * from ListOfProduct where Id=5

GO
CREATE PROCEDURE GetProduct

As
Begin
	select * from ListOfProduct
end;

getProduct
Go
CREATE PROCEDURE GETPRODUCTBYID
@ID int
As
Begin
		select * from ListOfProduct where Id=@ID
End;

GetProductBYId @ID=3
GO
create procedure updateproduct
@ID int,
@Name varchar(50),
@Price decimal,
@Orderdate date
As
Begin
 update ListOfProduct Set Pname=@Name,Price=@Price,DateofOrder=@Orderdate  where Id=@ID; 
END;

updateproduct @ID=3 ,@Name="Bluetooth 5 speakeers", @Price=500,@Orderdate='2025-05-28';

go
create procedure InsertData
@Name varchar(50),
@Price decimal,
@Orderdate date
as
begin
	Insert into ListOfProduct (Pname,Price,DateofOrder)values (@Name,@Price,@Orderdate)
end;
go
InsertData @Name='Watch',@Price=200,@Orderdate='2025-05-25';
go

create procedure DeleteData
@ID int
as
begin
	delete from ListOfProduct where Id=@ID;
end

DeleteData @ID=8;


