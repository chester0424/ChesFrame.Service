create table Menu
(
	SysNo int identity(1,1),
	RelationID varchar(30),
	Name nvarchar(50),
	Url varchar(200),
	Target varchar(10),
	Remark nvarchar(100),
	Priority int
)
