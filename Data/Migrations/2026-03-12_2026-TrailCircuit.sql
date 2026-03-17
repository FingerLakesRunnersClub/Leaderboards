INSERT INTO Iterations (ID, SeriesID, Name, StartDate, EndDate)
VALUES (uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2026', '2026-02-07', '2026-10-10')
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (IterationID, RaceID)
	(SELECT DISTINCT Iterations.ID, Races.ID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2026'
			  CROSS JOIN Races
			  INNER JOIN Courses ON Courses.RaceID = Races.ID AND Courses.IsActive
			  INNER JOIN RaceIterations ON Races.ID = RaceIterations.RaceID
			  INNER JOIN Iterations AS allI ON RaceIterations.IterationID = allI.ID
			  INNER JOIN Series AS AllS ON AllS.ID = allI.SeriesID AND allS.Key = 'TrailCircuit' AND allI.Name = '2025')
ON CONFLICT DO NOTHING;