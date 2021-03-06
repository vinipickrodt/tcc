USE [sasi]
GO
/****** Object:  Table [dbo].[DadosTreinamento]    Script Date: 12/07/2022 12:23:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DadosTreinamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Dados] [text] NOT NULL,
	[ParametrosModeloId] [int] NOT NULL,
 CONSTRAINT [PK_DadosTreinamento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Modelo]    Script Date: 12/07/2022 12:23:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Modelo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[TipoModelo] [int] NOT NULL,
	[FrequenciaTreinamentos] [varchar](50) NULL,
	[DataUltimoTreinamento] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[StatusTreinamento] [int] NOT NULL,
 CONSTRAINT [PK_Modelo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModeloTreinado]    Script Date: 12/07/2022 12:23:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModeloTreinado](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Dados] [varbinary](max) NOT NULL,
	[ParametrosModeloId] [int] NOT NULL,
 CONSTRAINT [PK_ModeloTreinado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParametrosModelo]    Script Date: 12/07/2022 12:23:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParametrosModelo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CamposEntrada] [text] NOT NULL,
	[CamposSaida] [text] NOT NULL,
	[ModeloId] [int] NOT NULL,
	[Data] [timestamp] NOT NULL,
 CONSTRAINT [PK_ParametrosModelo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Treinamento]    Script Date: 12/07/2022 12:23:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Treinamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModeloId] [int] NOT NULL,
	[Acuracia] [float] NOT NULL,
	[Duracao] [int] NOT NULL,
	[Data] [datetime] NOT NULL,
 CONSTRAINT [PK_Treinamento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DadosTreinamento]  WITH CHECK ADD  CONSTRAINT [FK_DadosTreinamento_ParametrosModelo] FOREIGN KEY([ParametrosModeloId])
REFERENCES [dbo].[ParametrosModelo] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DadosTreinamento] CHECK CONSTRAINT [FK_DadosTreinamento_ParametrosModelo]
GO
ALTER TABLE [dbo].[ModeloTreinado]  WITH CHECK ADD  CONSTRAINT [FK_ModeloTreinado_ParametrosModelo] FOREIGN KEY([ParametrosModeloId])
REFERENCES [dbo].[ParametrosModelo] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ModeloTreinado] CHECK CONSTRAINT [FK_ModeloTreinado_ParametrosModelo]
GO
ALTER TABLE [dbo].[ParametrosModelo]  WITH CHECK ADD  CONSTRAINT [FK_ParametrosModelo_Modelo] FOREIGN KEY([ModeloId])
REFERENCES [dbo].[Modelo] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParametrosModelo] CHECK CONSTRAINT [FK_ParametrosModelo_Modelo]
GO
ALTER TABLE [dbo].[Treinamento]  WITH CHECK ADD  CONSTRAINT [FK_Treinamento_Modelo] FOREIGN KEY([ModeloId])
REFERENCES [dbo].[Modelo] ([Id])
GO
ALTER TABLE [dbo].[Treinamento] CHECK CONSTRAINT [FK_Treinamento_Modelo]
GO
