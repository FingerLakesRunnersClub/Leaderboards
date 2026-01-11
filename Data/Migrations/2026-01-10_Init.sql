CREATE TABLE Series
(
	ID   UUID NOT NULL PRIMARY KEY,
	Key  TEXT NOT NULL UNIQUE,
	Name TEXT NOT NULL UNIQUE
);

CREATE TABLE Settings
(
	SeriesID UUID NOT NULL REFERENCES Series (ID),
	Key      TEXT NOT NULL,
	Value    TEXT NOT NULL,
	PRIMARY KEY (SeriesID, Key)
);

CREATE TABLE Features
(
	SeriesID UUID NOT NULL REFERENCES Series (ID),
	Key      TEXT NOT NULL,
	Value    BIT  NOT NULL,
	PRIMARY KEY (SeriesID, Key)
);

CREATE TABLE Iterations
(
	ID        UUID NOT NULL PRIMARY KEY,
	SeriesID  UUID NOT NULL REFERENCES Series (ID),
	Name      TEXT NOT NULL,
	StartDate DATE,
	EndDate   DATE,
	UNIQUE (SeriesID, Name),
	INDEX (StartDate, EndDate)
);

CREATE TABLE Races
(
	ID   UUID NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL INDEX,
	Type TEXT NOT NULL
);

CREATE TABLE RaceIterations
(
	IterationID UUID NOT NULL REFERENCES Iterations (ID),
	RaceID      UUID NOT NULL REFERENCES Races (ID),
	PRIMARY KEY (IterationID, RaceID)
);

CREATE TABLE Courses
(
	ID       UUID          NOT NULL PRIMARY KEY,
	RaceID   UUID          NOT NULL REFERENCES Races (ID),
	Distance NUMERIC(6, 3) NOT NULL,
	Units    CHAR(2)       NOT NULL
);

CREATE TABLE Challenges
(
	ID          UUID NOT NULL PRIMARY KEY,
	IterationID UUID NOT NULL REFERENCES Iteration (ID),
	Name        TEXT NOT NULL,
	IsOfficial  BIT  NOT NULL INDEX,
	IsPublic    BIT  NOT NULL INDEX,
	StartDate   DATE,
	EndDate     DATE,
	TimeLimit   TIME,
	UNIQUE (SeriesID, Name),
	INDEX (StartDate, EndDate)
);

CREATE TABLE ChallengeCourses
(
	ChallengeID UUID NOT NULL REFERENCES Challenges (ID),
	CourseID    UUID NOT NULL REFERENCES Courses (ID),
	PRIMARY KEY (ChallengeID, CourseID)
);

CREATE TABLE Athletes
(
	ID          UUID NOT NULL PRIMARY KEY,
	Name        TEXT NOT NULL INDEX,
	Category    CHAR NOT NULL INDEX,
	DateOfBirth DATE NOT NULL INDEX,
	IsPrivate   BIT  NOT NULL INDEX
);

CREATE TABLE Results
(
	ID        UUID      NOT NULL PRIMARY KEY,
	CourseID  UUID      NOT NULL REFERENCES Courses (ID),
	AthleteID UUID      NOT NULL REFERENCES Athletes (ID),
	StartTime TIMESTAMP NOT NULL,
	Duration  INT       NOT NULL INDEX,
	UNIQUE (AthleteID, StartTime)
);

CREATE TABLE LinkedAccounts
(
	ID        UUID NOT NULL PRIMARY KEY,
	AthleteID UUID NOT NULL REFERENCES Athletes (ID),
	Type      TEXT NOT NULL,
	Value     TEXT NOT NULL,
	UNIQUE (Type, Value)
);