CREATE TABLE [dbo].[Treinamento]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ModeloId] INT NOT NULL, 
    CONSTRAINT [FK_Treinamento_ToTable] FOREIGN KEY ([ModeloId]) REFERENCES [Modelo]([Id]) 
)
