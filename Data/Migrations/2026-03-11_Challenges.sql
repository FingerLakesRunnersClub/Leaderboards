ALTER TABLE Challenges
	DROP COLUMN IF EXISTS IsPublic,
	DROP COLUMN IF EXISTS StartDate,
	DROP COLUMN IF EXISTS EndDate,
	ADD COLUMN IF NOT EXISTS IsPrimary BOOLEAN,
	ADD COLUMN IF NOT EXISTS AthleteID UUID REFERENCES Athletes (ID);

UPDATE Challenges
SET IsPrimary = FALSE
WHERE IsPrimary IS NULL
   OR IsPrimary = FALSE;

ALTER TABLE Challenges
	ALTER COLUMN IsPrimary SET NOT NULL;

WITH i AS (SELECT Series.ID AS SeriesID, Iterations.ID AS IterationID, Iterations.Name AS IterationName
		   FROM Iterations
					INNER JOIN Series ON Series.ID = SeriesID AND Series.Key = 'Challenge')
INSERT
INTO Challenges (ID, IterationID, Name, IsOfficial, IsPrimary, TimeLimit)
VALUES (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2025'), 'FLRC Challenge', TRUE, TRUE, NULL),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2025'), 'FLRC 100K Ultra Challenge', TRUE, FALSE, 24),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2025'), 'FLRC Tough Trail Challenge', TRUE, FALSE, 12),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2025'), 'FLRC Tough Tarmac Challenge', TRUE, FALSE, 12),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2024'), 'FLRC Challenge', TRUE, TRUE, NULL),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2024'), 'FLRC 100K Ultra Challenge', TRUE, FALSE, 24),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2024'), 'FLRC Tough Trail Challenge', TRUE, FALSE, 12),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2024'), 'FLRC Tough Tarmac Challenge', TRUE, FALSE, 12),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2023'), 'FLRC Challenge', TRUE, TRUE, NULL),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2023'), 'FLRC 100K Ultra Challenge', TRUE, FALSE, 24),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2022'), 'FLRC Challenge', TRUE, TRUE, NULL),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2022'), 'FLRC 100K Ultra Challenge', TRUE, FALSE, 24),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2021'), 'FLRC Challenge', TRUE, TRUE, NULL),
	   (uuidv4(), (SELECT IterationID FROM i WHERE IterationName = '2021'), 'FLRC 100K Ultra Challenge', TRUE, FALSE, 24)
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM Challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID
			  INNER JOIN RaceIterations ON RaceIterations.IterationID = Iterations.id
			  INNER JOIN Courses ON Courses.RaceID = RaceIterations.RaceID
	 WHERE Challenges.Name = 'FLRC Challenge'
		OR Challenges.Name LIKE '%100K%')
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM Challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID
			  INNER JOIN RaceIterations ON RaceIterations.IterationID = Iterations.id
			  INNER JOIN Races ON RaceIterations.RaceID = Races.ID
			  INNER JOIN Courses ON Courses.RaceID = Races.ID
	 WHERE Challenges.Name LIKE '%Tough Trail%'
	   AND (Races.Type = 'Trail' OR Races.Name = 'FH Fox'))
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM Challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID
			  INNER JOIN RaceIterations ON RaceIterations.IterationID = Iterations.id
			  INNER JOIN Races ON RaceIterations.RaceID = Races.ID
			  INNER JOIN Courses ON Courses.RaceID = Races.ID
	 WHERE Challenges.Name LIKE '%Tough Trail%'
	   AND (Races.Type IN ('Road', 'Mixed', 'Track') AND Races.Name <> 'FH Fox'))
ON CONFLICT DO NOTHING;