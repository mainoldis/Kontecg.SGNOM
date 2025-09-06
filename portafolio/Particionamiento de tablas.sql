--CREATE PARTITION FUNCTION PartitionByYear (datetime2(7)) AS RANGE LEFT FOR VALUES ('2024-01-01', '2025-01-01', '2026-01-01', '2027-01-01', '2028-01-01', '2029-01-01', '2030-01-01');

--CREATE PARTITION SCHEME ByYear AS PARTITION PartitionByYear ALL TO ('PRIMARY');


CREATE TABLE [sgnom].[sal_incidents](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[EmploymentId] [bigint] NOT NULL,
	[Key] [nvarchar](5) NOT NULL,
	[Start] [datetime2](7) NOT NULL,
	[End] [datetime2](7) NOT NULL,
	[Hours] [decimal](28, 24) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Tight] [bit] NOT NULL,
	[Annotation] [nvarchar](500) NULL,
	[CreationTime] [datetime2](7) NOT NULL,
	[CreatorUserId] [bigint] NULL,
	[LastModificationTime] [datetime2](7) NULL,
	[LastModifierUserId] [bigint] NULL,
 CONSTRAINT [PK_sal_incidents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Start] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON ByYear ([Start])
) ON ByYear ([Start])

--ALTER TABLE sgnom.[sal_incidents] SWITCH PARTITION 1 TO [sal_incidents_2024] PARTITION 1;