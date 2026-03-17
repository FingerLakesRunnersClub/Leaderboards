UPDATE Iterations
SET RegistrationType = 'AnyRace'
WHERE ID = (SELECT Iterations.ID
			FROM Iterations
					 INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Iterations.Name = '2026' AND Series.Key = 'TrailCircuit' AND RegistrationType IS NULL);

WITH PastRaces AS (SELECT DISTINCT Races.ID AS RaceID
				   FROM Races
							INNER JOIN Courses ON Courses.RaceID = Races.ID AND Courses.IsActive
							INNER JOIN RaceIterations ON Races.ID = RaceIterations.RaceID
							INNER JOIN Iterations AS allI ON RaceIterations.IterationID = allI.ID
							INNER JOIN Series AS AllS ON AllS.ID = allI.SeriesID AND allS.Key = 'Challenge' AND allI.Name = '2025')
INSERT
INTO RaceIterations (IterationID, RaceID)
	(SELECT DISTINCT Iterations.ID, PastRaces.RaceID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'TrailCircuit' AND Iterations.Name = '2026'
			  CROSS JOIN PastRaces)
ON CONFLICT DO NOTHING;
