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
	SeriesID UUID    NOT NULL REFERENCES Series (ID),
	Key      TEXT    NOT NULL,
	Value    BOOLEAN NOT NULL,
	PRIMARY KEY (SeriesID, Key)
);

CREATE TABLE Iterations
(
	ID        UUID NOT NULL PRIMARY KEY,
	SeriesID  UUID NOT NULL REFERENCES Series (ID),
	Name      TEXT NOT NULL,
	StartDate DATE,
	EndDate   DATE,
	UNIQUE (SeriesID, Name)
);

CREATE TABLE Races
(
	ID   UUID NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL UNIQUE,
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
	ID          UUID    NOT NULL PRIMARY KEY,
	IterationID UUID    NOT NULL REFERENCES Iterations (ID),
	Name        TEXT    NOT NULL,
	IsOfficial  BOOLEAN NOT NULL,
	IsPublic    BOOLEAN NOT NULL,
	StartDate   DATE,
	EndDate     DATE,
	TimeLimit   SMALLINT,
	UNIQUE (IterationID, Name)
);

CREATE TABLE ChallengeCourses
(
	ChallengeID UUID NOT NULL REFERENCES Challenges (ID),
	CourseID    UUID NOT NULL REFERENCES Courses (ID),
	PRIMARY KEY (ChallengeID, CourseID)
);

CREATE TABLE Athletes
(
	ID          UUID    NOT NULL PRIMARY KEY,
	Name        TEXT    NOT NULL,
	Category    CHAR    NOT NULL,
	DateOfBirth DATE    NOT NULL,
	IsPrivate   BOOLEAN NOT NULL
);

CREATE TABLE Results
(
	ID        UUID      NOT NULL PRIMARY KEY,
	CourseID  UUID      NOT NULL REFERENCES Courses (ID),
	AthleteID UUID      NOT NULL REFERENCES Athletes (ID),
	StartTime TIMESTAMP NOT NULL,
	Duration  INT       NOT NULL,
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