# VivaApiProject

A .NET 9 Web API project that handles country data with caching, database storage, and external API integration.

##  Setup

1. **Create the database**  
   ```sql
   CREATE DATABASE VivaDb;
Apply migrations

dotnet ef migrations add InitialCreate
dotnet ef database update

Project Structure
Controllers

NumbersController.cs: Handles number-related logic

CountriesController.cs: Handles country/border logic

Services

CountryService.cs: Core logic for data retrieval, caching, and persistence.
Core service that handles all the main business logic related to countries:
Loads from cache
Falls back to database
Fetches from external API (https://restcountries.com) if needed
Saves to DB and updates cache

Models

Contains entity models (Country, Border) and DTOs (CountryResponse)

Mapping

CountryMapper.cs: Converts models to DTOs

Handlers

CountryCacheHandler.cs: Manages caching using IMemoryCache
(requires .NET 9 due to MemoryCache.Keys usage)

Notes
Uses in-memory caching for performance

Falls back to DB or external API (restcountries.com) as needed

Ensure you are running .NET 9 SDK

How It Works
On the first API call, the service checks memory cache.
If no cached data is found, it queries the database.
If still no results, it fetches data from restcountries.com, saves to DB, and updates the cache.
On subsequent requests, data is retrieved from memory, providing optimal performance.








