

USE GridHeistDB2;
GO


-- DROP EXISTING TABLES 


IF OBJECT_ID('dbo.ChatMessages', 'U') IS NOT NULL DROP TABLE dbo.ChatMessages;
IF OBJECT_ID('dbo.PlayerInventory', 'U') IS NOT NULL DROP TABLE dbo.PlayerInventory;
IF OBJECT_ID('dbo.GameItems', 'U') IS NOT NULL DROP TABLE dbo.GameItems;
IF OBJECT_ID('dbo.NPCPath', 'U') IS NOT NULL DROP TABLE dbo.NPCPath;
IF OBJECT_ID('dbo.NPCs', 'U') IS NOT NULL DROP TABLE dbo.NPCs;
IF OBJECT_ID('dbo.GamePlayers', 'U') IS NOT NULL DROP TABLE dbo.GamePlayers;
IF OBJECT_ID('dbo.GameTiles', 'U') IS NOT NULL DROP TABLE dbo.GameTiles;
IF OBJECT_ID('dbo.Games', 'U') IS NOT NULL DROP TABLE dbo.Games;
IF OBJECT_ID('dbo.ItemTypes', 'U') IS NOT NULL DROP TABLE dbo.ItemTypes;
IF OBJECT_ID('dbo.Players', 'U') IS NOT NULL DROP TABLE dbo.Players;
GO


-- TABLE 1: Players

CREATE TABLE dbo.Players
(
    PlayerID              INT IDENTITY(1,1) NOT NULL,
    PlayerName            VARCHAR(50)       NOT NULL,
    PasswordHash          VARCHAR(255)      NOT NULL,
    FailedLoginAttempts   INT               NOT NULL  CONSTRAINT DF_Players_FailedAttempts DEFAULT 0,
    IsLockedOut           BIT               NOT NULL  CONSTRAINT DF_Players_IsLockedOut    DEFAULT 0,
    IsOnline              BIT               NOT NULL  CONSTRAINT DF_Players_IsOnline       DEFAULT 0,
    IsAdmin               BIT               NOT NULL  CONSTRAINT DF_Players_IsAdmin        DEFAULT 0,
    HighestScore          INT               NOT NULL  CONSTRAINT DF_Players_HighestScore   DEFAULT 0,
    CreatedAt             DATETIME2         NOT NULL  CONSTRAINT DF_Players_CreatedAt      DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Players            PRIMARY KEY (PlayerID),
    CONSTRAINT UQ_Players_PlayerName UNIQUE (PlayerName)
);
GO

-- TABLE 2: Games

CREATE TABLE dbo.Games
(
    GameID              INT IDENTITY(1,1) NOT NULL,
    CreatedByPlayerID   INT               NULL,
    GameStatus          VARCHAR(20)       NOT NULL  CONSTRAINT DF_Games_Status     DEFAULT 'Active',
    MaxPlayers          INT               NOT NULL  CONSTRAINT DF_Games_MaxPlayers DEFAULT 4,
    TimeLimitSeconds    INT               NOT NULL  CONSTRAINT DF_Games_TimeLimit  DEFAULT 900,
    StartTime           DATETIME2         NULL,
    EndTime             DATETIME2         NULL,

    CONSTRAINT PK_Games                PRIMARY KEY (GameID),
    CONSTRAINT FK_Games_CreatedByPlayer FOREIGN KEY (CreatedByPlayerID) REFERENCES dbo.Players(PlayerID),
    CONSTRAINT CK_Games_Status         CHECK (GameStatus IN ('Active', 'Completed', 'Stopped'))
);
GO


-- TABLE 3: ItemTypes

CREATE TABLE dbo.ItemTypes
(
    ItemTypeID   INT IDENTITY(1,1) NOT NULL,
    ItemName     VARCHAR(50)       NOT NULL,
    ItemEffect   VARCHAR(100)      NULL,

    CONSTRAINT PK_ItemTypes PRIMARY KEY (ItemTypeID)
);
GO


-- TABLE 4: GameTiles

CREATE TABLE dbo.GameTiles
(
    TileID        INT IDENTITY(1,1) NOT NULL,
    GameID        INT               NOT NULL,
    RowNumber     INT               NOT NULL,
    ColumnNumber  INT               NOT NULL,
    TileType      VARCHAR(20)       NOT NULL  CONSTRAINT DF_GameTiles_Type     DEFAULT 'Floor',
    IsWalkable    BIT               NOT NULL  CONSTRAINT DF_GameTiles_Walkable DEFAULT 1,

    CONSTRAINT PK_GameTiles          PRIMARY KEY (TileID),
    CONSTRAINT FK_GameTiles_Game     FOREIGN KEY (GameID) REFERENCES dbo.Games(GameID),
    CONSTRAINT UQ_GameTiles_Position UNIQUE (GameID, RowNumber, ColumnNumber)
);
GO


-- TABLE 5: GamePlayers

CREATE TABLE dbo.GamePlayers
(
    GamePlayerID   INT IDENTITY(1,1) NOT NULL,
    GameID         INT               NOT NULL,
    PlayerID       INT               NOT NULL,
    CurrentTileID  INT               NOT NULL,
    Score          INT               NOT NULL  CONSTRAINT DF_GamePlayers_Score    DEFAULT 0,
    PlayerStatus   VARCHAR(20)       NOT NULL  CONSTRAINT DF_GamePlayers_Status   DEFAULT 'Playing',
    IsWinner       BIT               NOT NULL  CONSTRAINT DF_GamePlayers_IsWinner DEFAULT 0,

    CONSTRAINT PK_GamePlayers        PRIMARY KEY (GamePlayerID),
    CONSTRAINT FK_GamePlayers_Game   FOREIGN KEY (GameID)        REFERENCES dbo.Games(GameID),
    CONSTRAINT FK_GamePlayers_Player FOREIGN KEY (PlayerID)      REFERENCES dbo.Players(PlayerID),
    CONSTRAINT FK_GamePlayers_Tile   FOREIGN KEY (CurrentTileID) REFERENCES dbo.GameTiles(TileID),
    CONSTRAINT CK_GamePlayers_Status CHECK (PlayerStatus IN ('Playing', 'Finished', 'Left'))
);
GO

-- TABLE 6: GameItems

CREATE TABLE dbo.GameItems
(
    GameItemID    INT IDENTITY(1,1) NOT NULL,
    GameID        INT               NOT NULL,
    ItemTypeID    INT               NOT NULL,
    CurrentTileID INT               NULL,
    ItemStatus    VARCHAR(20)       NOT NULL  CONSTRAINT DF_GameItems_Status DEFAULT 'OnTile',

    CONSTRAINT PK_GameItems          PRIMARY KEY (GameItemID),
    CONSTRAINT FK_GameItems_Game     FOREIGN KEY (GameID)        REFERENCES dbo.Games(GameID),
    CONSTRAINT FK_GameItems_ItemType FOREIGN KEY (ItemTypeID)    REFERENCES dbo.ItemTypes(ItemTypeID),
    CONSTRAINT FK_GameItems_Tile     FOREIGN KEY (CurrentTileID) REFERENCES dbo.GameTiles(TileID),
    CONSTRAINT CK_GameItems_Status   CHECK (ItemStatus IN ('OnTile', 'InInventory', 'Used'))
);
GO


-- TABLE 7: PlayerInventory

CREATE TABLE dbo.PlayerInventory
(
    InventoryID   INT IDENTITY(1,1) NOT NULL,
    GamePlayerID  INT               NOT NULL,
    GameItemID    INT               NOT NULL,
    CollectedAt   DATETIME2         NOT NULL  CONSTRAINT DF_Inventory_CollectedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_PlayerInventory        PRIMARY KEY (InventoryID),
    CONSTRAINT FK_Inventory_GamePlayer   FOREIGN KEY (GamePlayerID) REFERENCES dbo.GamePlayers(GamePlayerID),
    CONSTRAINT FK_Inventory_GameItem     FOREIGN KEY (GameItemID)   REFERENCES dbo.GameItems(GameItemID)
);
GO


-- TABLE 8: NPCs

CREATE TABLE dbo.NPCs
(
    NPCID            INT IDENTITY(1,1) NOT NULL,
    GameID           INT               NOT NULL,
    NPCName          VARCHAR(50)       NOT NULL,
    CurrentTileID    INT               NOT NULL,
    IsMovingForward  BIT               NOT NULL  CONSTRAINT DF_NPCs_Moving DEFAULT 1,

    CONSTRAINT PK_NPCs      PRIMARY KEY (NPCID),
    CONSTRAINT FK_NPCs_Game FOREIGN KEY (GameID)        REFERENCES dbo.Games(GameID),
    CONSTRAINT FK_NPCs_Tile FOREIGN KEY (CurrentTileID) REFERENCES dbo.GameTiles(TileID)
);
GO


-- TABLE 9: NPCPath

CREATE TABLE dbo.NPCPath
(
    NPCPathID   INT IDENTITY(1,1) NOT NULL,
    NPCID       INT               NOT NULL,
    TileID      INT               NOT NULL,
    PathOrder   INT               NOT NULL,

    CONSTRAINT PK_NPCPath      PRIMARY KEY (NPCPathID),
    CONSTRAINT FK_NPCPath_NPC  FOREIGN KEY (NPCID)  REFERENCES dbo.NPCs(NPCID),
    CONSTRAINT FK_NPCPath_Tile FOREIGN KEY (TileID)  REFERENCES dbo.GameTiles(TileID)
);
GO


-- TABLE 10: ChatMessages

CREATE TABLE dbo.ChatMessages
(
    ChatMessageID  INT IDENTITY(1,1) NOT NULL,
    GamePlayerID   INT               NOT NULL,
    MessageText    VARCHAR(500)      NOT NULL,
    SentAt         DATETIME2         NOT NULL  CONSTRAINT DF_Chat_SentAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_ChatMessages     PRIMARY KEY (ChatMessageID),
    CONSTRAINT FK_Chat_GamePlayer  FOREIGN KEY (GamePlayerID) REFERENCES dbo.GamePlayers(GamePlayerID)
);
GO

