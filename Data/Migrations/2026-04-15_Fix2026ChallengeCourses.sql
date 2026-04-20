DELETE
FROM RaceIterations
WHERE iterationid =
      (SELECT ID FROM Iterations WHERE Name = '2026' and SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge'))
  AND RaceID IN (SELECT RaceID
                 FROM RaceIterations
                 WHERE IterationID IN
                       (SELECT ID FROM Iterations WHERE SeriesID IN (SELECT ID FROM Series WHERE Key = 'TrailCircuit')));