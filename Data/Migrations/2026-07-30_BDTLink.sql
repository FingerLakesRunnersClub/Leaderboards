UPDATE RaceLinks
SET RaceID = (SELECT ID FROM Races WHERE Name = 'Black Diamond 10-Mile')
WHERE URL LIKE '%cass-to-gorge%';