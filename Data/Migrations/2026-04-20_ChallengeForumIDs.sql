DELETE
FROM RaceLinks
WHERE Type = 'Community'
  AND RaceID IN (SELECT RaceID FROM RaceIterations WHERE IterationID = (SELECT ID FROM Iterations WHERE Name = '2026' AND SeriesID = (SELECT ID FROM Series WHERE Key = 'Challenge')));

INSERT INTO RaceLinks (ID, Name, Type, URL, RaceID)
VALUES (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9881', (SELECT ID FROM Races WHERE Name = 'Pseudo Skunk Cabbage')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9880', (SELECT ID FROM Races WHERE Name = 'North Country Half')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9879', (SELECT ID FROM Races WHERE Name = 'Lick Brook & Treman FLT')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9878', (SELECT ID FROM Races WHERE Name = 'Freeville Fly-In')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9877', (SELECT ID FROM Races WHERE Name = 'Run Rabbit Run')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9876', (SELECT ID FROM Races WHERE Name = 'Brookton Hill & Dale')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9875', (SELECT ID FROM Races WHERE Name = 'Triple Hump')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9874', (SELECT ID FROM Races WHERE Name = 'Forest Frolic 15K')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9873', (SELECT ID FROM Races WHERE Name = 'Hammond Hill Hoctathon')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9872', (SELECT ID FROM Races WHERE Name = 'Thom B. Trail Run')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9871', (SELECT ID FROM Races WHERE Name = 'Blueberry Patch')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9870', (SELECT ID FROM Races WHERE Name = 'Jim Schug Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9869', (SELECT ID FROM Races WHERE Name = 'Abbott Ascent')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9868', (SELECT ID FROM Races WHERE Name = 'East Hill Dryden Rail Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9867', (SELECT ID FROM Races WHERE Name = 'South Hill Rec Way')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9866', (SELECT ID FROM Races WHERE Name = 'Six Mile Creek')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9865', (SELECT ID FROM Races WHERE Name = 'Valley Views')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9864', (SELECT ID FROM Races WHERE Name = 'Inlet Shore Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9863', (SELECT ID FROM Races WHERE Name = 'Danby Down & Dirty')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9862', (SELECT ID FROM Races WHERE Name = 'Town & Gown Up & Down')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9861', (SELECT ID FROM Races WHERE Name = 'Long Loomis')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9860', (SELECT ID FROM Races WHERE Name = 'Tortoise and Hare')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9859', (SELECT ID FROM Races WHERE Name = 'Fall Creek Trails')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9858', (SELECT ID FROM Races WHERE Name = 'Cornell Scenic Circuit')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9857', (SELECT ID FROM Races WHERE Name = 'FH Fox')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9856', (SELECT ID FROM Races WHERE Name = 'Lime Hollow')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9855', (SELECT ID FROM Races WHERE Name = 'Lindsay-Parsons')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9854', (SELECT ID FROM Races WHERE Name = 'Cayuga Cliffs')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9853', (SELECT ID FROM Races WHERE Name = 'Lansing Center Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9852', (SELECT ID FROM Races WHERE Name = 'Lakefront Loops 5K')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9851', (SELECT ID FROM Races WHERE Name = 'Mulholland Waterfalls')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9850', (SELECT ID FROM Races WHERE Name = 'Cornell Botanic Gardens')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9849', (SELECT ID FROM Races WHERE Name = 'Ellis Hollow Creek Crossings')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9848', (SELECT ID FROM Races WHERE Name = 'Sweet 1600')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9847', (SELECT ID FROM Races WHERE Name = 'Dryden Lake Lollipop')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9846', (SELECT ID FROM Races WHERE Name = 'Ludlowville Loop')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9845', (SELECT ID FROM Races WHERE Name = 'Black Diamond Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9844', (SELECT ID FROM Races WHERE Name = 'Treman Trailipop')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9843', (SELECT ID FROM Races WHERE Name = 'Sights of the Heights')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9842', (SELECT ID FROM Races WHERE Name = 'Taughannock Rim & Falls')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9840', (SELECT ID FROM Races WHERE Name = 'Beebe Lake')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9839', (SELECT ID FROM Races WHERE Name = 'Waterfront Trail')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9838', (SELECT ID FROM Races WHERE Name = 'Duck Trails')),
       (uuidv4(), 'Community Posts', 'Community', 'https://forum.fingerlakesrunners.org/t/9837', (SELECT ID FROM Races WHERE Name = 'East Hill Rec Way'))
ON CONFLICT DO NOTHING;

UPDATE RaceLinks
SET URL = 'https://fingerlakesrunners.org/challenge/pseudo-skunk-cabbage/'
WHERE Type = 'Info'
  AND RaceID = (SELECT ID FROM Races WHERE Name = 'Pseudo Skunk Cabbage');