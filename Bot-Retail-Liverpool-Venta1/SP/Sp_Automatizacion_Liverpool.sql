USE [Global]
GO
/****** Object:  StoredProcedure [dbo].[Automatizacion_Liverpool]    Script Date: 01/07/2019 8:14:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Automatizacion_Liverpool] (
	 @Accion VARCHAR(MAX) = ''
	,@Fecha VARCHAR(MAX) = null
	,@Importe VARCHAR(MAX) = null
	,@Unidades VARCHAR(MAX) = null
	)
AS
BEGIN


/* DEVOLUCIONES */

	IF @Accion = 'Devoluciones'
	BEGIN

		DELETE FROM [Z_DV_Liverpool_Bot]
		WHERE 1=1 AND Column1 = 'NroReferen'

		delete from [Z_DV_Liverpool_Bot]
		where 1=1
		and column5 like '%00000000%'
		
		delete from [Z_DV_Liverpool_Bot]
		where 1=1
		and column5 IS NULL
		
		/* INSERTA DATOS ACUMULADOS */
		INSERT INTO Z_DV_Liverpool
		SELECT
			 [Column1]
			,[Column2]
			,CASE 
				WHEN Column3 LIKE '%/%' THEN CONVERT(DATE,REPLACE(SUBSTRING(Column3, LEN(Column3)-3,4) + '-' + SUBSTRING(Column3, CHARINDEX('/',Column3,1)+1,2) + '-' + SUBSTRING(Column3, 1,CHARINDEX('/',Column3,1)),'/',''))
				ELSE CONVERT(DATE,Column3) 
			END AS Column3
			,[Column4]
			,CASE 
				WHEN Column5 LIKE '%/%' THEN CONVERT(DATE,REPLACE(SUBSTRING(Column5, LEN(Column5)-3,5) + '-' + SUBSTRING(Column5, CHARINDEX('/',Column5,1)+1,2) + '-' + SUBSTRING(Column5, 1,CHARINDEX('/',Column5,1)),'/',''))
				WHEN [Column5] LIKE '%/%' THEN CONVERT(DATE,SUBSTRING([Column5], 7,4) + '-' + SUBSTRING([Column5], 4,2) + '-' + SUBSTRING([Column5], 1,2))
				ELSE CONVERT(DATE,[Column5]) 
			END AS [Column5]
			,[Column6]
			,[Column7]
			,[Column8]
			,[Column9]
			,[Column10]
			,[Column11]
			,[Column12]
			,[Column13]
			,[Column14]
			,[Column15]
			,[Column16]
			,[Column17]
			,[Column18]
			,GETDATE()
		FROM [Global].[dbo].[Z_DV_Liverpool_Bot] A
		WHERE NOT EXISTS(SELECT * FROM Z_DV_Liverpool B WHERE A.Column1 = B.NroReferen AND CONVERT(INT,A.Column9) = CONVERT(INT,B.Articulo) AND A.Column6 = B.CentroOrig AND A.Column12 = B.Cantidad AND A.Column18 = B.COD_P)
		and Column3 is not null

		update Z_DV_Liverpool
		set Cantidad = REPLACE(Cantidad,',','.')
		where 1 = 1
		and Cantidad like '%,%'

		TRUNCATE TABLE Z_DV_Liverpool_Bot

	END

--****************************************************************************************************************************************************************
	IF @Accion = 'LimpiarDevoluciones'
	BEGIN
		TRUNCATE TABLE Z_DV_Liverpool_Bot
	END
--****************************************************************************************************************************************************************
	IF @Accion = 'LimpiarVentas'
	BEGIN
		TRUNCATE TABLE Z_VE_Liverpool_Bot
	END

--****************************************************************************************************************************************************************
IF @Accion = 'VentasLiverpool'
	BEGIN

				--Incio de proceso de minado de datos 
		create table #DataOriginalOrder
		(
		id int primary key identity (1,1),
		Column1 varchar(max),
		Column2 varchar(max),
		Column3 varchar(max),
		Column4 varchar(max), 
		Column5 varchar(max),
		Column6 varchar(max),
		Column7 varchar(max),
		Column8 varchar(max),
		Column9 varchar(max),
		Column10 varchar(max),
		Column11 varchar(max),
		Column12 varchar(max),
		Column13 varchar(max)
		)

--******************Agregamos el contendo de bot a #Concentrado******** 
		select * into #Concentrado from Z_VE_Liverpool_Bot
		order by convert(int,Column13) asc
		
--******************Limpiamos la base de datos*************************
	delete from #Concentrado
	where 1=1
	and Column4 is null
	and Column5 is null
	and Column6 is null
	and Column7 is null

	delete from #Concentrado
	where 1=1
	and Column1 like '%BWR%'

	delete from #Concentrado
	where 1=1
	and Column3 is null
	and Column4 is null
	and Column5 is null
	and Column6 is null

	--select * from #Concentrado
	delete from #Concentrado
	where 1=1
	and Column4 like ''
	and Column5 like ''
	and Column6 like ''	
	and Column7 like ''

	--Eliminamos Resultado
	delete from #Concentrado
	where 1=1
	and Column4 like '%Resultado%'
	and Column5 like ''
	and Column6 like ''
	and Column7 like ''

	delete from #Concentrado
	where 1=1
	and Column1 like '%Centro%'

	delete from #Concentrado
	where 1=1
	and Column11 like '0%'

	
	update #Concentrado
	set column11= replace(column11,',','.') 
	where 1=1
	and column11 like '%,%'

	----CONVERTIR NUMEROS
	update #Concentrado
	set column11= replace(replace(left(column11,len(column11)-3),'.',''),',','')+'.'+ replace(replace(right(column11,3),'.',''),',','') 
	where 1=1
	and column11 like '%.%'

	update #Concentrado
	set column11= replace(column11,'"','') 
	where 1=1
	and column11 like '%"%'


	update #Concentrado
	set column11= replace(column11,' ','') 
	where 1=1
	and column11 like '% %'

	--*********************Finalizamos la limpieza de #concentrado**********************
	
	---INGRESAMOS LA DATA DE FORMA ASC
	insert into #DataOriginalOrder (Column1, Column2,Column3,Column4,Column5,Column6,Column7,Column8,Column9,Column10,Column11,Column12,Column13)
	Select Column1, Column2,Column3,Column4,Column5,Column6,Column7,Column8,Column9,Column10,Column11,Column12,Column13 from #Concentrado
	order by convert(int,Column13) asc

	
	--INGRESAMOS EL CENTRO EN LOS ESPACIOS QUE NO CONTIENEN CENTRO Y SUCURSAL
	Declare 
		@Id int = 0,
		@Sucursal varchar(max) = null,
		@lapCounter int = 0,
		@NombreSucursal varchar(max) = null
	while (exists( select top 1 Id, Column1 from #DataOriginalOrder where 1 = 1 and Column1 = '' order by id asc))
	--while (exists( select top 1 Column13, Column1 from #Concentrado where 1 = 1 and Column1 = '' order by Column13 asc))
	begin
		--Identificamos el id que vamos a actualizar
		select top 1 @Id=Id, @Sucursal=Column1,@NombreSucursal=column2 from #DataOriginalOrder where 1 = 1 and Column1 = '' order by id asc
		--select top 1 @Id=Column13, @Sucursal=Column1,@NombreSucursal=column2 from #Concentrado where 1 = 1 and Column1 = '' order by Column13 asc
		
		--print 'Id a actualizar = ' + convert(varchar,@Id)
		--Identificamos el Id que vamos a tomar como referencia para llevar a cabo la actualización
		Select top 1 @Sucursal=Column1,@NombreSucursal=column2 from #DataOriginalOrder where 1 = 1 and Id < @Id and Column1 != '' order by Id desc
		--Select top 1 @Sucursal=Column1,@NombreSucursal=column2 from #Concentrado where 1 = 1 and Column13 < @Id and Column1 != '' order by Column13 desc
		--print 'Sucursal a tomar como base para la actualización' + @Sucursal
	
		update x
			set x.Column1 = @Sucursal, x.Column2 = @NombreSucursal
		from #DataOriginalOrder x
		--from #Concentrado x
		where 1 = 1
			and id = @Id

		select @lapCounter = @lapCounter + 1
		--print 'Lap Counter ' + convert(varchar,@lapCounter)
	end
	
	
		--******************************Agregamos la informacion a otra tabla temporarl*********
		
				
		alter table  #DataOriginalOrder Drop column column4 
		 
		 
	--Proceso de acumulación de Venta
	--drop table #Liverpool1
		SELECT  
			 Column1 Centro
			,Column2 Sucursal
			,Column3 Articulo
			,Column5 Descripcion
			,Column6 Estado
			,Column7 [Modelo/Editor]
			,Column8 [EAN/UPC]
			,Column9 Temporada
			,Convert(int,Column10)  VentasUnidades
			,Convert(float,Convert(money,Column11)) Ventas$
			--,Column11 Ventas$
			,Convert(date,(substring(Column12,7,4) + '-' + substring(Column12,4,2) + '-' + substring(Column12,1,2) )) FechaVenta
			,Convert(int,Column13) COD_P
		into #Liverpool1
		FROM #DataOriginalOrder
		
		--Identificamos que vamos a cambiar

		Select FechaVenta 
		into #Analizar
		from Z_VE_Liverpool
		where 1 = 1
			and FechaVenta in (Select distinct FechaVenta from #Liverpool1)


		--eliminamos la venta que ya tenemos en el bot
		delete a from Z_VE_Liverpool a
		where 1 = 1
			and FechaVenta in (Select distinct FechaVenta from #Analizar)


		--Inserción Final
		Insert into Z_VE_Liverpool
		Select
			 a.Centro
			,a.Sucursal
			,a.Articulo
			,a.Descripcion
			,a.Estado
			,a.[Modelo/Editor]
			,a.[EAN/UPC]
			,a.Temporada
			,a.VentasUnidades
			,a.Ventas$
			,a.FechaVenta
			,a.COD_P
			,GETDATE() FechaSubida
		from #Liverpool1 a
		

	END	
END


