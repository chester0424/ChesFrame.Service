--Create Table Person

create table Person
(
	SysNo int identity(1,1),
	Name nvarchar(100) not null ,
	Age int ,
	Phone varchar(20),
	Email varchar(50),
	Sex int,
	Remark varchar(100)
)

-- Add Data
insert into Person
(	
	Name,
	Age,
	Phone,
	Email,
	Sex,
	Remark
)
values(
	'������',
	15,
	'15236595458',
	'373934650@qq.com',
	1,
	'���Ա���'
)

insert into Person
(	
	Name,
	Age,
	Phone,
	Email,
	Sex,
	Remark
)
values(
	'ʢ����',
	18,
	'13652526548',
	'945632157@qq.com',
	1,
	'�����Ϻ�'
)

--Select
select * from Person

SELECT * FROM Person
WHERE SysNo = @SysNo
