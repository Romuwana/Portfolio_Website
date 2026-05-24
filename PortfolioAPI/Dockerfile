# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all the files from the repository into the Docker container
COPY . .

# Restore and publish pointing specifically to the inner folder
RUN dotnet restore "PortfolioAPI/PortfolioAPI.csproj"
RUN dotnet publish "PortfolioAPI/PortfolioAPI.csproj" -c Release -o /app/publish

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port Render uses
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortfolioAPI.dll"]