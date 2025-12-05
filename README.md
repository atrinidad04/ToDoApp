Ejecutar Script SQL para crear la db, sp, function e insertar valores mock antes de correr la app:

-- 1. CREAR BASE DE DATOS
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TodoAppDB')
BEGIN
    ALTER DATABASE TodoAppDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TodoAppDB;
END
GO

CREATE DATABASE TodoAppDB;
GO

USE TodoAppDB;
GO

-- 2. CREAR TABLA Tasks
CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CompletedAt DATETIME NULL
);
GO

-- 3. INSERTAR DATOS MOCK
INSERT INTO Tasks (Title, Description, IsCompleted, CreatedAt, CompletedAt)
VALUES
('Título de tarea #1', 'Descripción #1', 0, GETDATE(), NULL),
('Título de tarea #2', 'Descripción #2', 0, GETDATE(), NULL),
('Título de tarea #3', 'Descripción #3', 0, GETDATE(), NULL),
('Título de tarea #4', 'Descripción #4', 0, GETDATE(), NULL);
GO

-- 4.1 SP - Obtener todas las tareas
CREATE PROCEDURE sp_GetAllTasks
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Title,
        Description,
        IsCompleted,
        CreatedAt,
        CompletedAt,
        dbo.fn_FormatDateToLongFormat(CreatedAt) AS CreatedAtFormatted,
        dbo.fn_FormatDateToLongFormat(CompletedAt) AS CompletedAtFormatted
    FROM Tasks
    ORDER BY 
        IsCompleted ASC,
        CreatedAt DESC;
END
GO

-- 4.2 SP - Obtener tarea por ID
CREATE PROCEDURE sp_GetTaskById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Title,
        Description,
        IsCompleted,
        CreatedAt,
        CompletedAt
    FROM Tasks
    WHERE Id = @Id;
END
GO

-- 4.3 SP - Insertar nueva tarea
CREATE PROCEDURE sp_InsertTask
    @Title NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @NewTaskId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Tasks (Title, Description, IsCompleted, CreatedAt)
    VALUES (@Title, @Description, 0, GETDATE());
    
    SET @NewTaskId = SCOPE_IDENTITY();
    
    SELECT 
        Id,
        Title,
        Description,
        IsCompleted,
        CreatedAt,
        CompletedAt
    FROM Tasks
    WHERE Id = @NewTaskId;
END
GO

-- 4.4 SP - Actualizar tarea
CREATE PROCEDURE sp_UpdateTask
    @Id INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Tasks
    SET 
        Title = @Title,
        Description = @Description
    WHERE Id = @Id;
    
    IF @@ROWCOUNT > 0
    BEGIN
        SELECT 
            Id,
            Title,
            Description,
            IsCompleted,
            CreatedAt,
            CompletedAt
        FROM Tasks
        WHERE Id = @Id;
    END
END
GO

-- 4.5 SP - Marcar tarea como completada/pendiente
CREATE PROCEDURE sp_ToggleTaskCompletion
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Tasks
    SET 
        IsCompleted = CASE WHEN IsCompleted = 1 THEN 0 ELSE 1 END,
        CompletedAt = CASE 
            WHEN IsCompleted = 0 THEN GETDATE() 
            ELSE NULL 
        END
    WHERE Id = @Id;
    
    SELECT 
        Id,
        Title,
        Description,
        IsCompleted,
        CreatedAt,
        CompletedAt
    FROM Tasks
    WHERE Id = @Id;
END
GO

-- 4.6 SP - Eliminar tarea
CREATE PROCEDURE sp_DeleteTask
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM Tasks
    WHERE Id = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 5.1 Función - Contar tareas pendientes
CREATE FUNCTION fn_GetPendingTasksCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    
    SELECT @Count = COUNT(*)
    FROM Tasks
    WHERE IsCompleted = 0;
    
    RETURN @Count;
END
GO

-- 5.2 Función - Contar tareas completadas
CREATE FUNCTION fn_GetCompletedTasksCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    
    SELECT @Count = COUNT(*)
    FROM Tasks
    WHERE IsCompleted = 1;
    
    RETURN @Count;
END
GO

-- 5.3 Función - Formatear fecha a formato largo
CREATE FUNCTION fn_FormatDateToLongFormat
(
    @Date DATETIME
)
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @FormattedDate NVARCHAR(100);
    
    IF @Date IS NULL
    BEGIN
        RETURN NULL;
    END
    
    DECLARE @Day INT = DAY(@Date);
    DECLARE @Month INT = MONTH(@Date);
    DECLARE @Year INT = YEAR(@Date);
    DECLARE @MonthName NVARCHAR(20);
    
    -- Obtener el nombre del mes en español
    SET @MonthName = CASE @Month
        WHEN 1 THEN 'enero'
        WHEN 2 THEN 'febrero'
        WHEN 3 THEN 'marzo'
        WHEN 4 THEN 'abril'
        WHEN 5 THEN 'mayo'
        WHEN 6 THEN 'junio'
        WHEN 7 THEN 'julio'
        WHEN 8 THEN 'agosto'
        WHEN 9 THEN 'septiembre'
        WHEN 10 THEN 'octubre'
        WHEN 11 THEN 'noviembre'
        WHEN 12 THEN 'diciembre'
    END;
    
    -- Formato: "15 de enero de 2026"
    SET @FormattedDate = CAST(@Day AS NVARCHAR(2)) + ' de ' + @MonthName + ' de ' + CAST(@Year AS NVARCHAR(4));
    
    RETURN @FormattedDate;
END
GO

