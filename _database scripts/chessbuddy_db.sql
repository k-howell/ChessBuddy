/* check if database exists, and drop it if it does */
print '*** checking database ***'
GO
IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases
			WHERE name = 'chessbuddy_db')
BEGIN
	DROP DATABASE chessbuddy_db
	print '' print '*** Dropping database chessbuddy_db ***'
END
GO

print '' print '*** Creating database chessbuddy_db ***'
GO
CREATE DATABASE chessbuddy_db
GO

print '' print '*** Using database chessbuddy_db ***'
GO
USE [chessbuddy_db]
GO

/* ----- Tables ----- */

print '' print '*** Creating User table ***'
GO
CREATE TABLE dbo.[User] (
	UserID				[nvarchar](20)				NOT NULL,
	GivenName			[nvarchar](50)				NOT NULL,
	FamilyName			[nvarchar](100)				NOT NULL,
	Email				[nvarchar](100)				NOT NULL,
	PasswordHash		[nvarchar](100)				NOT NULL DEFAULT
		'9C9064C59F1FFA2E174EE754D2979BE80DD30DB552EC03E7E327E9B1A4BD594E',
	Active				[bit]						NOT NULL DEFAULT 1,
	CONSTRAINT pk_User PRIMARY KEY(UserID),
	CONSTRAINT ak_User_Email UNIQUE(Email)
)
GO

print '' print '*** Creating Role table ***'
GO
CREATE TABLE dbo.[Role] (
	RoleID				[nvarchar](20)				NOT NULL
	CONSTRAINT pk_Role PRIMARY KEY(RoleID)
)
GO

print '' print '*** Creating UserRole table ***'
GO
CREATE TABLE dbo.[UserRole] (
	UserID				[nvarchar](20)				NOT NULL,
	RoleID				[nvarchar](20)				NOT NULL,
	CONSTRAINT pk_UserRole PRIMARY KEY(UserID, RoleID),
	CONSTRAINT fk_UserRole_UserID FOREIGN KEY(UserID)
		REFERENCES [User](UserID),
	CONSTRAINT fk_UserRole_RoleID FOREIGN KEY(RoleID)
		REFERENCES [Role](RoleID) ON UPDATE CASCADE
)
GO

print '' print '*** Creating Piece table ***'
GO
CREATE TABLE dbo.[Piece] (
	PieceID				[nvarchar](6)				NOT NULL,
	CONSTRAINT pk_Piece PRIMARY KEY(PieceID)
)
GO

print '' print '*** Creating Square table ***'
GO
CREATE TABLE dbo.[Square] (
	SquareID			[nchar](2)					NOT NULL,
	SquareFile			[nchar](1)					NOT NULL,
	SquareRank			[int]						NOT NULL,
	CONSTRAINT pk_Square PRIMARY KEY(SquareID)
)
GO

print '' print '*** Creating Color table ***'
GO
CREATE TABLE dbo.[Color] (
	ColorID				[nchar](5)					NOT NULL,
	CONSTRAINT pk_Color PRIMARY KEY(ColorID)
)
GO

print '' print '*** Creating Opening table ***'
GO
CREATE TABLE dbo.[Opening] (
	ECO					[nchar](3)					NOT NULL,
	Name				[nvarchar](50)				NOT NULL
	CONSTRAINT pk_Opening PRIMARY KEY(ECO)
)
GO

print '' print '*** Creating OpeningVariation table ***'
GO
CREATE TABLE dbo.[OpeningVariation] (
	VariationID			[int] IDENTITY(100000, 1)	NOT NULL,
	Name				[nvarchar](50)				NOT NULL,
	ECO					[nchar](3)					NOT NULL,
	CONSTRAINT pk_OpeningVariation PRIMARY KEY(VariationID),
	CONSTRAINT ak_OpeningVariation_Name UNIQUE(Name),
	CONSTRAINT fk_OpeningVariation_ECO FOREIGN KEY(ECO)
		REFERENCES [Opening](ECO)
)
GO

print '' print '*** Creating Game table ***'
GO
CREATE TABLE dbo.[Game] (
	GameID				[int] IDENTITY(100000, 1)	NOT NULL,
	PlayerWhite			[nvarchar](20)				NOT NULL DEFAULT 'Player1',
	WhiteElo			[int]						NULL,
	PlayerBlack			[nvarchar](20)				NOT NULL DEFAULT 'Player2',
	BlackElo			[int]						NULL,
	ECO					[nchar](3)					NULL,
	OpeningVariation	[nvarchar](50)				NULL,
	Termination			[nvarchar](20)				NOT NULL DEFAULT 'Normal',
	Outcome				[nvarchar](7)				NOT NULL,
	TimeControl			[nvarchar](10)				NULL,
	DatePlayed			[date]						NULL,
	CONSTRAINT pk_Game PRIMARY KEY(GameID),
	CONSTRAINT fk_Game_ECO FOREIGN KEY(ECO)
		REFERENCES [Opening](ECO),
	CONSTRAINT fk_Game_OpeningVariation FOREIGN KEY(OpeningVariation)
		REFERENCES [OpeningVariation](Name),
)
GO

print '' print '*** Creating UserGame table ***'
GO
CREATE TABLE dbo.[UserGame] (
	UserID				[nvarchar](20)				NOT NULL,
	GameID				[int]						NOT NULL,
	CONSTRAINT pk_UserGame PRIMARY KEY(UserID, GameID),
	CONSTRAINT fk_UserGame_UserID FOREIGN KEY(UserID)
		REFERENCES [User](UserID),
	CONSTRAINT fk_UserGame_GameID FOREIGN KEY(GameID)
		REFERENCES [Game](GameID)
)
GO

print '' print '*** Creating GameMove table ***'
GO
CREATE TABLE dbo.[GameMove] (
	GameID				[int]						NOT NULL,
	Turn				[int]						NOT NULL,
	Color				[nchar](5)					NOT NULL,
	Piece				[nvarchar](6)				NOT NULL,
	Notation			[nvarchar](10)				NOT NULL,
	Origin				[nchar](2)					NOT NULL,
	Destination			[nchar](2)					NOT NULL,
	Capture				[nvarchar](6)				NULL,
	Promotion			[nvarchar](6)				NULL,
	IsCheck				[bit]						NOT NULL DEFAULT 0,
	IsMate				[bit]						NOT NULL DEFAULT 0,
	IsCastle			[bit]						NOT NULL DEFAULT 0,
	IsEnPassant			[bit]						NOT NULL DEFAULT 0,
	CONSTRAINT pk_GameMove PRIMARY KEY(GameID, Turn, Color),
	CONSTRAINT fk_GameMove_GameID FOREIGN KEY(GameID)
		REFERENCES [Game](GameID),
	CONSTRAINT fk_GameMove_Color FOREIGN KEY(Color)
		REFERENCES [Color](ColorID),
	CONSTRAINT fk_GameMove_Piece FOREIGN KEY(Piece)
		REFERENCES [Piece](PieceID),
	CONSTRAINT fk_GameMove_Origin FOREIGN KEY(Origin)
		REFERENCES [Square](SquareID),
	CONSTRAINT fk_GameMove_Destination FOREIGN KEY(Destination)
		REFERENCES [Square](SquareID),
	CONSTRAINT fk_GameMove_Capture FOREIGN KEY(Capture)
		REFERENCES [Piece](PieceID),
	CONSTRAINT fk_GameMove_Promotion FOREIGN KEY(Promotion)
		REFERENCES [Piece](PieceID)
)
GO

/* ----- Sample Data ----- */

print '' print '*** Creating test User records ***'
GO
INSERT INTO dbo.[User] (
	UserID, GivenName, FamilyName, Email
)
VALUES
	('acampbell', 'Aaron', 'Campbell', 'aaron@testing.com'),
	('nbryant', 'Nate', 'Bryant', 'nate@testing.com'),
	('jsmith', 'Jessen', 'Smith', 'jessen@testing.com'),
	('mrogers', 'Mitch', 'Rogers', 'mitch@testing.com'),
	('mcarrazza', 'Miguel', 'Carrazza', 'miguel@testing.com'),
	('khowell', 'Kris', 'Howell', 'kris@testing.com'),
	('showell', 'Scott', 'Howell', 'scott@testing.com'),
	('jglasgow', 'Jim', 'Glasgow', 'jim@testing.com')
GO

print '' print '*** populating Role lookup table ***'
INSERT INTO dbo.[Role] (
	RoleID
)
VALUES
	('Guest'),
	('Member'),
	('Admin')
GO

print '' print '*** Creating test UserRole records ***'
GO
INSERT INTO dbo.[UserRole] (
	UserID, RoleID
)
VALUES
	('acampbell', 'Guest'),
	('nbryant', 'Guest'),
	('jsmith', 'Guest'),
	('mrogers', 'Guest'),
	('mcarrazza', 'Guest'),
	('showell', 'Guest'),
	('showell', 'Member'),
	('jglasgow', 'Guest'),
	('jglasgow', 'Member'),
	('khowell', 'Guest'),
	('khowell', 'Member'),
	('khowell', 'Admin')
GO

print '' print '*** populating Piece lookup table ***'
GO
INSERT INTO dbo.[Piece] (
	PieceID
)
VALUES
	('Pawn'),
	('Knight'),
	('Bishop'),
	('Rook'),
	('Queen'),
	('King')
GO

print '' print '*** populating Square lookup table ***'
GO
INSERT INTO dbo.[Square] (
	SquareID, SquareFile, SquareRank
)
VALUES
	('a1', 'a', 1),
	('a2', 'a', 2),
	('a3', 'a', 3),
	('a4', 'a', 4),
	('a5', 'a', 5),
	('a6', 'a', 6),
	('a7', 'a', 7),
	('a8', 'a', 8),
	('b1', 'b', 1),
	('b2', 'b', 2),
	('b3', 'b', 3),
	('b4', 'b', 4),
	('b5', 'b', 5),
	('b6', 'b', 6),
	('b7', 'b', 7),
	('b8', 'b', 8),
	('c1', 'c', 1),
	('c2', 'c', 2),
	('c3', 'c', 3),
	('c4', 'c', 4),
	('c5', 'c', 5),
	('c6', 'c', 6),
	('c7', 'c', 7),
	('c8', 'c', 8),
	('d1', 'd', 1),
	('d2', 'd', 2),
	('d3', 'd', 3),
	('d4', 'd', 4),
	('d5', 'd', 5),
	('d6', 'd', 6),
	('d7', 'd', 7),
	('d8', 'd', 8),
	('e1', 'e', 1),
	('e2', 'e', 2),
	('e3', 'e', 3),
	('e4', 'e', 4),
	('e5', 'e', 5),
	('e6', 'e', 6),
	('e7', 'e', 7),
	('e8', 'e', 8),
	('f1', 'f', 1),
	('f2', 'f', 2),
	('f3', 'f', 3),
	('f4', 'f', 4),
	('f5', 'f', 5),
	('f6', 'f', 6),
	('f7', 'f', 7),
	('f8', 'f', 8),
	('g1', 'g', 1),
	('g2', 'g', 2),
	('g3', 'g', 3),
	('g4', 'g', 4),
	('g5', 'g', 5),
	('g6', 'g', 6),
	('g7', 'g', 7),
	('g8', 'g', 8),
	('h1', 'h', 1),
	('h2', 'h', 2),
	('h3', 'h', 3),
	('h4', 'h', 4),
	('h5', 'h', 5),
	('h6', 'h', 6),
	('h7', 'h', 7),
	('h8', 'h', 8)	
GO

print '' print '*** populating Color lookup table ***'
GO
INSERT INTO dbo.[Color] (
	ColorID
)
VALUES
	('White'),
	('Black')
GO

print '' print '*** Creating test Opening records ***'
GO
INSERT INTO dbo.[Opening] (
	ECO, Name
)
VALUES
	('A20', 'English Opening'),
	('B33', 'Sicilian Defense'),
	('B44', 'Sicilian Defense')
GO

print '' print '*** Creating test OpeningVariation records ***'
GO
INSERT INTO dbo.[OpeningVariation] (
	Name, ECO
)
VALUES
	("English Opening: King's English Variation", 'A20'),
	("Sicilian Defense: Lasker-Pelikan Variation", 'B33'),
	("Sicilian, Szen (`anti-Taimanov') variation", 'B44')
GO

print '' print '*** Creating test Game records ***'
GO
INSERT INTO dbo.[Game] (
	PlayerWhite, WhiteElo, PlayerBlack, BlackElo, ECO, OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
)
VALUES
	('Penguin12356', 2489, 'Doa79', 2472, 'A20', "English Opening: King's English Variation", 'Normal', '0-1', '180+2', '2020-05-01'),
	('sardarmelli', 2223, 'Dafnu', 2401, 'B33', "Sicilian Defense: Lasker-Pelikan Variation", 'Normal', '1-0', '180+0', '2020-05-03'),
	('Anatoly Karpov', 2720, 'Garry Kasparov', 2715, 'B44', "Sicilian, Szen (`anti-Taimanov') variation", 'Normal', '0-1', '7200+30', '1985-10-15')
GO

print '' print '*** Creating test GameMove records ***'
INSERT INTO dbo.[GameMove] (
	GameID, Turn, Color, Piece, Origin, Destination, Notation, Capture, Promotion, IsCheck, IsMate, IsCastle, IsEnPassant
)
VALUES
	(100000, 1, 'White', 'Pawn', 'c2', 'c4', 'c4', null, null, 0, 0, 0, 0),
	(100000, 1, 'Black', 'Pawn', 'e7', 'e5', 'e5', null, null, 0, 0, 0, 0),
	(100000, 2, 'White', 'Pawn', 'g2', 'g3', 'g3', null, null, 0, 0, 0, 0),
	(100000, 2, 'Black', 'Knight', 'b8', 'c6', 'Nc6', null, null, 0, 0, 0, 0),
	(100000, 3, 'White', 'Bishop', 'f1', 'g2', 'Bg2', null, null, 0, 0, 0, 0),
	(100000, 3, 'Black', 'Bishop', 'f8', 'c5', 'Bc5', null, null, 0, 0, 0, 0),
	(100000, 4, 'White', 'Knight', 'b1', 'c3', 'Nc3', null, null, 0, 0, 0, 0),
	(100000, 4, 'Black', 'Pawn', 'd7', 'd6', 'd6', null, null, 0, 0, 0, 0),
	(100000, 5, 'White', 'Pawn', 'e2', 'e3', 'e3', null, null, 0, 0, 0, 0),
	(100000, 5, 'Black', 'Pawn', 'a7', 'a6', 'a6', null, null, 0, 0, 0, 0),
	(100000, 6, 'White', 'Knight', 'g1', 'f3', 'Nf3', null, null, 0, 0, 0, 0),
	(100000, 6, 'Black', 'Bishop', 'c5', 'a7', 'Ba7', null, null, 0, 0, 0, 0),
	(100000, 7, 'White', 'Pawn', 'd2', 'd4', 'd4', null, null, 0, 0, 0, 0),
	(100000, 7, 'Black', 'Pawn', 'f7', 'f6', 'f6', null, null, 0, 0, 0, 0),
	(100000, 8, 'White', 'King', 'e1', 'g1', 'O-O', null, null, 0, 0, 1, 0),
	(100000, 8, 'Black', 'Pawn', 'h7', 'h5', 'h5', null, null, 0, 0, 0, 0),
	(100000, 9, 'White', 'Queen', 'd1', 'd3', 'Qd3', null, null, 0, 0, 0, 0),
	(100000, 9, 'Black', 'Knight', 'g8', 'e7', 'Nge7', null, null, 0, 0, 0, 0),
	(100000, 10, 'White', 'Pawn', 'h2', 'h4', 'h4', null, null, 0, 0, 0, 0),
	(100000, 10, 'Black', 'Bishop', 'c8', 'g4', 'Bg4', null, null, 0, 0, 0, 0),
	(100000, 11, 'White', 'Pawn', 'd6', 'd5', 'd5', null, null, 0, 0, 0, 0),
	(100000, 11, 'Black', 'Knight', 'c6', 'b4', 'Nb4', null, null, 0, 0, 0, 0),
	(100000, 12, 'White', 'Queen', 'd3', 'd1', 'Qd1', null, null, 0, 0, 0, 0),
	(100000, 12, 'Black', 'Pawn', 'a6', 'a5', 'a5', null, null, 0, 0, 0, 0),
	(100000, 13, 'White', 'Pawn', 'a2', 'a3', 'a3', null, null, 0, 0, 0, 0),
	(100000, 13, 'Black', 'Knight', 'b4', 'a6', 'Na6', null, null, 0, 0, 0, 0),
	(100000, 14, 'White', 'Queen', 'd1', 'a4', 'Qa4+', null, null, 1, 0, 0, 0),
	(100000, 14, 'Black', 'Queen', 'd8', 'd7', 'Qd7', null, null, 0, 0, 0, 0),
	(100000, 15, 'White', 'Queen', 'a4', 'd7', 'Qxd7+', 'Queen', null, 1, 0, 0, 0),
	(100000, 15, 'Black', 'Bishop', 'g4', 'd7', 'Bxd7', 'Queen', null, 0, 0, 0, 0),
	(100000, 16, 'White', 'Rook', 'a1', 'b1', 'Rb1', null, null, 0, 0, 0, 0),
	(100000, 16, 'Black', 'Pawn', 'a5', 'a4', 'a4', null, null, 0, 0, 0, 0),
	(100000, 17, 'White', 'Knight', 'c3', 'b5', 'Nb5', null, null, 0, 0, 0, 0),
	(100000, 17, 'Black', 'Bishop', 'a7', 'b6', 'Bb6', null, null, 0, 0, 0, 0),
	(100000, 18, 'White', 'Knight', 'f3', 'd2', 'Nd2', null, null, 0, 0, 0, 0),
	(100000, 18, 'Black', 'Knight', 'a6', 'c5', 'Nc5', null, null, 0, 0, 0, 0),
	(100000, 19, 'White', 'Pawn', 'b2', 'b4', 'b4', null, null, 0, 0, 0, 0),
	(100000, 19, 'Black', 'Pawn', 'a4', 'b3', 'axb3', 'Pawn', null, 0, 0, 0, 1),
	(100000, 20, 'White', 'Knight', 'd2', 'b3', 'Nxb3', 'Pawn', null, 0, 0, 0, 0),
	(100000, 20, 'Black', 'Knight', 'c5', 'a4', 'Na4', null, null, 0, 0, 0, 0),
	(100000, 21, 'White', 'Bishop', 'c1', 'd2', 'Bd2', null, null, 0, 0, 0, 0),
	(100000, 21, 'Black', 'Pawn', 'g7', 'g5', 'g5', null, null, 0, 0, 0, 0),
	(100000, 22, 'White', 'Pawn', 'h4', 'g5', 'hxg5', 'Pawn', null, 0, 0, 0, 0),
	(100000, 22, 'Black', 'Pawn', 'f6', 'g5', 'fxg5', 'Pawn', null, 0, 0, 0, 0),
	(100000, 23, 'White', 'Pawn', 'e3', 'e4', 'e4', null, null, 0, 0, 0, 0),
	(100000, 23, 'Black', 'Pawn', 'h5', 'h4', 'h4', null, null, 0, 0, 0, 0),
	(100000, 24, 'White', 'Bishop', 'd2', 'g5', 'Bxg5', 'Pawn', null, 0, 0, 0, 0),
	(100000, 24, 'Black', 'Pawn', 'h4', 'g3', 'hxg3', 'Pawn', null, 0, 0, 0, 0),
	(100000, 25, 'White', 'Pawn', 'c4', 'c5', 'c5', null, null, 0, 0, 0, 0),
	(100000, 25, 'Black', 'Bishop', 'd7', 'b5', 'Bxb5', 'Knight', null, 0, 0, 0, 0),
	(100000, 26, 'White', 'Pawn', 'c5', 'b6', 'cxb6', 'Bishop', null, 0, 0, 0, 0),
	(100000, 26, 'Black', 'Bishop', 'b5', 'f1', 'Bxf1', 'Rook', null, 0, 0, 0, 0),
	(100000, 27, 'White', 'Rook', 'b1', 'f1', 'Rxf1', 'Bishop', null, 0, 0, 0, 0),
	(100000, 27, 'Black', 'Pawn', 'g3', 'f2', 'gxf2+', 'Pawn', null, 1, 0, 0, 0),
	(100000, 28, 'White', 'Rook', 'f1', 'f2', 'Rxf2', 'Pawn', null, 0, 0, 0, 0),
	(100000, 28, 'Black', 'Knight', 'a4', 'b6', 'Nxb6', 'Pawn', null, 0, 0, 0, 0),
	
	(100001, 1, 'White', 'Pawn', 'e2', 'e4', 'e4', null, null, 0, 0, 0, 0),
	(100001, 1, 'Black', 'Pawn', 'c7', 'c5', 'c5', null, null, 0, 0, 0, 0),
	(100001, 2, 'White', 'Knight', 'g1', 'f3', 'Nf3', null, null, 0, 0, 0, 0),
	(100001, 2, 'Black', 'Knight', 'b8', 'c6', 'Nc6', null, null, 0, 0, 0, 0),
	(100001, 3, 'White', 'Pawn', 'd2', 'd4', 'd4', null, null, 0, 0, 0, 0),
	(100001, 3, 'Black', 'Pawn', 'c5', 'd4', 'cxd4', 'Pawn', null, 0, 0, 0, 0),
	(100001, 4, 'White', 'Knight', 'f3', 'd4', 'Nxd4', 'Pawn', null, 0, 0, 0, 0),
	(100001, 4, 'Black', 'Knight', 'g8', 'f6', 'Nf6', null, null, 0, 0, 0, 0),
	(100001, 5, 'White', 'Knight', 'b1', 'c3', 'Nc3', null, null, 0, 0, 0, 0),
	(100001, 5, 'Black', 'Pawn', 'e7', 'e5', 'e5', null, null, 0, 0, 0, 0),
	(100001, 6, 'White', 'Knight', 'd4', 'b5', 'Ndb5', null, null, 0, 0, 0, 0),
	(100001, 6, 'Black', 'Pawn', 'd7', 'd6', 'd6', null, null, 0, 0, 0, 0),
	(100001, 7, 'White', 'Pawn', 'a2', 'a4', 'a4', null, null, 0, 0, 0, 0),
	(100001, 7, 'Black', 'Pawn', 'a7', 'a6', 'a6', null, null, 0, 0, 0, 0),
	(100001, 8, 'White', 'Knight', 'b5', 'a3', 'Na3', null, null, 0, 0, 0, 0),
	(100001, 8, 'Black', 'Bishop', 'c8', 'g4', 'Bg4', null, null, 0, 0, 0, 0),
	(100001, 9, 'White', 'Bishop', 'f1', 'e2', 'Be2', null, null, 0, 0, 0, 0),
	(100001, 9, 'Black', 'Bishop', 'g4', 'e2', 'Bxe2', 'Bishop', null, 0, 0, 0, 0),
	(100001, 10, 'White', 'Queen', 'd1', 'e2', 'Qxe2', 'Bishop', null, 0, 0, 0, 0),
	(100001, 10, 'Black', 'Knight', 'c6', 'd4', 'Nd4', null, null, 0, 0, 0, 0),
	(100001, 11, 'White', 'Queen', 'e2', 'd3', 'Qd3', null, null, 0, 0, 0, 0),
	(100001, 11, 'Black', 'Bishop', 'f8', 'e7', 'Be7', null, null, 0, 0, 0, 0),
	(100001, 12, 'White', 'Bishop', 'c1', 'g5', 'Bg5', null, null, 0, 0, 0, 0),
	(100001, 12, 'Black', 'King', 'e8', 'g8', 'O-O', null, null, 0, 0, 1, 0),
	(100001, 13, 'White', 'Bishop', 'g5', 'f6', 'Bxf6', 'Knight', null, 0, 0, 0, 0),
	(100001, 13, 'Black', 'Bishop', 'e7', 'f6', 'Bxf6', 'Bishop', null, 0, 0, 0, 0),
	(100001, 14, 'White', 'Knight', 'c3', 'd5', 'Nd5', null, null, 0, 0, 0, 0),
	(100001, 14, 'Black', 'Bishop', 'f6', 'g5', 'Bg5', null, null, 0, 0, 0, 0),
	(100001, 15, 'White', 'Pawn', 'c2', 'c3', 'c3', null, null, 0, 0, 0, 0),
	(100001, 15, 'Black', 'Knight', 'd4', 'e6', 'Ne6', null, null, 0, 0, 0, 0),
	(100001, 16, 'White', 'Knight', 'a3', 'c4', 'Nc4', null, null, 0, 0, 0, 0),
	(100001, 16, 'Black', 'Rook', 'a8', 'c8', 'Rc8', null, null, 0, 0, 0, 0),
	(100001, 17, 'White', 'Pawn', 'g2', 'g3', 'g3', null, null, 0, 0, 0, 0),
	(100001, 17, 'Black', 'Knight', 'e6', 'c5', 'Nc5', null, null, 0, 0, 0, 0),
	(100001, 18, 'White', 'Queen', 'd3', 'e2', 'Qe2', null, null, 0, 0, 0, 0),
	(100001, 18, 'Black', 'Pawn', 'f7', 'f5', 'f5', null, null, 0, 0, 0, 0),
	(100001, 19, 'White', 'Pawn', 'e4', 'f5', 'exf5', 'Pawn', null, 0, 0, 0, 0),
	(100001, 19, 'Black', 'Rook', 'f8', 'f5', 'Rxf5', 'Pawn', null, 0, 0, 0, 0),
	(100001, 20, 'White', 'King', 'e1', 'g1', 'O-O', null, null, 0, 0, 1, 0),
	(100001, 20, 'Black', 'Queen', 'd8', 'f8', 'Qf8', null, null, 0, 0, 0, 0),
	(100001, 21, 'White', 'Knight', 'c4', 'b6', 'Ncb6', null, null, 0, 0, 0, 0),
	(100001, 21, 'Black', 'Rook', 'c8', 'e8', 'Re8', null, null, 0, 0, 0, 0),
	(100001, 22, 'White', 'Pawn', 'b2', 'b4', 'b4', null, null, 0, 0, 0, 0),
	(100001, 22, 'Black', 'Knight', 'c5', 'e6', 'Ne6', null, null, 0, 0, 0, 0),
	(100001, 23, 'White', 'Queen', 'e2', 'c4', 'Qc4', null, null, 0, 0, 0, 0),
	(100001, 23, 'Black', 'King', 'g8', 'h8', 'Kh8', null, null, 0, 0, 0, 0),
	(100001, 24, 'White', 'Knight', 'd5', 'c7', 'Nc7', null, null, 0, 0, 0, 0),
	(100001, 24, 'Black', 'Knight', 'e6', 'c7', 'Nxc7', 'Knight', null, 0, 0, 0, 0),
	(100001, 25, 'White', 'Queen', 'c4', 'c7', 'Qxc7', 'Knight', null, 0, 0, 0, 0),
	(100001, 25, 'Black', 'Rook', 'f5', 'f7', 'Rf7', null, null, 0, 0, 0, 0),
	(100001, 26, 'White', 'Queen', 'c7', 'c4', 'Qc4', null, null, 0, 0, 0, 0),
	(100001, 26, 'Black', 'Pawn', 'e5', 'e4', 'e4', null, null, 0, 0, 0, 0),
	(100001, 27, 'White', 'Knight', 'b6', 'd5', 'Nd5', null, null, 0, 0, 0, 0),
	(100001, 27, 'Black', 'Pawn', 'h7', 'h6', 'h6', null, null, 0, 0, 0, 0),
	(100001, 28, 'White', 'Rook', 'a1', 'e1', 'Rae1', null, null, 0, 0, 0, 0),
	(100001, 28, 'Black', 'Pawn', 'e4', 'e3', 'e3', null, null, 0, 0, 0, 0),
	(100001, 29, 'White', 'Pawn', 'f2', 'e3', 'fxe3', 'Pawn', null, 0, 0, 0, 0),
	(100001, 29, 'Black', 'Rook', 'f7', 'f1', 'Rxf1+', 'Rook', null, 1, 0, 0, 0),
	(100001, 30, 'White', 'Rook', 'e1', 'f1', 'Rxf1', 'Rook', null, 0, 0, 0, 0),
	(100001, 30, 'Black', 'Queen', 'f8', 'g8', 'Qg8', null, null, 0, 0, 0, 0),
	(100001, 31, 'White', 'Queen', 'c4', 'd3', 'Qd3', null, null, 0, 0, 0, 0),
	(100001, 31, 'Black', 'Queen', 'g8', 'e6', 'Qe6', null, null, 0, 0, 0, 0),
	(100001, 32, 'White', 'King', 'g1', 'g2', 'Kg2', null, null, 0, 0, 0, 0),
	(100001, 32, 'Black', 'Rook', 'e8', 'c8', 'Rc8', null, null, 0, 0, 0, 0),
	(100001, 33, 'White', 'Pawn', 'h2', 'h4', 'h4', null, null, 0, 0, 0, 0),
	(100001, 33, 'Black', 'Bishop', 'g5', 'd8', 'Bd8', null, null, 0, 0, 0, 0),
	(100001, 34, 'White', 'Knight', 'd5', 'f4', 'Nf4', null, null, 0, 0, 0, 0),
	(100001, 34, 'Black', 'Queen', 'e6', 'c4', 'Qc4', null, null, 0, 0, 0, 0),
	(100001, 35, 'White', 'Knight', 'f4', 'g6', 'Ng6+', null, null, 1, 0, 0, 0),
	(100001, 35, 'Black', 'King', 'h8', 'h7', 'Kh7', null, null, 0, 0, 0, 0),
	(100001, 36, 'White', 'Knight', 'g6', 'f8', 'Nf8+', null, null, 1, 0, 0, 0),
	(100001, 36, 'Black', 'King', 'h7', 'g8', 'Kg8', null, null, 0, 0, 0, 0),
	(100001, 37, 'White', 'Queen', 'd3', 'h7', 'Qh7#', null, null, 1, 1, 0, 0),
	
	(100002, 1, 'White', 'Pawn', 'e2', 'e4', 'e4', null, null, 0, 0, 0, 0),
	(100002, 1, 'Black', 'Pawn', 'c7', 'c5', 'c5', null, null, 0, 0, 0, 0),
	(100002, 2, 'White', 'Knight', 'g1', 'f3', 'Nf3', null, null, 0, 0, 0, 0),
	(100002, 2, 'Black', 'Pawn', 'e7', 'e6', 'e6', null, null, 0, 0, 0, 0),
	(100002, 3, 'White', 'Pawn', 'd2', 'd4', 'd4', null, null, 0, 0, 0, 0),
	(100002, 3, 'Black', 'Pawn', 'c5', 'd4', 'cxd4', 'Pawn', null, 0, 0, 0, 0),
	(100002, 4, 'White', 'Knight', 'f3', 'd4', 'Nxd4', 'Pawn', null, 0, 0, 0, 0),
	(100002, 4, 'Black', 'Knight', 'b8', 'c6', 'Nc6', null, null, 0, 0, 0, 0),
	(100002, 5, 'White', 'Knight', 'd4', 'b5', 'Nb5', null, null, 0, 0, 0, 0),
	(100002, 5, 'Black', 'Pawn', 'd7', 'd6', 'd6', null, null, 0, 0, 0, 0),
	(100002, 6, 'White', 'Pawn', 'c2', 'c4', 'c4', null, null, 0, 0, 0, 0),
	(100002, 6, 'Black', 'Knight', 'g8', 'f6', 'Nf6', null, null, 0, 0, 0, 0),
	(100002, 7, 'White', 'Knight', 'b1', 'c3', 'N1c3', null, null, 0, 0, 0, 0),
	(100002, 7, 'Black', 'Pawn', 'a7', 'a6', 'a6', null, null, 0, 0, 0, 0),
	(100002, 8, 'White', 'Knight', 'b5', 'a3', 'Na3', null, null, 0, 0, 0, 0),
	(100002, 8, 'Black', 'Pawn', 'd6', 'd5', 'd5', null, null, 0, 0, 0, 0),
	(100002, 9, 'White', 'Pawn', 'c4', 'd5', 'cxd5', 'Pawn', null, 0, 0, 0, 0),
	(100002, 9, 'Black', 'Pawn', 'e6', 'd5', 'exd5', 'Pawn', null, 0, 0, 0, 0),
	(100002, 10, 'White', 'Pawn', 'e4', 'd5', 'exd5', 'Pawn', null, 0, 0, 0, 0),
	(100002, 10, 'Black', 'Knight', 'c6', 'b4', 'Nb4', null, null, 0, 0, 0, 0),
	(100002, 11, 'White', 'Bishop', 'f1', 'e2', 'Be2', null, null, 0, 0, 0, 0),
	(100002, 11, 'Black', 'Bishop', 'f8', 'c5', 'Bc5', null, null, 0, 0, 0, 0),
	(100002, 12, 'White', 'King', 'e1', 'g1', 'O-O', null, null, 0, 0, 1, 0),
	(100002, 12, 'Black', 'King', 'e8', 'g8', 'O-O', null, null, 0, 0, 1, 0),
	(100002, 13, 'White', 'Bishop', 'e2', 'f3', 'Bf3', null, null, 0, 0, 0, 0),
	(100002, 13, 'Black', 'Bishop', 'c8', 'f5', 'Bf5', null, null, 0, 0, 0, 0),
	(100002, 14, 'White', 'Bishop', 'c1', 'g5', 'Bg5', null, null, 0, 0, 0, 0),
	(100002, 14, 'Black', 'Rook', 'f8', 'e8', 'Re8', null, null, 0, 0, 0, 0),
	(100002, 15, 'White', 'Queen', 'd1', 'd2', 'Qd2', null, null, 0, 0, 0, 0),
	(100002, 15, 'Black', 'Pawn', 'b7', 'b5', 'b5', null, null, 0, 0, 0, 0),
	(100002, 16, 'White', 'Rook', 'a1', 'd1', 'Rad1', null, null, 0, 0, 0, 0),
	(100002, 16, 'Black', 'Knight', 'b4', 'd3', 'Nd3', null, null, 0, 0, 0, 0),
	(100002, 17, 'White', 'Knight', 'a3', 'b1', 'Nab1', null, null, 0, 0, 0, 0),
	(100002, 17, 'Black', 'Pawn', 'h7', 'h6', 'h6', null, null, 0, 0, 0, 0),
	(100002, 18, 'White', 'Bishop', 'g5', 'h4', 'Bh4', null, null, 0, 0, 0, 0),
	(100002, 18, 'Black', 'Pawn', 'b5', 'b4', 'b4', null, null, 0, 0, 0, 0),
	(100002, 19, 'White', 'Knight', 'c3', 'a4', 'Na4', null, null, 0, 0, 0, 0),
	(100002, 19, 'Black', 'Bishop', 'c5', 'd6', 'Bd6', null, null, 0, 0, 0, 0),
	(100002, 20, 'White', 'Bishop', 'h4', 'g3', 'Bg3', null, null, 0, 0, 0, 0),
	(100002, 20, 'Black', 'Rook', 'a8', 'c8', 'Rc8', null, null, 0, 0, 0, 0),
	(100002, 21, 'White', 'Pawn', 'b2', 'b3', 'b3', null, null, 0, 0, 0, 0),
	(100002, 21, 'Black', 'Pawn', 'g7', 'g5', 'g5', null, null, 0, 0, 0, 0),
	(100002, 22, 'White', 'Bishop', 'g3', 'd6', 'Bxd6', 'Bishop', null, 0, 0, 0, 0),
	(100002, 22, 'Black', 'Queen', 'd8', 'd6', 'Qxd6', 'Bishop', null, 0, 0, 0, 0),
	(100002, 23, 'White', 'Pawn', 'g2', 'g3', 'g3', null, null, 0, 0, 0, 0),
	(100002, 23, 'Black', 'Knight', 'f6', 'd7', 'Nd7', null, null, 0, 0, 0, 0),
	(100002, 24, 'White', 'Bishop', 'f3', 'g2', 'Bg2', null, null, 0, 0, 0, 0),
	(100002, 24, 'Black', 'Queen', 'd6', 'f6', 'Qf6', null, null, 0, 0, 0, 0),
	(100002, 25, 'White', 'Pawn', 'a2', 'a3', 'a3', null, null, 0, 0, 0, 0),
	(100002, 25, 'Black', 'Pawn', 'a6', 'a5', 'a5', null, null, 0, 0, 0, 0),
	(100002, 26, 'White', 'Pawn', 'a3', 'b4', 'axb4', 'Pawn', null, 0, 0, 0, 0),
	(100002, 26, 'Black', 'Pawn', 'a5', 'b4', 'axb4', 'Pawn', null, 0, 0, 0, 0),
	(100002, 27, 'White', 'Queen', 'd2', 'a2', 'Qa2', null, null, 0, 0, 0, 0),
	(100002, 27, 'Black', 'Bishop', 'f5', 'g6', 'Bg6', null, null, 0, 0, 0, 0),
	(100002, 28, 'White', 'Pawn', 'd5', 'd6', 'd6', null, null, 0, 0, 0, 0),
	(100002, 28, 'Black', 'Pawn', 'g5', 'g4', 'g4', null, null, 0, 0, 0, 0),
	(100002, 29, 'White', 'Queen', 'a2', 'd2', 'Qd2', null, null, 0, 0, 0, 0),
	(100002, 29, 'Black', 'King', 'g8', 'g7', 'Kg7', null, null, 0, 0, 0, 0),
	(100002, 30, 'White', 'Pawn', 'f2', 'f3', 'f3', null, null, 0, 0, 0, 0),
	(100002, 30, 'Black', 'Queen', 'f6', 'd6', 'Qxd6', 'Pawn', null, 0, 0, 0, 0),
	(100002, 31, 'White', 'Pawn', 'f3', 'g4', 'fxg4', 'Pawn', null, 0, 0, 0, 0),
	(100002, 31, 'Black', 'Queen', 'd6', 'd4', 'Qd4+', null, null, 1, 0, 0, 0),
	(100002, 32, 'White', 'King', 'g1', 'h1', 'Kh1', null, null, 0, 0, 0, 0),
	(100002, 32, 'Black', 'Knight', 'd7', 'f6', 'Nf6', null, null, 0, 0, 0, 0),
	(100002, 33, 'White', 'Rook', 'f1', 'f4', 'Rf4', null, null, 0, 0, 0, 0),
	(100002, 33, 'Black', 'Knight', 'f6', 'e4', 'Ne4', null, null, 0, 0, 0, 0),
	(100002, 34, 'White', 'Queen', 'd2', 'd3', 'Qxd3', 'Knight', null, 0, 0, 0, 0),
	(100002, 34, 'Black', 'Knight', 'e4', 'f2', 'Nf2+', null, null, 1, 0, 0, 0),
	(100002, 35, 'White', 'Rook', 'f4', 'f2', 'Rxf2', 'Knight', null, 0, 0, 0, 0),
	(100002, 35, 'Black', 'Bishop', 'g6', 'd3', 'Bxd3', 'Queen', null, 0, 0, 0, 0),
	(100002, 36, 'White', 'Rook', 'f2', 'd2', 'Rfd2', null, null, 0, 0, 0, 0),
	(100002, 36, 'Black', 'Queen', 'd4', 'e3', 'Qe3', null, null, 0, 0, 0, 0),
	(100002, 37, 'White', 'Rook', 'd2', 'd3', 'Rxd3', 'Bishop', null, 0, 0, 0, 0),
	(100002, 37, 'Black', 'Rook', 'c8', 'c1', 'Rc1', null, null, 0, 0, 0, 0),
	(100002, 38, 'White', 'Knight', 'a4', 'b2', 'Nb2', null, null, 0, 0, 0, 0),
	(100002, 38, 'Black', 'Queen', 'e3', 'f2', 'Qf2', null, null, 0, 0, 0, 0),
	(100002, 39, 'White', 'Knight', 'b1', 'd2', 'Nd2', null, null, 0, 0, 0, 0),
	(100002, 39, 'Black', 'Rook', 'c1', 'd1', 'Rxd1+', 'Rook', null, 1, 0, 0, 0),
	(100002, 40, 'White', 'Knight', 'b2', 'd1', 'Nxd1', 'Rook', null, 0, 0, 0, 0),
	(100002, 40, 'Black', 'Rook', 'e8', 'e1', 'Rel+', null, null, 1, 0, 0, 0)
GO

/* ----- Stored Procedures ----- */
/*
	--- User ---
	UserID				[nvarchar](20)	
	GivenName			[nvarchar](50)	
	FamilyName			[nvarchar](100)	
	Email				[nvarchar](100)	
	PasswordHash		[nvarchar](100)	
	Active				[bit]			
*/
print '' print '*** Creating sp_authenticate_user ***'
GO
CREATE PROCEDURE dbo.sp_authenticate_user
(
	@UserID				[nvarchar](20),
	@PasswordHash		[nvarchar](100)
)
AS
	BEGIN
		SELECT COUNT(UserID) AS 'Authenticated'
		FROM [User]
		WHERE @UserID = UserID
			AND @PasswordHash = PasswordHash
			AND Active = 1
	END
GO

print '' print '*** Creating sp_select_user_by_userID ***'
GO
CREATE PROCEDURE dbo.sp_select_user_by_userID
(
	@UserID				[nvarchar](20)
)
AS
	BEGIN
		SELECT 
				UserID,
				GivenName,
				FamilyName,
				Email,
				Active
		FROM
				[User]
		WHERE
				@UserID = UserID
	END
GO

print '' print'*** Creating sp_update_passwordHash ***'
GO
CREATE PROCEDURE dbo.sp_update_passwordHash
(
	@UserID				[nvarchar](20),
	@OldPasswordHash	[nvarchar](100),
	@NewPasswordHash	[nvarchar](100)
)
AS
	BEGIN
		UPDATE [User]
		SET	
			PasswordHash = @NewPasswordHash
		WHERE
			@UserID = UserID
			AND PasswordHash = @OldPasswordHash
		RETURN
			@@ROWCOUNT
	END
GO

print '' print '*** Creating sp_select_user_roles_by_userID ***'
GO
CREATE PROCEDURE dbo.sp_select_user_roles_by_userID
(
	@UserID				[nvarchar](20)
)
AS
	BEGIN
		SELECT	
			RoleID
		FROM
			[UserRole]
		WHERE
			@UserID = UserID
	END
GO

print '' print '*** Creating sp_select_all_user_roles ***'
GO
CREATE PROCEDURE dbo.sp_select_all_user_roles
AS
	BEGIN
		SELECT
			RoleID
		FROM
			[Role]
	END
GO

/*
	--- Game ---
	GameID				[int]
	PlayerWhite			[nvarchar](20)
	WhiteElo			[int]			
	PlayerBlack			[nvarchar](20)	
	BlackElo			[int]			
	ECO					[nchar](3)		
	OpeningVariation	[nvarchar](50)	
	Termination			[nvarchar](20)	
	Outcome				[nvarchar](7)		
	TimeControl			[nvarchar](10)	
	DatePlayed			[date]			
*/
print '' print '*** Creating sp_select_game_by_GameID ***'
GO
CREATE PROCEDURE dbo.sp_select_game_by_GameID
(
	@GameID				[int]
)
AS
	BEGIN
		SELECT
			GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[Game]
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			GameID = @GameID
		ORDER BY DatePlayed DESC
	END
GO

print '' print '*** Creating sp_select_all_games ***'
GO
CREATE PROCEDURE dbo.sp_select_all_games
AS
	BEGIN
		SELECT
			GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[Game]
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		ORDER BY DatePlayed DESC
	END
GO

print '' print '*** Creating sp_select_games_by_elo ***'
GO
CREATE PROCEDURE dbo.sp_select_games_by_elo
(
	@EloFloor			[int],
	@EloCeiling			[int]
)
AS
	BEGIN
		SELECT
			GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[Game]
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			(WhiteElo IN(@EloFloor, @EloCeiling))
		OR 	(BlackElo IN(@EloFloor, @EloCeiling))
		ORDER BY DatePlayed DESC
	END
GO

print '' print '*** Creating sp_select_games_by_outcome ***'
GO
CREATE PROCEDURE dbo.sp_select_games_by_outcome
(
	@Outcome			[nvarchar](7)
)
AS
	BEGIN
		SELECT
			GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[Game]
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			@Outcome = Outcome
		ORDER BY DatePlayed DESC
	END
GO

print '' print '*** Creating sp_select_games_by_ECO ***'
GO
CREATE PROCEDURE dbo.sp_select_games_by_ECO
(
	@ECO				[nchar](3)
)
AS
	BEGIN
		SELECT
			GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[Game]
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			@ECO = [Game].ECO
		ORDER BY OpeningVariation
	END
GO

print '' print '*** Creating sp_select_favorite_games_by_userID ***'
GO
CREATE PROCEDURE dbo.sp_select_favorite_games_by_userID
(
	@UserID				[nvarchar](20)
)
AS
	BEGIN
		SELECT
			[UserGame].GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[UserGame]
		JOIN [Game]
			ON [UserGame].GameID = [Game].GameID
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			@UserID = UserID
	END
GO

print '' print '*** Creating sp_select_favorite_games_by_userID_and_ECO ***'
GO
CREATE PROCEDURE dbo.sp_select_favorite_games_by_userID_and_ECO
(
	@UserID				[nvarchar](20),
	@ECO				[nchar](3)
)
AS
	BEGIN
		SELECT
			[UserGame].GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name,
			OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
		FROM
			[UserGame]
		JOIN [Game]
			ON [UserGame].GameID = [Game].GameID
		JOIN
			[Opening]
		ON
			[Game].ECO = [Opening].ECO
		WHERE
			@UserID = UserID AND
			@ECO = [Game].ECO
	END
GO

print '' print '*** Creating sp_insert_user_favorite_game ***'
GO
CREATE PROCEDURE dbo.sp_insert_user_favorite_game
(
	@UserID				[nvarchar](20),
	@GameID				[int]
)
AS
	BEGIN
		INSERT INTO [UserGame] 
			(UserID, GameID)
		VALUES
			(@UserID, @GameID)
		RETURN @@ROWCOUNT
	END
GO

print '' print '*** Creating sp_delete_user_favorite_game ***'
GO
CREATE PROCEDURE dbo.sp_delete_user_favorite_game
(
	@UserID				[nvarchar](20),
	@GameID				[int]
)
AS
	BEGIN
		DELETE FROM [UserGame]
		WHERE
			@UserID = UserID AND
			@GameID = GameID
		RETURN @@ROWCOUNT
	END
GO

print '' print '*** Creating sp_update_game ***'
GO
CREATE PROCEDURE dbo.sp_update_game
(
	@GameID					[int],
	@OldPlayerWhite			[nvarchar](20),
	@OldWhiteElo			[int],
	@OldPlayerBlack			[nvarchar](20),
	@OldBlackElo			[int],
	@OldECO					[nchar](3),
	@OldOpeningVariation	[nvarchar](50),
	@OldTermination			[nvarchar](20),
	@OldOutcome				[nvarchar](7),
	@OldTimeControl			[nvarchar](10),
	@OldDatePlayed			[date],
	
	@NewPlayerWhite			[nvarchar](20),
	@NewWhiteElo			[int],
	@NewPlayerBlack			[nvarchar](20),
	@NewBlackElo			[int],
	@NewECO					[nchar](3),
	@NewOpeningVariation	[nvarchar](50),
	@NewTermination			[nvarchar](20),
	@NewOutcome				[nvarchar](7),
	@NewTimeControl			[nvarchar](10),
	@NewDatePlayed			[date]
)
AS
	BEGIN
		UPDATE [Game]
		SET
			PlayerWhite	= @NewPlayerWhite,
			WhiteElo = @NewWhiteElo,
			PlayerBlack = @NewPlayerBlack,
			BlackElo = @NewBlackElo,
			ECO = @NewECO,
			OpeningVariation = @NewOpeningVariation,
			Termination = @NewTermination,
			Outcome	= @NewOutcome,
			TimeControl = @NewTimeControl,
			DatePlayed = @NewDatePlayed
		WHERE
			@GameID = GameID AND
			@OldPlayerWhite	= PlayerWhite AND
			@OldWhiteElo = WhiteElo AND
			@OldPlayerBlack = PlayerBlack AND
			@OldBlackElo = BlackElo AND
			@OldECO = ECO AND
			@OldOpeningVariation = OpeningVariation AND
			@OldTermination = Termination AND
			@OldOutcome	= Outcome AND
			@OldTimeControl = TimeControl AND
			@OldDatePlayed = DatePlayed			
		RETURN @@ROWCOUNT
	END
GO

print '' print '*** Creating sp_delete_game ***'
GO
CREATE PROCEDURE sp_delete_game
(
	@GameID			[int]
)
AS
	BEGIN
		DELETE FROM
			[GameMove]
		WHERE
			@GameID = GameID
		;
		
		DELETE FROM
			[UserGame]
		WHERE
			@GameID = GameID
		;
		
		DELETE FROM
			[Game]
		WHERE
			@GameID = GameID
		;
		RETURN @@ROWCOUNT
	END
GO

/*
	--- GameMove ---
	GameID				[int]			
	Turn				[int]			
	Color				[nchar](5)		
	Piece				[nvarchar](6)	
	Notation			[nvarchar](10)	
	Origin				[nchar](2)		
	Destination			[nchar](2)		
	Capture				[nvarchar](6)	
	Promotion			[nvarchar](6)	
	IsCheck				[bit]			
	IsMate				[bit]			
	IsCastle			[bit]			
	IsEnPassant			[bit]			
*/
print '' print '*** Creating sp_select_moves_by_gameID ***'
GO
CREATE PROCEDURE dbo.sp_select_moves_by_gameID
(
	@GameID				[int]
)
AS
	BEGIN
		SELECT
			GameID, Turn, Color, Piece, Notation, Origin, Destination,
			Capture, Promotion, IsCheck, IsCastle, IsEnPassant
		FROM
			[GameMove]
		WHERE
			@GameID = GameID
		ORDER BY Turn ASC, Color DESC
	END
GO

/*
	--- Opening ---
	ECO					[nchar](3)		
	Name				[nvarchar](50)
	
	--- OpeningVariation ---
	VariationID			[int] IDENTITY(100000,1)
	Name				[nvarchar](50)	
	ECO					[nchar](3)		
*/
print '' print '*** Creating sp_select_all_openings ***'
GO
CREATE PROCEDURE dbo.sp_select_all_openings
AS
	BEGIN
		SELECT
			ECO, Name
		FROM [Opening]
		ORDER BY ECO
	END
GO

print '' print '*** Creating sp_select_all_variations ***'
GO
CREATE PROCEDURE dbo.sp_select_all_variations
AS
	BEGIN
		SELECT
			[OpeningVariation].ECO,
			[Opening].Name,
			[OpeningVariation].Name
		FROM [OpeningVariation]
		JOIN [Opening]
			ON [OpeningVariation].ECO = [Opening].ECO
		ORDER BY [OpeningVariation].ECO, [OpeningVariation].Name
	END
GO

print '' print '*** Creating sp_select_variations_by_ECO ***'
GO
CREATE PROCEDURE dbo.sp_select_variations_by_ECO
(
	@ECO				[nchar](3)
)
AS
	BEGIN
		SELECT
			VariationID, Name, ECO
		FROM
			[OpeningVariation]
		WHERE
			@ECO = ECO
		ORDER BY Name
	END
GO

print '' print '*** Creating sp_insert_opening_variation ***'
GO
CREATE PROCEDURE dbo.sp_insert_opening_variation
(
	@ECO				[nchar](3),
	@Name				[nvarchar](50)
)
AS
	BEGIN
		INSERT INTO [OpeningVariation]
			(Name, ECO)
		VALUES
			(@Name, @ECO)
		RETURN
			@@ROWCOUNT
	END
GO