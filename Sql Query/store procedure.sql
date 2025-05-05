use learnsql

select * from books
select * from employee
select * from newstudent
select * from student

--sytem defined procedure
sp_help
sp_who
sp_tables
sp_databases

--user defined procedure 
create proc empdata
as 
begin
select * from employee
end

alter proc empdata
as
begin 
select * from employee where empid > 10 and empid < 14
select * from employee where empid > 16 and empid < 20
end

empdata
execute empdata
exec empdata

create proc empdetail
@empi int,
@empname varchar(100)
as
begin
select * from employee where empid = 2
select * from employee where empid =3
end

Alter proc empdetail
@empi int,
@empname varchar(100)
as
begin
select * from employee where empid = 2
select * from employee where empid =3
end

alter proc empdetail
@empid int,
@empname varchar(100)
as
begin
select * from employee where empid =  @empid
select * from employee where lastname = @empname
end

empdetail  @empname='king',@empid=14

alter proc empdetail
@empid int=10,
@empname varchar(100)
as
begin
select * from employee where empid =  @empid
select * from employee where lastname = @empname
end
empdetail  @empname='king'

create proc SumofNum
@num1 int,
@num2 int,
@Res int output
as 
begin
	set @Res=@num1+@num2
end

declare @Result Int
exec SumofNum 50,50, @Result output
select @Result

-- Pagination raw a sql
declare @pagenumber int =2
declare @pageSize int=5

select * from books order by bookid offset(@pagenumber-1) * @pageSize rows
fetch next @pageSize rows only