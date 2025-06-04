use sql_invoicing
use invoicing


select * from clients
select * from payment_methods
select * from payments
select * from invoices

select max(invoice_total) as highest,
		min(invoice_total) as Lowest,
		Avg(invoice_total) as Average,
		Sum(invoice_total * 5) as  Total,
		COUNT(invoice_total) as totalCount,
		COUNT(*) as invoice_total
		from invoices where invoice_date > '2019-07-01'


select max(invoice_total) as high from invoices 
union 
select min(invoice_total) as low  from invoices

select  client_id,sum(payment_total) as TotalPayment,sum(invoice_total) as total 
from invoices 
where invoice_date > '2019-07-01'
group by (client_id)
order by total desc


select c.state,c.city,sum(i.invoice_total) 
as total_sales from invoices i join clients c on c.client_id=i.client_id
group by c.state,c.city
order by total_sales desc

-- find the client without invoice
select * from clients 
where client_id not in (select distinct client_id from invoices)

--select invoices larger than all invoices of client

select * from invoices 
where invoice_total > (
select max(invoice_total) 
from invoices 
where client_id = 3
)

select * from invoices 
where invoice_total > All
(select invoice_total from invoices where client_id =3)  

--Select client at least two invoices

select * from invoices
where client_id in(

select client_id from invoices 
group by client_id 
having count(*) >= 2
)

select * from invoices 
where client_id = any(
select client_id from invoices
group by client_id
having count(*) >=2
)

select * from invoices i
where invoice_total >
(select AVG(invoice_total) from invoices 
where client_id =i.client_id
)
