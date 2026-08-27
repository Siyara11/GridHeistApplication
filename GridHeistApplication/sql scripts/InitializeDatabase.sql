
USE GridHeistDB;
GO

/*
    DAT602 - Assignment 1: Grid Heist
    Database Initialization Script

    This script:
    1. Creates/recreates the stored procedure sp_InitializeDatabase.
    2. Drops existing tables in dependency order.
    3. Recreates all database tables.
    4. Inserts test data.
    5. Executes the initialization procedure.
*/


/* ============================================================
   1. REMOVE EXISTING STORED PROCEDURE
   ============================================================ */

IF OBJECT_ID('dbo.sp_InitializeDatabase', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_InitializeDatabase;
GO


/* ============================================================
   2. CREATE INITIALIZATION STORED PROCEDURE
   ============================================================ */

CREATE PROCEDURE dbo.sp_InitializeDatabase
AS
BEGIN
    SET NOCOUNT ON;

    PRINT '============================================================';
    PRINT 'Starting Grid Heist database initialization...';
    PRINT '============================================================';


    /* ========================================================
       2.1 DROP EXISTING TABLES
       Drop child/dependent tables before parent tables.
       ======================================================== */

    PRINT 'Dropping existing tables...';

    IF OBJECT_ID('dbo.PlayerInventory', 'U') IS NOT NULL
        DROP TABLE dbo.PlayerInventory;

    IF OBJECT_ID('dbo.ItemLocations', 'U') IS NOT NULL
        DROP TABLE dbo.ItemLocations;

    IF OBJECT_ID('dbo.ActiveGames', 'U') IS NOT NULL
        DROP TABLE dbo.ActiveGames;

    IF OBJECT_ID('dbo.NPCs', 'U') IS NOT NULL
        DROP TABLE dbo.NPCs;

    IF OBJECT_ID('dbo.Players', 'U') IS NOT NULL
        DROP TABLE dbo.Players;

    IF OBJECT_ID('dbo.Items', 'U') IS NOT NULL
        DROP TABLE dbo.Items;

    IF OBJECT_ID('dbo.Tiles', 'U') IS NOT NULL
        DROP TABLE dbo.Tiles;

    PRINT 'Existing tables dropped successfully.';


    /* ========================================================
       2.2 CREATE PLAYERS TABLE
       ======================================================== */

    CREATE TABLE dbo.Players
    (
        PlayerID INT IDENTITY(1,1) NOT NULL,
        PlayerName VARCHAR(50) NOT NULL,
        PasswordHash VARCHAR(255) NOT NULL,
        IsLockedOut BIT NOT NULL
            CONSTRAINT DF_Players_IsLockedOut DEFAULT 0,

        CONSTRAINT PK_Players
            PRIMARY KEY (PlayerID),

        CONSTRAINT UQ_Players_PlayerName
            UNIQUE (PlayerName)
    );

    PRINT 'Players table created.';


    /* ========================================================
       2.3 CREATE ITEMS TABLE
       ======================================================== */

    CREATE TABLE dbo.Items
    (
        ItemID INT IDENTITY(1,1) NOT NULL,
        ItemName VARCHAR(50) NOT NULL,
        ItemType VARCHAR(50) NULL,

        CONSTRAINT PK_Items
            PRIMARY KEY (ItemID)
    );

    PRINT 'Items table created.';


    /* ========================================================
       2.4 CREATE TILES TABLE
       ======================================================== */

    CREATE TABLE dbo.Tiles
    (
        TileID INT IDENTITY(1,1) NOT NULL,
        X_Coordinate INT NOT NULL,
        Y_Coordinate INT NOT NULL,

        CONSTRAINT PK_Tiles
            PRIMARY KEY (TileID),

        CONSTRAINT UQ_Tiles_Coordinates
            UNIQUE (X_Coordinate, Y_Coordinate)
    );

    PRINT 'Tiles table created.';


    /* ========================================================
       2.5 CREATE NPCS TABLE
       ======================================================== */

    CREATE TABLE dbo.NPCs
    (
        NpcID INT IDENTITY(1,1) NOT NULL,
        NpcType VARCHAR(50) NOT NULL,
        CurrentTileID INT NULL,
        PatrolStartTileID INT NULL,
        PatrolEndTileID INT NULL,
        IsMovingForward BIT NULL,

        CONSTRAINT PK_NPCs
            PRIMARY KEY (NpcID),

        CONSTRAINT FK_NPCs_CurrentTile
            FOREIGN KEY (CurrentTileID)
            REFERENCES dbo.Tiles(TileID),

        CONSTRAINT FK_NPCs_PatrolStartTile
            FOREIGN KEY (PatrolStartTileID)
            REFERENCES dbo.Tiles(TileID),

        CONSTRAINT FK_NPCs_PatrolEndTile
            FOREIGN KEY (PatrolEndTileID)
            REFERENCES dbo.Tiles(TileID)
    );

    PRINT 'NPCs table created.';


    /* ========================================================
       2.6 CREATE ACTIVEGAMES TABLE
       ======================================================== */

    CREATE TABLE dbo.ActiveGames
    (
        GameID INT IDENTITY(1,1) NOT NULL,
        Player1_ID INT NOT NULL,
        Player2_ID INT NOT NULL,
        Player1_TileID INT NOT NULL,
        Player2_TileID INT NOT NULL,
        CurrentTurn_PlayerID INT NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_ActiveGames_IsActive DEFAULT 1,

        CONSTRAINT PK_ActiveGames
            PRIMARY KEY (GameID),

        CONSTRAINT FK_ActiveGames_Player1
            FOREIGN KEY (Player1_ID)
            REFERENCES dbo.Players(PlayerID),

        CONSTRAINT FK_ActiveGames_Player2
            FOREIGN KEY (Player2_ID)
            REFERENCES dbo.Players(PlayerID),

        CONSTRAINT FK_ActiveGames_Player1Tile
            FOREIGN KEY (Player1_TileID)
            REFERENCES dbo.Tiles(TileID),

        CONSTRAINT FK_ActiveGames_Player2Tile
            FOREIGN KEY (Player2_TileID)
            REFERENCES dbo.Tiles(TileID),

        CONSTRAINT FK_ActiveGames_CurrentTurnPlayer
            FOREIGN KEY (CurrentTurn_PlayerID)
            REFERENCES dbo.Players(PlayerID)
    );

    PRINT 'ActiveGames table created.';


    /* ========================================================
       2.7 CREATE PLAYERINVENTORY TABLE
       ======================================================== */

    CREATE TABLE dbo.PlayerInventory
    (
        PlayerID INT NOT NULL,
        ItemID INT NOT NULL,
        Quantity INT NOT NULL
            CONSTRAINT DF_PlayerInventory_Quantity DEFAULT 1,

        CONSTRAINT PK_PlayerInventory
            PRIMARY KEY (PlayerID, ItemID),

        CONSTRAINT FK_PlayerInventory_Player
            FOREIGN KEY (PlayerID)
            REFERENCES dbo.Players(PlayerID),

        CONSTRAINT FK_PlayerInventory_Item
            FOREIGN KEY (ItemID)
            REFERENCES dbo.Items(ItemID),

        CONSTRAINT CK_PlayerInventory_Quantity
            CHECK (Quantity > 0)
    );

    PRINT 'PlayerInventory table created.';


    /* ========================================================
       2.8 CREATE ITEMLOCATIONS TABLE

       TileID is the primary key.
       Therefore each tile can contain at most one item.
       ======================================================== */

    CREATE TABLE dbo.ItemLocations
    (
        TileID INT NOT NULL,
        ItemID INT NOT NULL,

        CONSTRAINT PK_ItemLocations
            PRIMARY KEY (TileID),

        CONSTRAINT FK_ItemLocations_Tile
            FOREIGN KEY (TileID)
            REFERENCES dbo.Tiles(TileID),

        CONSTRAINT FK_ItemLocations_Item
            FOREIGN KEY (ItemID)
            REFERENCES dbo.Items(ItemID)
    );

    PRINT 'ItemLocations table created.';


    /* ========================================================
       3. INSERT TEST DATA
       ======================================================== */

    PRINT 'Inserting test data...';


    /* --------------------------------------------------------
       3.1 PLAYERS
       -------------------------------------------------------- */

    INSERT INTO dbo.Players
    (
        PlayerName,
        PasswordHash,
        IsLockedOut
    )
    VALUES
    ('Siyara', 'hash123_placeholder', 0),
    ('OpponentX', 'hash456_placeholder', 0);

    PRINT '2 players inserted.';


    /* --------------------------------------------------------
       3.2 ITEMS
       -------------------------------------------------------- */

    INSERT INTO dbo.Items
    (
        ItemName,
        ItemType
    )
    VALUES
    ('Gem', 'Collectible'),
    ('Trap', 'Hazard');

    PRINT '2 items inserted.';


    /* --------------------------------------------------------
       3.3 TILES
       Creates a 3 x 3 grid.
       -------------------------------------------------------- */

    INSERT INTO dbo.Tiles
    (
        X_Coordinate,
        Y_Coordinate
    )
    VALUES
    (1, 1),
    (1, 2),
    (1, 3),
    (2, 1),
    (2, 2),
    (2, 3),
    (3, 1),
    (3, 2),
    (3, 3);

    PRINT '9 tiles inserted.';


    /* --------------------------------------------------------
       3.4 NPCS

       PatrolBot:
       Current tile = 2
       Patrol start = 1
       Patrol end = 3

       Glitch:
       Current tile = 7
       No patrol route
       -------------------------------------------------------- */

    INSERT INTO dbo.NPCs
    (
        NpcType,
        CurrentTileID,
        PatrolStartTileID,
        PatrolEndTileID,
        IsMovingForward
    )
    VALUES
    ('PatrolBot', 2, 1, 3, 1),
    ('Glitch', 7, NULL, NULL, NULL);

    PRINT '2 NPCs inserted.';


    /* --------------------------------------------------------
       3.5 ACTIVE GAME

       Player 1 = Siyara
       Player 2 = OpponentX
       Player 1 tile = Tile 1
       Player 2 tile = Tile 5
       Current turn = Siyara
       -------------------------------------------------------- */

    INSERT INTO dbo.ActiveGames
    (
        Player1_ID,
        Player2_ID,
        Player1_TileID,
        Player2_TileID,
        CurrentTurn_PlayerID,
        IsActive
    )
    VALUES
    (1, 2, 1, 5, 1, 1);

    PRINT '1 active game inserted.';


    /* --------------------------------------------------------
       3.6 PLAYER INVENTORY

       Siyara starts with one Gem.
       -------------------------------------------------------- */

    INSERT INTO dbo.PlayerInventory
    (
        PlayerID,
        ItemID,
        Quantity
    )
    VALUES
    (1, 1, 1);

    PRINT 'Player inventory inserted.';


    /* --------------------------------------------------------
       3.7 ITEM LOCATIONS

       Gem placed on Tile 3 and Tile 9.
       Since TileID is the PK, each tile contains
       at most one item.
       -------------------------------------------------------- */

    INSERT INTO dbo.ItemLocations
    (
        TileID,
        ItemID
    )
    VALUES
    (3, 1),
    (9, 1);

    PRINT '2 item locations inserted.';


    /* ========================================================
       4. COMPLETION MESSAGE
       ======================================================== */

    PRINT '';
    PRINT '============================================================';
    PRINT 'Grid Heist database initialization completed successfully.';
    PRINT '============================================================';

    SET NOCOUNT OFF;
END;
GO


/* ============================================================
   5. EXECUTE STORED PROCEDURE
   ============================================================ */

EXEC dbo.sp_InitializeDatabase;
GO

