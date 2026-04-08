INSERT INTO ChallengeCourses (ChallengeID, CourseID)
	(SELECT Challenges.ID, Courses.ID FROM Challenges
		INNER JOIN Iterations ON Challenges.IterationID = Iterations.ID AND Iterations.Name = '2026' AND Challenges.Name = 'FLRC 100K Ultra Challenge'
		INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge'
	                                  CROSS JOIN Courses INNER JOIN ChallengeCourses CCC ON Courses.ID = CCC.CourseID
	                                  INNER JOIN Challenges CC ON CC.ID = CCC.ChallengeID AND CC.Name LIKE '%Tough%'
	                                  INNER JOIN Iterations CI ON CC.IterationID = CI.ID AND CI.Name = '2026')
ON CONFLICT DO NOTHING;

UPDATE Iterations SET RegistrationType = 'WebScorer', RegistrationContext = '424732'
	WHERE Name = '2026' AND SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge');