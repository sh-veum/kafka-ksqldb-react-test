# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build environment and build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the main project and its dependencies
COPY ["KafkaAuction/KafkaAuction.csproj", "KafkaAuction/"]
# COPY ["KafkaAuction.Tests/KafkaAuction.Tests.csproj", "KafkaAuction.Tests/"]
# Restore packages for both projects
RUN dotnet restore "KafkaAuction/KafkaAuction.csproj"
#RUN dotnet restore "KafkaAuction.Tests/KafkaAuction.Tests.csproj"
# Copy the rest of the source code
COPY . .
# Build the main project
RUN dotnet build "KafkaAuction/KafkaAuction.csproj" -c Release -o /app/build
# Test the main project
# RUN dotnet test "KafkaAuction.Tests/KafkaAuction.Tests.csproj" --verbosity normal

# Publish the application
FROM build AS publish
RUN dotnet publish "KafkaAuction/KafkaAuction.csproj" -c Release -o /app/publish

# Build the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaAuction.dll"]