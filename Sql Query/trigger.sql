

use basicDb

select * from emp
insert into emp (Name,Role,salary) values ('Vignesh','Dot.net',20000)
delete  emp where id=5
create table TriggerData(
	Id int primary key identity,
	Name varchar(50),
	InsertionDate Datetime default GetDate() 

)
select * from TriggerData
go
Alter trigger oninsertEmp1
on emp after insert
as
begin
	Declare @id int
	select @id=Id from inserted
	insert into Audit_Table (Audit_Info) values ('insertedd data of id '+cast(@id as varchar(50))+' created at '+ cast(GETDATE() as varchar(50)))

end
drop trigger oninsertEmp1
go

Alter trigger afteDeletion
on emp after Delete
as
begin
		declare @id int
		select @id=Id from deleted
		select * from deleted
		insert into Audit_Table (Audit_Info) values ('deleted data of id '+cast(@id as varchar(50))+' Deleted  at '+ cast(GETDATE() as varchar(50)))
end
drop trigger afteDeletion
-- Creating Audit Table

Create Table Audit_Table(
	Audit_Id int primary key Identity,
	Audit_Info varchar(100)
)

select * from Audit_Table
Create trigger OnUpdation
on emp after update
as
begin
		select * from deleted
		select* from inserted
end

update emp set Name='Anirudha' where Id=1;
go

WITH getemp as(
	select * from emp
)
select * from getemp

Alter trigger InsteadofTrigger
on emp instead of insert
as
begin
		
			insert into emp(Name,Role,Salary)
			select i.Name,i.Role,i.Salary from inserted i where i.salary > 25000
		if not Exists(select salary from emp where salary > 25000)
		begin
        RAISERROR('You cannot delete employees with salary > 50000.', 16, 1);
			return
		end
end
delete emp where Id=7

create table #EmpDetail(

	Id int,
	Name varchar(50),	
)
insert into #EmpDetail(Id,Name) values(1,'Rajat')
insert into #EmpDetail(Id,Name) values(2,'Niraj')

select * from #EmpDetail

drop table #EmpDetail

--Transaction
create table student(

	Id int Primary Key identity,
	Name Varchar(100)
)
alter table student
add  balance int

insert into student (Name,balance) values ('rajat',1000)
insert into student (Name,balance) values ('Niraj',1000)
truncate table student
select * from student

begin try
	begin transaction 
		update student set balance =(balance-100) where Id =1
		update student set balance =(balance+100) where Id ='B'
		commit transaction
		print 'Transaction comitted'
end try
begin catch
			rollback transaction
			print 'Transaction is Rolled Back'
end catch;
		