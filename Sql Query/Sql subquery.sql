use sql_store
select * from customers
select * from shippers
select * from orders
select * from order_items
select * from products

select  c.customer_id,c.first_name,c.last_name,c.city , sum(o.quantity*o.unit_price)  as spent
from customers c
join order_items oi on c.customer_id=oi.order_id join order_items o on oi.order_id =o.order_id
group by  c.customer_id,c.first_name,c.last_name,c.city

select * from customers
select * from products

select * from products 
where unit_price > (select unit_price from products where product_id=3)

		
use sql_hr
select * from employees
select * from offices

select * from employees 
where  salary > (select avg(salary) from employees)

use sql_store

select * from products
where product_id not in (select distinct product_id from  order_items)

--find the customer who have orderd lettuce id=3 using subquery and joins

select distinct c.customer_id,c.first_name,c.last_name from 
customers c join  orders o on c.customer_id = o.customer_id 
join order_items oi on oi.order_id=o.order_id  where oi.product_id =3

select c.customer_id,c.first_name,c.last_name 
from customers c
where customer_id in (select o.customer_id from orders o join order_items oi 
on o.order_id =oi.order_id where oi.product_id =3
)

