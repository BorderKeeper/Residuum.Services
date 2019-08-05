# Residuum.Services
World of Warcraft Guild Website Backend

This is a backend for a simple one-page World of Warcraft guild website, handles connections to the new blizzard API and raider IO API.
Now uses Entity Database (code-first aproach)

Showcase: www.residuum.cz (page is a modification of https://github.com/mdamyanova/ella)
Available service endpoints: 
- GET /api/raidprogress (Displays filtered raid json list of your guild)
- GET /api/guildroster (Displays members with rank X and up, along with their best mythic score)

Installation:
- Obtain ClientId and Secret from Blizzard API site
- Fill appsettings.json file with those plus with a connectionstring to your DB
- Make sure you run Add-Migration to create scripts and Update-Database to trigger them on your DB (if you are on azure publish settings contain toggle to do that for you)

Limitations:
- For simplicity this page does not use any database and stores its data for 60 minutes in a static cache class
- Utilizes ArgentPonyWarcraftClient nuget library for Blizzard API OAuth connection and Service calls
