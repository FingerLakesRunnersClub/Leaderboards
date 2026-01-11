CREATE TABLE Settings
(
	ID    VARCHAR(125) NOT NULL PRIMARY KEY,
	Value VARCHAR(125) NOT NULL
);

CREATE TABLE Races
(
	ID           UUID         NOT NULL PRIMARY KEY,
	Name         VARCHAR(125) NOT NULL,
	Type         VARCHAR(125) NOT NULL,
	Date         DATE         NOT NULL,
	AllowInvalid BIT          NOT NULL,
	CommunityID  SMALLINT NULL
);

CREATE TABLE Courses
(
	ID       UUID          NOT NULL PRIMARY KEY,
	RaceID   UUID          NOT NULL REFERENCES Races (ID),
	Distance NUMERIC(6, 3) NOT NULL,
	Units    CHAR(2)       NOT NULL
);