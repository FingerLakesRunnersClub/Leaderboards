CREATE TABLE IF NOT EXISTS IterationRegistration
(
	IterationID uuid NOT NULL REFERENCES Iterations (ID),
	AthleteID   uuid NOT NULL REFERENCES Athletes (ID),
	PRIMARY KEY (IterationID, AthleteID)
);

ALTER TABLE IF EXISTS Iterations
	ADD COLUMN IF NOT EXISTS RegistrationType    TEXT,
	ADD COLUMN IF NOT EXISTS RegistrationContext TEXT;

UPDATE Iterations
SET RegistrationType = 'AnyRace'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'TrailCircuit');

UPDATE Iterations
SET RegistrationType = 'WebScorer'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge');

UPDATE Iterations
SET RegistrationContext = '386059'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')
  AND Name = '2025';

UPDATE Iterations
SET RegistrationContext = '346590'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')
  AND Name = '2024';

UPDATE Iterations
SET RegistrationContext = '308700'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')
  AND Name = '2023';

UPDATE Iterations
SET RegistrationContext = '271794'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')
  AND Name = '2022';

UPDATE Iterations
SET RegistrationContext = '230581'
WHERE SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')
  AND Name = '2021';