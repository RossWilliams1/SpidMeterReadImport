CREATE TABLE [dbo].[SPIDMeterRead] (
    [Id]              INT          IDENTITY (1, 1) NOT NULL,
    [SpidId]          INT          NOT NULL,
    [ReadType]        VARCHAR (5)  NOT NULL,
    [ReadingDate]     DATE         NOT NULL,
    [Reading]         INT          NOT NULL,
    [UsedForEstimate] BIT          NOT NULL,
    [ManualReading]   BIT          NOT NULL,
    [Rollover]        BIT          NOT NULL,
    CONSTRAINT [PK_SpidMeterRead] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SpidMeterRead_Spid] FOREIGN KEY ([SpidId]) REFERENCES [dbo].[Spid] ([Id]) 
);

