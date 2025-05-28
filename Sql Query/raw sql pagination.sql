use EcommerceDb
select * from products

select * from users
select * from orderitems
select * from orders
select * from categories

select c.Name,p.Name from categories c  join products p on c.Id=p.Id 
select c.Name,p.Name from products p left join categories c on c.Id=p.Id


select u.Name as Buyer,p.Name as Item,c.Name as Category,o.OrderDate,q.Quantity,q.price,o.totalprice,o.address 
from orders o join users u on o.UserId=u.Id join orderitems q on q.OrderId=o.Id join 
products p on q.ProductId=p.Id join categories c on p.CategoryID=c.Id
go
create procedure GetDetail
@pagenumber int,
@pagesize int
as 
begin
select u.Name as Buyer,p.Name as Item,c.Name as Category,o.OrderDate,q.Quantity,q.price,o.totalprice,o.address 
from orders o join users u on o.UserId=u.Id join orderitems q on q.OrderId=o.Id join 
products p on q.ProductId=p.Id join categories c on p.CategoryID=c.Id 
order by Quantity offset(@pagenumber-1)*@pagesize rows
fetch next @pagesize rows only;
end
drop procedure GetDetail
GetDetail @pagenumber=1, @pagesize=10