ALTER TABLE Courses
	ADD COLUMN IF NOT EXISTS IsActive BOOLEAN;

UPDATE Courses
SET IsActive = TRUE WHERE IsActive IS NULL OR IsActive = FALSE;

ALTER TABLE Courses
	ALTER COLUMN IsActive SET NOT NULL;

UPDATE Courses
SET IsActive = FALSE
WHERE RaceID IN (SELECT ID FROM Races WHERE Name IN ('Black Diamond Park to Park', 'Black Diamond Trail'));

UPDATE Courses
SET IsActive = FALSE
WHERE RaceID = (SELECT ID FROM Races WHERE Name = 'Forge the Gorge') AND Distance IN (3.5, 7);