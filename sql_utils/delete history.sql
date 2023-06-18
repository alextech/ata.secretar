truncate table [Funds].[DailyHistory];


select count(1) from [Funds].[DailyHistory];

IF (SCHEMA_ID('FundsHistory') IS NULL)
BEGIN
	EXEC('CREATE SCHEMA [FundsHistory] Authorization [dbo]')
END;

ALTER SCHEMA [FundsHistory]
	TRANSFER [Funds].[DailyHistory]

DROP VIEW IF EXISTS [FundsHistory].[LastSync];


CREATE VIEW [FundsHistory].[LastSync]
AS
SELECT f.FundCode, f.MsUrl, f.MsCode, CASE WHEN h.FundCode IS NULL THEN CAST('1900-01-01' AS Date) ELSE h.Day END AS LastSync
FROM [Funds].[Funds] f
LEFT JOIN (select dh.FundCode, MAX(dh.Day) AS Day FROM [FundsHistory].DailyHistory dh GROUP BY dh.FundCode) h
	ON h.FundCode = f.FundCode

SELECT f.FundCode, f.MsUrl, f.MsCode
FROM [Funds].[Funds] f


SELECT * FROM [FundsHistory].[LastSync]


select * from [FundsHistory].[DailyHistory] h
where h.FundCode = 'cig4241'
order by day desc
