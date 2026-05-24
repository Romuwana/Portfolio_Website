# Stage 1: Build the application using .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all the files from the Render Root Directory
COPY . .

# Restore and publish using just the file name
RUN dotnet restore "PortfolioAPI.csproj"
RUN dotnet publish "PortfolioAPI.csproj" -c Release -o /app/publish

# Stage 2: Run the application using .NET 9 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port Render uses
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortfolioAPI.dll"]