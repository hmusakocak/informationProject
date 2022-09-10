# What does this app?
This app gets important system informations automatically.

## About database if you want to keep records
If you want to use your local or other type of databases, you need to create a table which one is named sys_info and contains column names which these in program code.
The table creation code is below;

CREATE TABLE [dbo].[sys_info](
	[serial_number] [varchar](300) NOT NULL,
	[date] [varchar](300) NULL,
	[model] [varchar](300) NOT NULL,
	[cpu] [varchar](300) NULL,
	[ram] [varchar](300) NULL,
	[os] [varchar](300) NULL,
	[domain] [varchar](300) NULL,
	[hostname] [varchar](300) NULL,
	[officeversion] [varchar](300) NULL,
	[storage] [varchar](300) NULL,
	[freespace] [varchar](300) NULL,
	[gpu1] [varchar](300) NULL,
	[gpu2] [varchar](300) NULL,
	[note] [varchar](500) NULL,
	[manufacturer] [nchar](300) NULL
)
