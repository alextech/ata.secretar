SELECT [allocation].[AllocationId], 
[allocation].[Version], 
[allocation].[Name], 
[allocation.ScoreRange].[AllocationId], 
[allocation.ScoreRange].[Version],
[allocation.ScoreRange].[ScoreRange_Max], 
[allocation.ScoreRange].[ScoreRange_Min]
FROM [Funds].[AllocationVersions] AS [allocation]
LEFT JOIN [Funds].[AllocationVersions] AS [allocation.ScoreRange] ON 
	([allocation].[AllocationId] = [allocation.ScoreRange].[AllocationId]) AND 
	([allocation].[Version] = [allocation.ScoreRange].[Version])
WHERE [allocation].[Version] = @__version_0