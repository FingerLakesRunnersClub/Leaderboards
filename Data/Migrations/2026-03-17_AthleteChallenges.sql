CREATE TABLE IF NOT EXISTS AthleteChallenges
(
	AthleteID UUID REFERENCES Athletes (ID),
	ChallengeID UUID REFERENCES Challenges (ID),
	PRIMARY KEY (AthleteID, ChallengeID)
);