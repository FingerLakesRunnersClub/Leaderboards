INSERT INTO Races (ID, Name, Type)
VALUES (uuidv4(), 'Super Frosty Loomis', 'Trail'),
	   (uuidv4(), 'Thom B Trail Runs', 'Trail'),
	   (uuidv4(), 'Lucifer''s Crossing', 'Trail'),
	   (uuidv4(), 'Finger Lakes 50s', 'Trail'),
	   (uuidv4(), 'Tortoise & Hare', 'Trail'),
	   (uuidv4(), 'Forest Frolic', 'Trail'),
	   (uuidv4(), 'Forge the Gorge', 'Trail'),
	   (uuidv4(), 'Monster Marathon', 'Trail'),
	   (uuidv4(), 'Danby Down and Dirty', 'Trail'),

	   (uuidv4(), 'East Hill Rec Way', 'Road'),
	   (uuidv4(), 'Cornell Botanic Gardens', 'Road'),
	   (uuidv4(), 'Waterfront Trail', 'Road'),
	   (uuidv4(), 'Tortoise and Hare', 'Trail'),
	   (uuidv4(), 'Danby Down & Dirty', 'Trail'),
	   (uuidv4(), 'South Hill Rec Way', 'Mixed'),
	   (uuidv4(), 'Thom B. Trail Run', 'Trail'),
	   (uuidv4(), 'Forest Frolic 15K', 'Trail'),
	   (uuidv4(), 'Black Diamond Trail', 'Trail'),
	   (uuidv4(), 'Pseudo Skunk Cabbage', 'Road'),

	   (uuidv4(), 'Sweet 1600', 'Track'),
	   (uuidv4(), 'Lansing Center Trail', 'XC'),
	   (uuidv4(), 'Beebe Lake', 'Mixed'),
	   (uuidv4(), 'Taughannock Rim & Falls', 'Trail'),
	   (uuidv4(), 'Long Loomis', 'Trail'),
	   (uuidv4(), 'Inlet Shore Trail', 'Mixed'),
	   (uuidv4(), 'East Hill Dryden Rail Trail', 'Mixed'),
	   (uuidv4(), 'Jim Schug Trail', 'Mixed'),
	   (uuidv4(), 'Brookton Hill & Dale', 'Road'),
	   (uuidv4(), 'Lick Brook & Treman FLT', 'Trail'),

	   (uuidv4(), 'Ellis Hollow Creek Crossings', 'Trail'),
	   (uuidv4(), 'Lakefront Loops 5K', 'Mixed'),
	   (uuidv4(), 'Lime Hollow', 'Trail'),
	   (uuidv4(), 'Cornell Scenic Circuit', 'Mixed'),
	   (uuidv4(), 'Six Mile Creek', 'Mixed'),
	   (uuidv4(), 'Black Diamond Park to Park', 'Trail'),
	   (uuidv4(), 'Blueberry Patch', 'Trail'),
	   (uuidv4(), 'Ludlowville Loop', 'Road'),
	   (uuidv4(), 'Dryden Lake Lollipop', 'Mixed'),

	   (uuidv4(), 'Duck Trails', 'Trail'),
	   (uuidv4(), 'Lindsay-Parsons', 'Trail'),
	   (uuidv4(), 'FH Fox', 'Mixed'),
	   (uuidv4(), 'Valley Views', 'Road'),
	   (uuidv4(), 'Treman Trailipop', 'Trail'),
	   (uuidv4(), 'Hammond Hill Hoctathon', 'Trail'),
	   (uuidv4(), 'Run Rabbit Run', 'Mixed'),
	   (uuidv4(), 'Freeville Fly-In', 'Road'),

	   (uuidv4(), 'Mulholland Waterfalls', 'Trail'),
	   (uuidv4(), 'Cayuga Cliffs', 'Trail'),
	   (uuidv4(), 'Fall Creek Trails', 'Mixed'),
	   (uuidv4(), 'Town & Gown Up & Down', 'Road'),
	   (uuidv4(), 'Abbott Ascent', 'Trail'),
	   (uuidv4(), 'Black Diamond Cass to Gorge', 'Trail'),
	   (uuidv4(), 'Triple Hump', 'Road'),
	   (uuidv4(), 'North Country Half', 'Trail')
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'TrailCircuit'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('Super Frosty Loomis',
						  'Thom B Trail Runs',
						  'Lucifer''s Crossing',
						  'Finger Lakes 50s',
						  'Tortoise & Hare',
						  'Forest Frolic',
						  'Forge the Gorge',
						  'Monster Marathon',
						  'Danby Down and Dirty'))
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2021'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('East Hill Rec Way',
						  'Cornell Botanic Gardens',
						  'Waterfront Trail',
						  'Tortoise and Hare',
						  'Danby Down & Dirty',
						  'South Hill Rec Way',
						  'Thom B. Trail Run',
						  'Forest Frolic 15K',
						  'Black Diamond Trail',
						  'Pseudo Skunk Cabbage'))
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2022'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('Sweet 1600',
						  'Lansing Center Trail',
						  'Beebe Lake',
						  'Taughannock Rim & Falls',
						  'Long Loomis',
						  'Inlet Shore Trail',
						  'East Hill Dryden Rail Trail',
						  'Jim Schug Trail',
						  'Brookton Hill & Dale',
						  'Lick Brook & Treman FLT'))
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2023'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('Sweet 1600',
						  'Ellis Hollow Creek Crossings',
						  'Lakefront Loops 5K',
						  'Lime Hollow',
						  'Cornell Scenic Circuit',
						  'Six Mile Creek',
						  'Black Diamond Park to Park',
						  'Blueberry Patch',
						  'Ludlowville Loop',
						  'Dryden Lake Lollipop'))
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2024'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('Sweet 1600',
						  'Duck Trails',
						  'Lakefront Loops 5K',
						  'Lindsay-Parsons',
						  'FH Fox',
						  'Valley Views',
						  'Treman Trailipop',
						  'Hammond Hill Hoctathon',
						  'Run Rabbit Run',
						  'Freeville Fly-In'))
ON CONFLICT DO NOTHING;

INSERT INTO RaceIterations (RaceID, IterationID)
	(SELECT Races.ID AS RaceID, Iterations.ID AS IterationID
	 FROM Iterations
			  INNER JOIN Series ON Iterations.SeriesID = Series.ID AND Series.Key = 'Challenge' AND Iterations.Name = '2025'
			  CROSS JOIN Races
	 WHERE Races.Name IN ('Sweet 1600',
						  'Mulholland Waterfalls',
						  'Lakefront Loops 5K',
						  'Cayuga Cliffs',
						  'Fall Creek Trails',
						  'Town & Gown Up & Down',
						  'Abbott Ascent',
						  'Black Diamond Cass to Gorge',
						  'Triple Hump',
						  'North Country Half'))
ON CONFLICT DO NOTHING;