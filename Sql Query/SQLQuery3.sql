

create database MoshCodeDB

use MoshCodeDB

create table moshproduct(
	
	Id int primary key Identity,
	Name Varchar(50),
	Price Decimal,

)
insert into moshproduct values('Apple',3000)

select name ,price, Price*1.5 as new_price from moshproduct

