USE [GLOBAL]
GO

/****** Object:  Table [dbo].[Z_VE_Liverpool]    Script Date: 03/05/2019 05:37:58 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Z_VE_Liverpool](
	[Centro] [varchar](5) NULL,
	[Sucursal] [varchar](50) NULL,
	[Articulo] [varchar](25) NULL,
	[Descripcion] [varchar](500) NULL,
	[Estado] [varchar](50) NULL,
	[Modelo] [varchar](500) NULL,
	[EAN/UPC] [varchar](25) NULL,
	[Temporada] [varchar](50) NULL,
	[VentasUnidades] [int] NULL,
	[Ventas$] [float] NULL,
	[FechaVenta] [date] NULL,
	[COD_P] [int] NULL,
	[FechaSubida] [datetime] NULL
) ON [PRIMARY]
GO


