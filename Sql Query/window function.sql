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
sum(sale_amount) over(partition  by department ) as totalsale ,
count(department) over(partition by department) as totolcount
from sales 
)

select * from Getsales