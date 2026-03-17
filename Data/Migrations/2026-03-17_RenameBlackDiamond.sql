UPDATE Races
SET Name = 'Black Diamond 10-Mile'
WHERE Name = 'Black Diamond Trail'
  AND (SELECT ID FROM Races WHERE Name = 'Black Diamond 10-Mile') IS NULL;

UPDATE Races
SET Name = 'Black Diamond Trail'
WHERE Name = 'Black Diamond Cass to Gorge'
  AND (SELECT ID FROM Races WHERE Name = 'Black Diamond Trail') IS NULL;