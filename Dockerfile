FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY ./publish /app
ENV ASPNETCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet","NotificationService.dll"]