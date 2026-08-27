# DAT602 Assignment 1: Grid Heist

This repository contains the source code, database scripts, and supporting documentation for the "Grid Heist" project, a prototype for a database-driven game application developed for the DAT602 Database Application Development course.

## Project Overview

"Grid Heist" is a multi-user, turn-based, "point-and-click" game prototype. The project's central architectural principle is that all game logic, state management, and rules are implemented and enforced directly within a SQL Server database via T-SQL stored procedures. The C# .NET Windows Forms application serves as a "dumb client," responsible only for rendering the game state provided by the database and sending user actions back for processing.

This database-centric design ensures data integrity, consistency across multiple game clients, and a robust separation of concerns between the application's logic and its presentation layer.

## Key Features

- **Database-Centric Architecture:** All game rules, player moves, NPC behavior, and state changes are handled authoritatively by the database.
- **User Authentication & Management:** Full prototype functionality for player registration, secure login validation, and account lockout logic.
- **Dynamic Gameplay Prototype:** The design supports a 2-player, turn-based system where players collect items and interact with each other on a grid.
- **Database-Driven NPCs:** The game includes two types of Non-Playing Characters ("Patrol Bots" and "Glitches") whose movement and actions are determined entirely by stored procedures, adding dynamic challenges to the game board.
- **Administrator Panel:** A dedicated interface for administrators to manage users and game sessions, demonstrating CRUD (Create, Read, Update, Delete) operations.
- **Structured C# Prototype:** The application is built using a clean, three-form structure (Login, Game, Admin) and utilizes a Data Access Object (DAO) pattern to centralize all database communication.

## Repository Contents

- 📁 **GridHeistApplication/** - Contains the complete C# .NET source code and the Visual Studio solution file (.sln).
- 📁 **SQL Scripts/** - Contains all T-SQL scripts required to create and populate the database.
  - **InitializeDatabase.sql** - A single, master script that creates all tables, constraints, and inserts comprehensive test data.
  - **StoredProcedures.sql** - Contains the specific stored procedures called by the C# application prototype.
- 📄 **Grid_Heist_Assignment_1_Report.docx** - The final, complete written report for Milestone One, covering game design, ERD, CRUD analysis, and all other required documentation.
- 📄 **README.md** - This file.

## Setup and Running the Project

To run this project, you will need Microsoft SQL Server (with SSMS) and Visual Studio.

### Part 1: Database Setup

1. **Create the Database:** In SQL Server Management Studio (SSMS), create a new, empty database named `GridHeistDB`.

2. **Run the Initialization Script:** Open the `SQL Scripts/InitializeDatabase.sql` file in SSMS. Ensure you are connected to the `GridHeistDB` database, and execute the entire script. This will create all tables, stored procedures, and test data.

3. **Run the Application Procedures Script:** Open the `SQL Scripts/StoredProcedures.sql` file in SSMS. Ensure you are connected to the `GridHeistDB` database, and execute the entire script. This will create the procedures that the C# application calls.

### Part 2: Application Setup

1. **Open the Project:** Open the `GridHeistApplication.sln` file in Visual Studio.

2. **IMPORTANT - Update Connection String:**
   - Navigate to the `DataAccess/DatabaseAccessor.cs` file.
   - On line 14, you will find the `connectionString` variable.
   - You must replace the placeholder string with the correct connection string for your local SQL Server instance. It will look similar to this:
   ```csharp
   private readonly string connectionString = "Server=DESKTOP-ABC123\\SQLEXPRESS;Database=GridHeistDB;Integrated Security=True;";
   ```

3. **Run the Application:** Press F5 or click the "Start" button in Visual Studio to build and run the project.

The login screen will appear. You can test the application using the pre-populated test user:
- **Username:** Siyara
- **Password:** hash123_placeholder

