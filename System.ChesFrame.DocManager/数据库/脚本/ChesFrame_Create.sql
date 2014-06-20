USE [ChesFrame]
GO

/****** Object:  Table [dbo].[Menu]    Script Date: 06/20/2014 15:26:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Menu]') AND type in (N'U'))
DROP TABLE [dbo].[Menu]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 06/20/2014 15:26:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Person]') AND type in (N'U'))
DROP TABLE [dbo].[Person]
GO

USE [ChesFrame]
GO

/****** Object:  Table [dbo].[Menu]    Script Date: 06/20/2014 15:26:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Menu](
	[SysNo] [int] IDENTITY(1,1) NOT NULL,
	[RelationID] [varchar](30) NULL,
	[Name] [nvarchar](50) NULL,
	[Url] [varchar](200) NULL,
	[Target] [varchar](10) NULL,
	[Remark] [nvarchar](100) NULL,
	[Priority] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [ChesFrame]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 06/20/2014 15:26:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Person](
	[SysNo] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Age] [int] NULL,
	[Phone] [varchar](20) NULL,
	[Email] [varchar](50) NULL,
	[Sex] [int] NULL,
	[Remark] [varchar](100) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


