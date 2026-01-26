INSERT INTO Series (ID, Key, Name)
VALUES (uuidv4(), 'Challenge', 'FLRC Challenge'),
	   (uuidv4(), 'TrailCircuit', 'FLRC Trail Circuit'),
	   (uuidv4(), 'Track', 'FLRC Track Bests');

INSERT INTO Settings (SeriesID, Key, Value)
VALUES ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'CourseLabel', 'Course'),
   ((SELECT ID FROM Series WHERE Key = 'TrailCircuit'), 'CourseLabel', 'Race'),
   ((SELECT ID FROM Series WHERE Key = 'Track'), 'CourseLabel', 'Event');

INSERT INTO Features (SeriesID, Key, Value)
VALUES ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'MultiAttempt', true),
   ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'MultiAttemptCompetitions', true),
   ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'SelfTiming', true),
   ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'AgeGroupTeams', true),
   ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'ShowBadges', true),
   ((SELECT ID FROM Series WHERE Key = 'Challenge'), 'CommunityStars', true),
   ((SELECT ID FROM Series WHERE Key = 'TrailCircuit'), 'GenerateAthleteID', true),
   ((SELECT ID FROM Series WHERE Key = 'Track'), 'FileSystemResults', true),
   ((SELECT ID FROM Series WHERE Key = 'Track'), 'GenerateAthleteID', true),
   ((SELECT ID FROM Series WHERE Key = 'Track'), 'MultiAttempt', true),
   ((SELECT ID FROM Series WHERE Key = 'Track'), 'MultiYear', true);

INSERT INTO Iterations (ID, SeriesID, Name, StartDate, EndDate)
VALUES (uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2025', '2025-04-19', '2025-09-01'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2024', '2024-04-20', '2024-09-02'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2023', '2023-04-15', '2023-08-13'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2022', '2022-04-16', '2022-08-14'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'Challenge'), '2021', '2021-02-21', '2021-12-31'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2025', '2025-02-01', '2025-10-31'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2024', '2024-02-01', '2024-10-31'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2023', '2023-02-01', '2023-10-31'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2022', '2022-02-01', '2022-10-31'),
	(uuidv4(), (SELECT ID FROM Series WHERE Key = 'TrailCircuit'), '2021', '2021-02-01', '2021-10-31');