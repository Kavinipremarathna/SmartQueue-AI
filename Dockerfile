# Backend container
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartQueueAPI.csproj ./
RUN dotnet restore "SmartQueueAPI.csproj"

COPY . .
RUN dotnet publish "SmartQueueAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5055
ENV ASPNETCORE_URLS=http://+:5055
ENTRYPOINT ["dotnet", "SmartQueueAPI.dll"]
