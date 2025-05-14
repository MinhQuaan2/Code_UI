	# Start with a base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the SWIPETUNE_API project file
COPY ["CodeUI.API/CodeUI.API.csproj", "CodeUI.API/"]

# Copy the BusinessObject project file
COPY ["CodeUI.Data/CodeUI.Data.csproj", "CodeUI.Data/"]

# Copy the DataAccess project file
COPY ["CodeUI.Service/CodeUI.Service.csproj", "CodeUI.Service/"]


# Restore the project dependencies
RUN dotnet restore "CodeUI.API/CodeUI.API.csproj"

# Copy the source code
COPY . .

# Build the application
RUN dotnet build "CodeUI.API/CodeUI.API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "CodeUI.API/CodeUI.API.csproj" -c Release -o /app/publish 


# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published files from the build image
COPY --from=build /app/publish .

# Expose the port(s) that the application will listen on
EXPOSE 44360

# Set the entry point for the container
ENTRYPOINT ["dotnet", "CodeUI.API.dll"]
