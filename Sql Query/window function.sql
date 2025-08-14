use invoicing
select * from sales

--select sum(sale_id) as total 
--from sales


--select employee_name, department ,sum(sale_amount) as totalSale from sales
--group by department,employee_name

--Window Function
--select sum(sale_amount) 
--over() as TotalSales
--from sales

--select 
--employee_name,department ,
--sum(sale_amount) over(partition  by department ) as totalsale ,
--count(department) over(partition by department) as totolcount
--from sales
--with cte_all (client_id, name) as (select client_id, name from invoicing)

with Getsales as 
( 
select 
employee_name,department ,
sum(sale_amount) over(partition  by department) as totalsale ,
count(department) over(partition by department) as totolcount
from sales 
)
select * from Getsales


with GetsalesRank as
(
	select 
	employee_Name,department,
	PERCENT_RANK()over(partition by department order by sale_amount desc) as Ranking,
	Rank() over(partition by department order by sale_amount desc) as Ranking,
	Dense_Rank() over(partition by department order by sale_amount desc) as Ranking,
	Row_number() over(partition by department order by sale_amount desc) as Ranking
	from sales
	
)

select * from GetsalesRank;

-- Create Department table
CREATE TABLE Department (
    department_id INT PRIMARY KEY,
    department_name VARCHAR(100) NOT NULL
);

-- Create Employee table
CREATE TABLE Employee (
    employee_id INT PRIMARY KEY,
    employee_name VARCHAR(100) NOT NULL,
    salary DECIMAL(10,2) NOT NULL,
    department_id INT,
    FOREIGN KEY (department_id) REFERENCES Department(department_id)
);

-- Insert Departments
INSERT INTO Department (department_id, department_name) VALUES
(1, 'HR'),
(2, 'Engineering'),
(3, 'Sales');

-- Insert Employees
INSERT INTO Employee (employee_id, employee_name, salary, department_id) VALUES
(1, 'Alice', 70000, 1),
(2, 'Bob', 65000, 1),
(3, 'Charlie', 72000, 1),
(4, 'David', 95000, 2),
(5, 'Eve', 99000, 2),
(6, 'Frank', 87000, 2),
(7, 'Grace', 91000, 2),
(8, 'Hannah', 60000, 3),
(9, 'Ian', 58000, 3),
(10, 'Jack', 62000, 3),
(11, 'Karen', 64000, 3);




select d.department_name ,sum(e.salary) as salary from Employee e
join Department d on e.department_id =d.department_id
group by d.department_name

with helper as (
select  e.employee_id,e.employee_name, d.department_name,e.salary  
from Employee e
join Department d on e.department_id =d.department_id  )

with Helping as(
select  e.employee_id,e.employee_name, d.department_name, e.salary,
 Rank() over(partition by d.department_name order by  e.salary desc) Ranking
from Employee e
join Department d on e.department_id =d.department_id 
)
select * from Helping where Ranking <=2
select * from Employee order by salary desc


select * from Employee e
where salary  in (
select top 2 salary from Employee e2
	where e.department_id =e2.department_id
	order by salary
)


  select * from customers
  
 select * from orders o 
join customers c on o.customer_id=c.customer_id
where o.order_date between  DATEADD(DAY, -30, GETDATE()) and GETDATE()

update orders set order_date ='2025-07-30' where order_id > 6