##Goal – Create OData Enpoints for IMDB Data
IMDB Movie and Actor data is readily available as text files at .  My goal was to build an OData web api on a Mac (not windows) only using .net core (2.0.)  I ended up using Entity Framework (Code First) to build the database.  I needed an IQueryable model for the OData Endpoints, and when I built the project, type providers for F# (eg. SQL Provider) were still under construction.   I did, however, manage to use a CSV provider to parse the text files.  I used F# to seed the database.    So, in a nutshell, there are three projects in this repor
1. Data Access – Creates Entity Framework entities
2. Data Migration – downloads csv files from the IMDB site and uploads the data to an sql server database.  For this project, I ended up using an sql-server docker container that ran locally on my mac.
3. IMDB OData Web API – the api that contains the OData Endpoints
 
##How to run this
1. You need .net core 2.0 (later, I may package more neatly via Docker)
2. Set up an SQL Server DB on your local machine
o	You can get it from docker
•	Install Docker
•	Download an SQL Server Docker Image
sudo docker pull microsoft/mssql-server-linux:2017-latest
•	Create a container from the image
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=1LoveProgramming -p 1433:1433 -d microsoft/mssql-server-linux:2017-CU4

Feel free to replace the asterix with your password.  Consider using the Kitematic tool (google it) to manage your container. 

Using your tool of choice, create an IMDB database in your SQL Server (i.e. CREATE DATABASE IMDB)
3. cd into the DataAccess Directory
o	Create a Database named IMDB in your SQL Server Instance. Using your favorite tool, just type CREATE DATABASE IMDB 
o	Create the required Tables/Relationsips in Database by typing:
dotnet ef database update
(Note: The Initial Data Migration has already been included in the file, so you don’t have to include the ‘Add Migration…’ command.  Also not, if you get an error, you probably don’t have the correct .net core cli tools available)
4. In the project, change the contents of all access.json files to reflect the data (e.g. user/password) of your sql server database
5. Set imdbOdataWebApi as your startup project
o	Build and run using a tool like Visual Studio for Mac

6. Open a tool like Google PostMan, and start issuing http request to your endpoints. Sample OData queries proceed:


Sample Queries #1: Get 2005 movies that are at least 150 minutes long
http://localhost:5000/odata/Media?$filter=startYear eq 2005 and runtimeMinutes ge 150&$expand=Genres&$orderby=runtimeMinutes desc

Sample Query #2: Find the 100 top rated movies in 2010. Make sure the movies have at least 1000 votes
http://localhost:5000/odata/Media?$filter=startYear eq 2010 and runtimeMinutes ge 150&$expand=Rating

Sample Query #3: Find movies with a start year of 2005 that are at least 150 minutes long whose Ratings have at least 10 votes. Show the ratings and order by movie duration
http://localhost:5000/odata/Media?$filter=startYear eq 2005 and runtimeMinutes ge 150 and Rating/NumVotes ge 10&$expand=Rating&$orderby=runtimeMinutes desc

Sample Queries: Find all actors/actresses with a name like 'Denzel'. List 4 movies
that they are known for.
http://localhost:5000/odata/Staff?$filter=contains(PrimaryName, 'Denzel')&$expand=KnownForTitles($expand=AssociatedMedia)
