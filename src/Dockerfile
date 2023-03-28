FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS SDK

ARG NUGET_LOGIN
ARG NUGET_PASSWORD
ARG PROJECT_NAME
ARG EXPOSE_PORT="5000"

ENV NUGET_LOGIN=$NUGET_LOGIN
ENV NUGET_PASSWORD=$NUGET_PASSWORD

COPY rca.crt /usr/local/share/ca-certificates/rca.crt
RUN chmod 644 /usr/local/share/ca-certificates/rca.crt && update-ca-certificates

WORKDIR /src
COPY . .

COPY rca.crt /usr/local/share/ca-certificates/rca.crt
RUN chmod 644 /usr/local/share/ca-certificates/rca.crt && update-ca-certificates

RUN dotnet restore "$PROJECT_NAME/$PROJECT_NAME.csproj" --disable-parallel --configfile nuget.config
RUN dotnet publish "$PROJECT_NAME/$PROJECT_NAME.csproj" -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as RUNTIME

COPY rca.crt /usr/local/share/ca-certificates/rca.crt
RUN chmod 644 /usr/local/share/ca-certificates/rca.crt && update-ca-certificates

WORKDIR /app

COPY --from=SDK /out .
RUN chmod +xr /app && addgroup -S app-group -g 1000 && adduser -S app-user -G app-group -u 1000 && chown -R app-user:app-group /app  

ARG PROJECT_NAME
ARG EXPOSE_PORT="5000"

ENV PROJECT_NAME=$PROJECT_NAME
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV COMPlus_EnableDiagnostics=0

USER 1000:1000

EXPOSE $EXPOSE_PORT

ENTRYPOINT dotnet $PROJECT_NAME.dll