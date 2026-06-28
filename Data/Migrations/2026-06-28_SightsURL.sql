INSERT INTO RaceLinks (ID, RaceID, Name, Type, URL)
VALUES (uuidv4(), (SELECT id FROM races WHERE name = 'Sights of the Heights'), 'Course Info', 'Info',
        'https://fingerlakesrunners.org/challenge/sights-heights/')
ON CONFLICT DO NOTHING;