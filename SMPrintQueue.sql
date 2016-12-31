CREATE TABLE [dbo].[SMPrintQueue](
	[CompanyID] [int] NOT NULL DEFAULT ((0)),
	[PrintQueue] [varchar](10) NOT NULL,
	[Descr] [nvarchar](100) NULL,
 CONSTRAINT [PK_SMPrintQueue] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[PrintQueue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SMPrintJob](
	[CompanyID] [int] NOT NULL DEFAULT ((0)),
	[JobID] [int] IDENTITY(1,1) NOT NULL,
	[PrintQueue] [varchar](10) NOT NULL,
	[ReportId] [varchar](8) NULL,
	[NoteID] [uniqueidentifier] NULL,
	[tstamp] [timestamp] NOT NULL,
	[CreatedByID] [uniqueidentifier] NOT NULL,
	[CreatedByScreenID] [char](8) NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[LastModifiedByID] [uniqueidentifier] NOT NULL,
	[LastModifiedByScreenID] [char](8) NOT NULL,
	[LastModifiedDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_SMPrintJob] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[JobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SMPrintJobParameter](
	[CompanyID] [int] NOT NULL DEFAULT ((0)),
	[JobID] [int] NOT NULL,
	[ParameterName] [nvarchar](255) NOT NULL,
	[ParameterValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_SMPrintJobParameter] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[JobID] ASC,
	[ParameterName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[SOPickPackShipPrintSetup](
	[CompanyID] [int] NOT NULL DEFAULT ((0)),
	[UserID] [uniqueidentifier] NOT NULL,
	[ShipmentConfirmation] [bit] NOT NULL,
	[ShipmentConfirmationQueue] [varchar](10) NULL,
	[ShipmentLabels] [bit] NOT NULL,
	[ShipmentLabelsQueue] [varchar](10) NULL,
 CONSTRAINT [PK_SOPickPackShipPrintSetup] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
