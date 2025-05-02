drop database basicDb

use learnsql

use taskDB

select * from userDetails

select * from books
select * from employee
use EmployeesDb

create database company
use company
create table employee(
	Id  int primary key  identity,
	EmpName varchar(30),
	Salary Float,
	Dept_Id varchar(10),
	Manager_Id int,
);

create table department(
	Dept_Id varchar(10),
	Dept_Name varchar (30)
);

create table Manager(
		Manager_Id int primary key,
		Manager_Name varchar(50),
		dept_id varchar(10)
		
);

create table project(
	project_Id int,
	project_Name varchar(50),
	Team_memid int

);

INSERT INTO department (Dept_Id, Dept_Name) VALUES
('D01', 'HR'),
('D02', 'Finance'),
('D03', 'IT'),
('D04', 'Marketing'),
('D05', 'Sales'),
('D06', 'Logistics'),
('D07', 'Admin'),
('D08', 'Operations'),
('D09', 'Legal'),
('D10', 'Support');

INSERT INTO Manager (Manager_Id, Manager_Name, dept_id) VALUES
(101, 'Alice Johnson', 'D01'),
(102, 'Bob Smith', 'D02'),
(103, 'Carol Lee', 'D03'),
(104, 'David Brown', 'D04'),
(105, 'Emma Davis', 'D05'),
(106, 'Frank Moore', 'D06'),
(107, 'Grace Allen', 'D07'),
(108, 'Henry King', 'D08'),
(109, 'Irene Scott', 'D09'),
(110, 'John Clark', 'D10');

INSERT INTO employee (EmpName, Salary, Dept_Id, Manager_Id) VALUES
('Tom', 55000, 'D01', 101),
('Sara', 62000, 'D02', 102),
('Mike', 75000, 'D03', 103),
('Linda', 68000, 'D04', 104),
('Jake', 50000, 'D05', 105),
('Emma', 47000, 'D06', 106),
('Chris', 53000, 'D07', 107),
('Sophia', 60000, 'D08', 108),
('Daniel', 71000, 'D09', 109),
('Nina', 49000, 'D10', 110);

INSERT INTO project (project_Id, project_Name, Team_memid) VALUES
(201, 'HR System Upgrade', 1),
(202, 'Payroll Automation', 2),
(203, 'Cloud Migration', 3),
(204, 'Rebranding Campaign', 4),
(205, 'Sales CRM Setup', 5),
(206, 'Warehouse System', 6),
(207, 'Office Renovation', 7),
(208, 'Production Dashboard', 8),
(209, 'Compliance Review', 9),
(210, 'Support Ticketing', 10);

create table companydetail(
	Id varchar(10),
	CompanyName varchar(50),
	location varchar(40)
);

	CREATE TABLE empmanager (
		Id INT PRIMARY KEY,
		EmpName VARCHAR(50),
		ManagerId INT,  -- refers to another employee
		FOREIGN KEY (ManagerId) REFERENCES Employee(Id)
	);
	INSERT INTO empmanager(Id, EmpName, ManagerId) VALUES
(1, 'Alice', NULL),         -- Top-level manager
(2, 'Bob', 1),              -- Reports to Alice
(3, 'Charlie', 1),          -- Reports to Alice
(4, 'David', 2),            -- Reports to Bob
(5, 'Eva', 2),              -- Reports to Bob
(6, 'Frank', 3);
Insert Into companydetail values('DO1','Nimap','Lower Parel');
select * from companydetail
select * from department
select * from employee
select * from Manager
select * from project
update employee Set Dept_Id ='D011' where Id > 5
update employee Set Dept_Id ='D06' where Id > 5
update Manager set Manager_Id =201 where Manager_Id=110;
-- inner Join
select e.EmpName,d.Dept_Name From employee e inner Join department d on e.Dept_Id=d.Dept_Id;
-- left Join
select e.EmpName,d.Dept_Name from employee e left Join department d on e.Dept_Id=d.Dept_Id;

--Right Join
select e.EmpName,d.Dept_Name from employee e Right Join department d on e.Dept_Id=d.Dept_Id;


select e.EmpName,d.Dept_Name ,m.Manager_Name,p.project_Name from employee e left Join department d on e.Dept_Id =d.Dept_Id
left join Manager m on m.Manager_Id=e.Manager_Id 
left join project p on p.project_Id=e.Id

-- Full Join
select e.EmpName,d.Dept_Name from employee e full join department d on e.Dept_Id = d.Dept_Id

--Cross Join
select e.EmpName ,d.Dept_Name
from employee e cross join department d

select e.EmpName,d.Dept_Name,c.CompanyName,c.location
from employee e inner join department d on e.Dept_Id = d.Dept_Id
cross join companydetail c

select * from empmanager
--self Join
select e.EmpName as emp ,m.ManagerId as man
from empmanager e left join	empmanager m on e.ManagerId=m.Id;