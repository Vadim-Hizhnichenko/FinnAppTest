# Market Asset Price API

This project is an API service that provides price information for specific market assets (e.g., EUR/USD, GOOG, etc.).

## Requirements

- .NET 7.0 SDK
- Docker (optional, for containerized deployment)

## Running the Application

### Using .NET CLI

1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001` and `http://localhost:5000`.

### Using Docker

1. Clone the repository
2. Navigate to the project directory
3. Build the Docker image:

```
docker build -t market-asset-api .
```

4. Run the Docker container:

```
docker run -p 8080:80 market-asset-api
```

The API will be available at `http://localhost:8080`.

## API Endpoints

- GET `/api/marketasset`: Get a list of supported market assets.
- GET `/api/marketasset/{symbol}`: Get price information for a specific asset.

## Configuration

The application uses the following configuration values, which can be found in `appsettings.json`:

- `Fintacharts:URI`: The base URI for the Fintacharts API
- `Fintacharts:URI_WSS`: The WebSocket URI for real-time updates
- `Fintacharts:USERNAME`: The username for authentication
- `Fintacharts:PASSWORD`: The password for authentication

Ensure these values are correctly set before running the application.
