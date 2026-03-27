DELETE
FROM RaceIterations
WHERE IterationID = (SELECT ID FROM Iterations WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'TrailCircuit') AND Name = '2026');

WITH PastRaces AS (SELECT DISTINCT Races.ID AS RaceID
				   FROM Races
							INNER JOIN Courses ON Courses.RaceID = Races.ID AND Courses.IsActive
							INNER JOIN RaceIterations ON Races.ID = RaceIterations.RaceID
							INNER JOIN Iterations AS allI ON RaceIterations.IterationID = allI.ID
							INNER JOIN Series AS AllS ON AllS.ID = allI.SeriesID AND allS.Key = 'TrailCircuit' AND allI.Name = '2025')
INSERT
INTO RaceIterations (IterationID, RaceID)
	(SELECT DISTINCT Iterations.ID, PastRaces.RaceID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'TrailCircuit' AND Iterations.Name = '2026'
			  CROSS JOIN PastRaces)
ON CONFLICT DO NOTHING;
