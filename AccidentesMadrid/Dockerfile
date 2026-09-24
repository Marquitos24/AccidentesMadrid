# --- ETAPA DE COMPILACIÓN ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["AccidentesMadrid/AccidentesMadrid.csproj", "AccidentesMadrid/"]
RUN dotnet restore "AccidentesMadrid/AccidentesMadrid.csproj"

# Copiar todo el código fuente restante y compilar
COPY . .
WORKDIR "/src/AccidentesMadrid"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# --- ETAPA DE EJECUCIÓN ---
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Comando para ejecutar la aplicación (cambia 'AccidentesMadrid.dll' por el nombre real de tu ensamblado)
ENTRYPOINT ["dotnet", "AccidentesMadrid.dll"]