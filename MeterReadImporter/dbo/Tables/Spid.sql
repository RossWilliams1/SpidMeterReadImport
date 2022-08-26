CREATE TABLE [dbo].[Spid] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [SPId]                     VARCHAR (50) NOT NULL,
    [MeterSerial]              VARCHAR (50) NULL,
    [MeterManufacturer]        VARCHAR (50) NULL,
    [MeterSewerageSize]        INT NOT NULL,
    [MeterWaterSize]           INT NOT NULL,
    [YearlyVolumeEstimate]     DECIMAL (16,8) NOT NULL,
    [MeterType]                TINYINT NOT NULL,
    [ReturnToSewer]            INT NULL,
    [GeneralSPId]              VARCHAR (50) NOT NULL,
    [NumberOfReadDigits]       INT NOT NULL,
    [MeterLocationDescription] VARCHAR (300) NOT NULL,
    [MeterReadFrequency]       INT NOT NULL,
    CONSTRAINT [PK_Spid] PRIMARY KEY CLUSTERED ([Id] ASC)
);

