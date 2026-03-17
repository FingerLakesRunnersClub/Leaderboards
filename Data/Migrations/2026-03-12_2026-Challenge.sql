INSERT INTO Iterations (ID, SeriesID, Name, StartDate, EndDate)
VALUES (uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2026', '2026-04-10', '2026-09-10')
ON CONFLICT DO NOTHING;

WITH PastRaces AS (SELECT DISTINCT Races.ID AS RaceID
				   FROM Races
							INNER JOIN Courses ON Courses.RaceID = Races.ID AND Courses.IsActive
							INNER JOIN RaceIterations ON Races.ID = RaceIterations.RaceID
							INNER JOIN Iterations AS allI ON RaceIterations.IterationID = allI.ID
							INNER JOIN Series AS AllS ON AllS.ID = allI.SeriesID AND allS.Key = 'Challenge' AND allI.Name IN ('2025', '2024', '2023', '2022', '2021'))
INSERT
INTO RaceIterations (IterationID, RaceID)
	(SELECT DISTINCT Iterations.ID, PastRaces.RaceID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2026'
			  CROSS JOIN PastRaces)
ON CONFLICT DO NOTHING;

INSERT INTO Races (ID, Name, Type, Description)
VALUES (uuidv4(), 'Sights of the Heights', 'Road', 'Take in the sights on the funky small roads in Cayuga Heights!')
ON CONFLICT DO NOTHING;

INSERT INTO Courses (ID, RaceID, IsActive, Distance, Units)
VALUES (uuidv4(), (SELECT ID FROM Races WHERE Name = 'Sights of the Heights'), TRUE, 5.3, 'mi')
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (IterationID, RaceID)
VALUES ((SELECT Iterations.ID
		 FROM Iterations
				  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2026'),
		(SELECT ID FROM Races WHERE Name = 'Sights of the Heights'))
ON CONFLICT DO NOTHING;

WITH NewChallenges AS (SELECT 'FLRC Challenge' AS Name, TRUE AS IsPrimary, NULL AS TimeLimit
					   UNION
					   SELECT 'FLRC 100K Ultra Challenge', FALSE, 24
					   UNION
					   SELECT 'FLRC Tough Trail Challenge', FALSE, 12
					   UNION
					   SELECT 'FLRC Tough Tarmac Challenge', FALSE, 12)
INSERT
INTO Challenges (ID, IterationID, Name, IsOfficial, IsPrimary, TimeLimit)
	(SELECT uuidv4(), Iterations.ID, NewChallenges.Name, TRUE, NewChallenges.IsPrimary, NewChallenges.TimeLimit
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2026'
			  CROSS JOIN NewChallenges)
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID AND Iterations.Name = '2026' AND Challenges.IsOfficial = TRUE AND IsPrimary = TRUE
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge'
			  CROSS JOIN Courses
			  INNER JOIN Races ON Courses.RaceID = Races.ID
		 AND Races.Name IN (
							'East Hill Rec Way',
							'Duck Trails',
							'Waterfront Trail',
							'Beebe Lake',
							'Taughannock Rim & Falls',
							'Sights of the Heights',
							'Treman Trailipop',
							'Black Diamond Cass to Gorge',
							'Ludlowville Loop',
							'Dryden Lake Lollipop'
			 ))
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID AND Iterations.Name = '2026' AND Challenges.Name = 'FLRC Tough Tarmac Challenge'
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge'
			  CROSS JOIN Courses
			  INNER JOIN Races ON Courses.RaceID = Races.ID
		 AND Races.Name IN (
							'East Hill Rec Way',
							'Waterfront Trail',
							'Sights of the Heights',
							'Black Diamond Cass to Gorge',
							'Ludlowville Loop'
			 ))
ON CONFLICT DO NOTHING;

INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID
	 FROM challenges
			  INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID AND Iterations.Name = '2026' AND Challenges.Name = 'FLRC Tough Trail Challenge'
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge'
			  CROSS JOIN Courses
			  INNER JOIN Races ON Courses.RaceID = Races.ID
		 AND Races.Name IN (
							'Duck Trails',
							'Beebe Lake',
							'Taughannock Rim & Falls',
							'Treman Trailipop',
							'Dryden Lake Lollipop'
			 ))
ON CONFLICT DO NOTHING;