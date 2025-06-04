use company

select * from companydetail
select * from department
select* from employee
select * from empmanager
select * from Manager
select * from project

select E.EmpName, C.location from empmanager E cross join companydetail C 

create procedure Pagination
@pagenumber int,
@pagesize int
as
begin
	select E.EmpName,M.Manager_Name,D.Dept_Name,C.CompanyName,C.location
	from employee E join Manager M 
	on E.Manager_Id= M.Manager_Id
	join department D
	on E.Dept_Id= D.Dept_Id  
	cross join companydetail c order by EmpName 
	offset (@pagenumber-1) *@pagesize rows 
	fetch next  @pagesize rows only
end

Pagination 1,5