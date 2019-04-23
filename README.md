# Residuum.Services
World of Warcraft Guild Website Backend

This is a backend for a simple one-page World of Warcraft guild website, handles connections to the new blizzard API and raider IO API.

Showcase: www.residuum.cz
Available service endpoints: 
- GET /api/raidprogress (Displays filtered raid json list of your guild)
- GET /api/guildroster (Displays members with rank X and up, along with their best mythic score)

Installation:
- Obtain ClientId and Secret from Blizzard API site
- Fill appsettings.json file

Limitations:
- For simplicity this page does not use any database and stores its data for 60 minutes in a static cache class
- Utilizes ArgentPonyWarcraftClient nuget library for Blizzard API OAuth connection and Service calls
