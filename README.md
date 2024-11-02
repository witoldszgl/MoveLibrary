# MoviesLibrary

MoviesLibrary is a web application made in Vue.js. It allows users to edit an SQLite database (movie list) by communicating with a C# .NET API.

## Installation and startup

### Server API
To start the server API:

1. Navigate to the `MovieLibraryAPI` folder.
2. Open PowerShell and run the following commands:

   ```sh
   dotnet restore
   dotnet run
   ```

### Web Application
To start the web application:

1. Navigate to the `Web` folder.
2. Open PowerShell and run the following commands:

   ```sh
   npm install
   npm run serve
   ```

After running both the API and the web application, the app should be accessible under the URL: [http://localhost:8080](http://localhost:8080).
