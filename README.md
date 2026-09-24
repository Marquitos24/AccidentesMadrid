# Análisis de Accidentes de Madrid con LINQ, PLINQ y DataFrames

Proyecto desarrollado en C# para el análisis de los accidentes de tráfico registrados en Madrid durante los años **2024, 2025 y 2026**.

La práctica utiliza tres enfoques para el procesamiento y análisis de los datos:

- **LINQ** sobre colecciones de objetos.
- **PLINQ** para estudiar la paralelización de las consultas.
- **DataFrames**, mediante `Microsoft.Data.Analysis`, para trabajar con datos tabulares.

El objetivo principal es comparar diferentes estrategias de procesamiento, estudiar su rendimiento y justificar las decisiones de diseño tomadas.

---

## Índice

- [Objetivo](#-objetivo)
- [Datos utilizados](#-datos-utilizados)
- [Tecnologías](#️-tecnologías)
- [Estructura del proyecto](#-estructura-del-proyecto)
- [Arquitectura](#️-arquitectura)
- [Lectura y combinación de datos](#-lectura-y-combinación-de-datos)
- [Modelo de datos](#-modelo-de-datos)
- [Mapper y transformación de datos](#-mapper-y-transformación-de-datos)
- [Análisis mediante LINQ y PLINQ](#-análisis-mediante-linq-y-plinq)
- [Análisis mediante DataFrames](#-análisis-mediante-dataframes)
- [Las 30 consultas](#-las-30-consultas)
- [Medición del rendimiento](#️-medición-del-rendimiento)
- [Resultados actuales](#-resultados-actuales)
- [Justificación de las decisiones de diseño](#-justificación-de-las-decisiones-de-diseño)
- [Docker](#-docker)
- [Ejecución del proyecto](#️-ejecución-del-proyecto)
- [Flujo general de ejecución](#-flujo-general-de-ejecución)
- [Estado de la práctica](#-estado-de-la-práctica)
- [Conclusiones](#-conclusiones)

---

# Objetivo

El objetivo de esta práctica es procesar los datos abiertos de accidentes de tráfico del Ayuntamiento de Madrid y realizar diferentes consultas sobre un conjunto de más de 100.000 registros.

Los objetivos principales son:

1. Leer los ficheros CSV de 2024, 2025 y 2026.
2. Convertir los registros CSV en objetos del modelo `Accidente`.
3. Combinar los tres años en una única colección.
4. Realizar 30 consultas utilizando LINQ.
5. Repetir las mismas consultas utilizando DataFrames.
6. Incorporar PLINQ para estudiar la ejecución paralela de las consultas.
7. Medir los tiempos de ejecución.
8. Comparar las diferentes estrategias.
9. Justificar las decisiones de diseño y las optimizaciones realizadas.

---

# Datos utilizados

Los datos proceden del portal de datos abiertos del Ayuntamiento de Madrid:

[Portal de datos abiertos de Madrid](https://datos.madrid.es/dataset/300228-0-accidentes-trafico-detalle/information)

Se utilizan los ficheros correspondientes a:

- 2024
- 2025
- 2026

Los CSV utilizan `;` como separador.

Las principales columnas disponibles son:

```text
num_expediente
fecha
hora
localizacion
numero
cod_distrito
distrito
tipo_accidente
estado_meteorológico
tipo_vehiculo
tipo_persona
rango_edad
sexo
cod_lesividad
lesividad
coordenada_x_utm
coordenada_y_utm
positiva_alcohol
positiva_droga
```

El conjunto combinado utilizado actualmente contiene:

**130.864 registros.**

---

# Tecnologías

| Tecnología | Uso |
|---|---|
| C# 14 | Lenguaje principal |
| .NET 10 | Plataforma de ejecución |
| LINQ | Consultas sobre colecciones |
| PLINQ | Paralelización de consultas LINQ |
| Microsoft.Data.Analysis | Trabajo con DataFrames |
| CsvHelper | Lectura y mapeo de CSV |
| Docker | Contenerización de la aplicación |
| Docker Compose | Configuración y ejecución del contenedor |
| Stopwatch | Medición de tiempos |

## Paquetes principales

```xml
<PackageReference Include="CsvHelper" />
<PackageReference Include="Microsoft.Data.Analysis" />
```

---

# Estructura del proyecto

```text
AccidentesMadrid/
│
├── Program.cs
├── AccidentesMadrid.csproj
│
├── data/
│   ├── 2024-accidentes-trafico-detalle-csv.csv
│   ├── 2025-accidentes-trafico-detalle-csv.csv
│   └── 2026-accidentes-trafico-detalle.csv
│
├── Models/
│   ├── Accidente.cs
│   ├── Sexo.cs
│   └── TipoPersona.cs
│
├── Mappers/
│   └── AccidenteMapper.cs
│
├── Repositories/
│   ├── IAccidentesRepository.cs
│   └── AccidentesRepository.cs
│
├── Services/
│   ├── IAccidentesAnalyzer.cs
│   ├── AccidentesLinqAnalyzer.cs
│   └── AccidentesDataFrameAnalyzer.cs
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

# Arquitectura

Se ha separado el proyecto en diferentes responsabilidades.

```text
                    ┌─────────────────┐
                    │    Program.cs   │
                    └────────┬────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
              ▼                             ▼
 ┌────────────────────────┐     ┌─────────────────────────┐
 │ AccidentesLinqAnalyzer │     │ AccidentesDataFrame     │
 │                        │     │ Analyzer                │
 └────────────┬───────────┘     └─────────────────────────┘
              │
              ▼
 ┌────────────────────────┐
 │ AccidentesRepository   │
 └────────────┬───────────┘
              │
              ▼
       ┌───────────────┐
       │ Archivos CSV  │
       └───────────────┘
```

## Repository

El `AccidentesRepository` se encarga de:

- Localizar los ficheros.
- Configurar `CsvHelper`.
- Leer los CSV.
- Utilizar `AccidenteMapper`.
- Obtener las tres listas.
- Combinar los registros en una única colección.

El analizador LINQ solicita los datos al repositorio y mantiene la colección combinada para realizar las diferentes consultas.

## Services

Los analizadores contienen la lógica de análisis.

Se ha definido una interfaz `IAccidentesAnalyzer` que establece las operaciones que debe proporcionar el analizador.

Esto permite mantener separada la definición de las operaciones de su implementación.

---

# Lectura y combinación de datos

## CsvHelper

Para los análisis basados en objetos se utiliza `CsvHelper`.

Los archivos utilizan `;` como separador, por lo que se configura explícitamente:

```csharp
Delimiter = ";"
```

Además, se utiliza una cultura española para trabajar correctamente con los datos.

## Lectura paralela de los tres archivos

Una de las decisiones de optimización realizadas es leer los tres archivos de forma concurrente.

Se crean tres tareas:

```csharp
var tarea1 = Task.Run(() => csvReader1.GetRecords<Accidente>().ToList());
var tarea2 = Task.Run(() => csvReader2.GetRecords<Accidente>().ToList());
var tarea3 = Task.Run(() => csvReader3.GetRecords<Accidente>().ToList());

await Task.WhenAll(tarea1, tarea2, tarea3);
```

La intención es aprovechar los recursos disponibles y evitar realizar las tres lecturas de forma estrictamente secuencial.

Una vez terminadas las tres tareas, las listas se combinan:

```csharp
var combinacion = accidentes1
    .Concat(accidentes2)
    .Concat(accidentes3)
    .ToList();
```

De esta forma se obtiene una única colección de `Accidente`.

### ¿Por qué combinar los datos una sola vez?

Las 30 consultas trabajan sobre los mismos datos.

Por ello, no tendría sentido volver a abrir y procesar los CSV para cada consulta.

La estrategia utilizada es:

```text
CSV 2024 ─┐
          │
CSV 2025 ─┼──> Lectura ──> Mapeo ──> Colección única
          │
CSV 2026 ─┘
                              │
                              ▼
                       30 consultas
```

Esto evita repetir una operación costosa de lectura y conversión.

---

# Modelo de datos

El modelo principal es `Accidente`.

Entre sus propiedades se encuentran:

```csharp
public string NumExpediente { get; set; }
public DateTime Fecha { get; set; }
public DateTime Hora { get; set; }
public string Localizacion { get; set; }
public string NumeroLocalizacion { get; set; }
public string CodDistrito { get; set; }
public string Distrito { get; set; }
public string TipoAccidente { get; set; }
public string EstadoMeteorologico { get; set; }
public string TipoVehiculo { get; set; }
public TipoPersona TipoPersona { get; set; }
public string RengoEdad { get; set; }
public Sexo Sexo { get; set; }
public string CodLesividad { get; set; }
public string Lesividad { get; set; }
public bool Alchol { get; set; }
public bool Drogas { get; set; }
```

Se han utilizado `enum` para representar determinados campos categóricos.

## Sexo

```text
Hombre
Mujer
Desconocido
```

## TipoPersona

```text
Conductor
Pasajero
Peaton
PeatonSc
```

La utilización de enumeraciones permite evitar trabajar directamente con determinados textos repetidos y hace que el modelo represente mejor el dominio.

---

# Mapper y transformación de datos

Se utiliza `AccidenteMapper`, basado en `ClassMap<Accidente>` de `CsvHelper`.

Su responsabilidad es convertir los nombres y formatos del CSV a las propiedades del modelo.

Por ejemplo:

```text
num_expediente → NumExpediente
fecha          → Fecha
hora           → Hora
cod_distrito   → CodDistrito
tipo_vehiculo  → TipoVehiculo
```

También se realizan transformaciones adicionales.

## Fechas

La fecha del CSV utiliza:

```text
dd/MM/yyyy
```

y se transforma en `DateTime`.

Esto permite posteriormente utilizar:

```csharp
a.Fecha.Year
a.Fecha.Month
a.Fecha.DayOfWeek
```

para realizar las consultas temporales.

## Alcohol y drogas

Los CSV utilizan:

```text
S
N
```

Mientras que el modelo utiliza:

```csharp
bool
```

Por tanto:

```text
S → true
N → false
```

Esto simplifica posteriormente las consultas:

```csharp
.Where(a => a.Alchol)
```

## Valores vacíos

En algunos campos existen valores vacíos.

Para determinados campos se ha decidido establecer un valor por defecto como:

```text
Desconocido
```

o:

```text
Desconocida
```

Esto evita tener que comprobar continuamente valores nulos o vacíos durante las consultas.

---

# Análisis mediante LINQ y PLINQ

La clase:

```text
AccidentesLinqAnalyzer
```

mantiene la colección combinada de accidentes y ejecuta las consultas sobre ella.

Los datos se cargan una única vez:

```csharp
accidentes = await repo.GetListaAccidentesLinq();
```

Después se reutiliza la colección para todas las consultas.

## LINQ

LINQ permite realizar operaciones como:

```csharp
GroupBy()
Where()
Select()
OrderBy()
OrderByDescending()
Take()
Count()
```

Por ejemplo, para agrupar accidentes por distrito:

```csharp
accidentes
    .GroupBy(a => a.Distrito)
    .Select(g => new
    {
        Distrito = g.Key,
        Total = g.Count()
    });
```

La ventaja principal de este enfoque es poder realizar consultas complejas sobre objetos C# utilizando una sintaxis declarativa.

## PLINQ

Como parte de la optimización se incorporará PLINQ mediante:

```csharp
.AsParallel()
```

La finalidad es comparar:

```text
LINQ
↓
Ejecución secuencial
```

frente a:

```text
PLINQ
↓
Procesamiento paralelo
```

No todas las consultas tienen por qué mejorar utilizando PLINQ.

El coste de paralelizar una operación también tiene una sobrecarga. Por tanto, una consulta sencilla sobre 130.864 elementos podría no obtener una mejora suficiente como para compensar dicha sobrecarga.

Este comportamiento será analizado mediante las mediciones de tiempo.

---

# Análisis mediante DataFrames

Para la segunda parte de la práctica se utiliza:

```text
Microsoft.Data.Analysis
```

La clase:

```text
AccidentesDataFrameAnalyzer
```

carga los tres CSV y los combina en un único `DataFrame`.

Los tres archivos se cargan inicialmente como texto:

```csharp
Type[] tipos = new Type[columnas.Length];
Array.Fill(tipos, typeof(string));
```

y posteriormente:

```csharp
DataFrame.LoadCsv(
    rutaArchivo,
    separator: ';',
    header: true,
    dataTypes: tipos
);
```

## ¿Por qué cargar las columnas como string?

Los CSV presentan diferentes situaciones que pueden provocar problemas al intentar inferir automáticamente los tipos:

- campos vacíos;
- números que pueden contener valores no numéricos;
- fechas almacenadas como texto;
- valores `S`/`N`;
- columnas con diferentes formatos.

Por ello se ha optado por una estrategia conservadora: cargar inicialmente las columnas como `string` y realizar las conversiones necesarias durante las consultas.

Esto aumenta parte del trabajo de procesamiento, pero evita errores de inferencia de tipos durante la carga.

## Combinación de DataFrames

Los tres DataFrames se combinan mediante:

```csharp
_df.Append(df2025.Rows, inPlace: true);
_df.Append(df2026.Rows, inPlace: true);
```

De esta manera se obtiene un único DataFrame con los tres años.

---

# Las 30 consultas

Se han planteado las siguientes operaciones para ambos enfoques:

| Nº | Consulta |
|---:|---|
| 1 | Total de accidentes |
| 2 | Accidentes por distrito — Top 5 |
| 3 | Accidentes por tipo |
| 4 | Accidentes por estado meteorológico |
| 5 | Accidentes por sexo |
| 6 | Accidentes por rango de edad |
| 7 | Positivos en alcohol |
| 8 | Positivos en drogas |
| 9 | Accidentes por día de la semana |
| 10 | Accidentes por mes |
| 11 | Hora con más accidentes |
| 12 | Lesiones más frecuentes |
| 13 | Tipo de vehículo más implicado |
| 14 | Accidentes con peatones |
| 15 | Proporción hombre/mujer |
| 16 | Distritos con más peatones |
| 17 | Fin de semana vs entre semana |
| 18 | Media de accidentes por día |
| 19 | Accidentes con alcohol + droga |
| 20 | Rangos de edad más vulnerables entre peatones |
| 21 | Distritos con más positivos en alcohol |
| 22 | Accidentes por código de distrito |
| 23 | Accidentes por año |
| 24 | Evolución mensual por año |
| 25 | Distrito con más accidentes por año |
| 26 | Tendencia de alcohol por año |
| 27 | Fin de semana vs entre semana por año |
| 28 | Hora pico por año |
| 29 | Lesión más frecuente por año |
| 30 | Evolución de peatones por año |

El analizador DataFrame contiene actualmente las 30 operaciones definidas en la interfaz `IAccidentesAnalyzer`.

---

# Medición del rendimiento

Para medir los tiempos se utiliza:

```csharp
Stopwatch
```

Ejemplo:

```csharp
var watch = Stopwatch.StartNew();

// operaciones

watch.Stop();

Console.WriteLine(
    $"Tiempo de ejecución: {watch.ElapsedMilliseconds} ms"
);
```

Se distinguen principalmente:

1. Tiempo de lectura de los CSV.
2. Tiempo de ejecución de las consultas LINQ/PLINQ.
3. Tiempo de ejecución de las consultas DataFrame.

Es importante separar la lectura del procesamiento porque cargar más de 100.000 registros es una operación diferente de realizar una consulta sobre una colección que ya está en memoria.

---

# Resultados actuales

> **Estado provisional:** estos resultados corresponden al estado actual del proyecto. La parte LINQ todavía no contiene las 30 consultas y PLINQ todavía está pendiente de incorporar completamente. Por tanto, los tiempos y resultados de esta sección deberán actualizarse al finalizar la implementación.

## Datos cargados

Actualmente se han combinado:

```text
2024: 49.340 registros
2025: 51.067 registros
2026: 30.457 registros

TOTAL: 130.864 registros
```

El resultado obtenido actualmente es de **130.864 accidentes/registros combinados**.

## Tiempo de lectura

Tiempo medido actualmente:

```text
1382 ms
```

Este tiempo corresponde a la lectura de los tres ficheros y su conversión a objetos `Accidente`.

## LINQ / PLINQ

Tiempo actual medido:

```text
255 ms
```

Este resultado es provisional porque todavía no están implementadas las 30 consultas ni la comparación definitiva con PLINQ.

## DataFrame

El tiempo actual de ejecución de las consultas DataFrame es:

```text
1009 ms
```

Este tiempo corresponde a la ejecución del bloque de las consultas DataFrame actualmente implementadas.

### Comparación provisional

| Operación | Tiempo actual |
|---|---:|
| Lectura CSV + mapeo | **1382 ms** |
| LINQ/PLINQ actual | **255 ms** |
| DataFrame actual | **1009 ms** |

Estos valores no deben considerarse todavía la comparación definitiva de la práctica, ya que LINQ/PLINQ debe completarse y las condiciones de comparación deben mantenerse equivalentes.

---

# Justificación de las decisiones de diseño

## 1. Separación Repository / Service

Se ha separado la lectura de los datos de la lógica de análisis.

El repositorio se ocupa de:

```text
CSV → lectura → mapeo → colección
```

Mientras que los servicios se ocupan de:

```text
colección → consultas → resultados
```

Esto evita mezclar operaciones de acceso a datos con las consultas de análisis.

---

## 2. Cargar los datos una sola vez

Los tres CSV se leen y combinan una única vez.

Posteriormente, las consultas reutilizan los datos que ya están en memoria.

Esto evita repetir:

```text
Abrir archivo
↓
Leer CSV
↓
Mapear
↓
Crear objetos
```

para cada una de las 30 consultas.

---

## 3. Lectura concurrente

Los tres archivos se procesan mediante tareas independientes:

```csharp
Task.Run(...)
```

y se espera a todas mediante:

```csharp
await Task.WhenAll(...)
```

La intención es aprovechar los recursos disponibles y reducir el tiempo necesario para procesar los tres ficheros.

Sin embargo, esta optimización se analizará mediante las mediciones, ya que utilizar paralelismo no garantiza que cualquier operación sea más rápida.

---

## 4. Uso de LINQ

LINQ resulta apropiado para trabajar con la colección de objetos `Accidente`.

Por ejemplo:

```csharp
Where()
GroupBy()
Select()
OrderByDescending()
Take()
Count()
```

permiten expresar las consultas de manera relativamente clara y directamente sobre el modelo de datos.

---

## 5. Uso de PLINQ

PLINQ se utilizará para comprobar si las consultas pueden beneficiarse de la ejecución paralela.

La decisión no consiste simplemente en utilizar PLINQ en todas las operaciones.

Una consulta pequeña puede no beneficiarse del paralelismo debido al coste de:

```text
crear particiones
↓
distribuir trabajo
↓
coordinar hilos
↓
combinar resultados
```

Por ello se compararán los tiempos reales obtenidos.

---

## 6. Uso de DataFrames

Los DataFrames son especialmente apropiados para datos tabulares.

En este caso los datos originales tienen una estructura claramente tabular:

```text
filas = accidentes
columnas = características del accidente
```

Por ello se ha implementado una segunda versión del análisis utilizando `Microsoft.Data.Analysis`.

Operaciones como:

```csharp
ValueCounts()
Filter()
```

permiten realizar determinadas agrupaciones y filtros directamente sobre columnas.

---

## 7. Comparación justa

Para poder comparar correctamente las tecnologías, ambas implementaciones deben trabajar sobre:

- los mismos tres años;
- los mismos registros;
- las mismas condiciones;
- las mismas consultas;
- resultados equivalentes.

No se debe concluir que una tecnología es más rápida únicamente porque una implementación realice menos trabajo que la otra.

---

# Docker

El proyecto incluye un `Dockerfile` basado en .NET 10.

Se utiliza una construcción en dos etapas:

```text
SDK .NET 10
     ↓
compilación / publish
     ↓
Runtime .NET 10
     ↓
ejecución
```

Esto permite separar el entorno necesario para compilar de la imagen final utilizada para ejecutar la aplicación.

El `Dockerfile` utiliza:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
```

para la compilación y:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
```

para la ejecución.

## Docker Compose

También se incluye `docker-compose.yml`.

La carpeta local:

```text
AccidentesMadrid/data
```

se monta dentro del contenedor como:

```text
/app/data
```

Esto permite mantener los CSV fuera de la imagen y proporcionar los datos al contenedor mediante un volumen.

---

# Ejecución del proyecto

## Ejecución normal

Desde la carpeta del proyecto:

```bash
dotnet restore
```

Después:

```bash
dotnet run
```

La aplicación cargará los datos y ejecutará los analizadores.

## Compilar

```bash
dotnet build
```

## Ejecutar con Docker Compose

Desde el directorio donde se encuentra `docker-compose.yml`:

```bash
docker compose build
```

Después:

```bash
docker compose up
```

Para detener el contenedor:

```bash
docker compose down
```

---

# Flujo general de ejecución

Actualmente el programa sigue este flujo:

```text
                  ┌──────────────────────┐
                  │      Program.cs      │
                  └──────────┬───────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
              ▼                             ▼
       ┌───────────────┐            ┌────────────────┐
       │ LINQ Analyzer │            │ DataFrame      │
       │               │            │ Analyzer       │
       └───────┬───────┘            └───────┬────────┘
               │                            │
               ▼                            ▼
       ┌───────────────┐            ┌────────────────┐
       │ Repository    │            │ LoadCsv        │
       └───────┬───────┘            └───────┬────────┘
               │                            │
               └──────────────┬─────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │ 3 archivos CSV   │
                    │ 2024 / 2025 / 26 │
                    └──────────────────┘
```

---

# Estado de la práctica

## Implementado actualmente

- [x] Lectura de los tres CSV.
- [x] Configuración de `CsvHelper`.
- [x] `AccidenteMapper`.
- [x] Modelo `Accidente`.
- [x] Enumeración `Sexo`.
- [x] Enumeración `TipoPersona`.
- [x] Combinación de los tres años.
- [x] Lectura concurrente de los tres CSV.
- [x] Analizador LINQ inicial.
- [x] Analizador DataFrame.
- [x] 30 métodos definidos en `IAccidentesAnalyzer`.
- [x] 30 operaciones DataFrame implementadas.
- [x] Medición de tiempos.
- [x] Dockerfile.
- [x] Docker Compose.

## ⏳ Pendiente

- [ ] Completar las 30 consultas LINQ.
- [ ] Revisar y validar los resultados de todas las consultas LINQ.
- [ ] Incorporar PLINQ.
- [ ] Comparar LINQ frente a PLINQ.
- [ ] Comparar los tres enfoques con las mismas condiciones.
- [ ] Obtener las mediciones definitivas.
- [ ] Actualizar la tabla de tiempos.
- [ ] Realizar el análisis final de rendimiento.

---

# Conclusiones

El proyecto permite comparar tres estrategias diferentes para procesar un conjunto de datos relativamente grande:

```text
LINQ
  │
  ├── Colección de objetos
  │
  └── Consultas declarativas

PLINQ
  │
  ├── Colección de objetos
  │
  └── Ejecución paralela

DataFrame
  │
  ├── Datos tabulares
  │
  └── Operaciones sobre columnas
```

Una de las conclusiones que se pretende obtener con la práctica es que **la utilización de paralelismo no implica automáticamente una mejora de rendimiento**.

El tiempo final depende de factores como:

- cantidad de datos;
- complejidad de la consulta;
- coste de agrupaciones y ordenaciones;
- número de operaciones realizadas;
- creación de estructuras auxiliares;
- coste de paralelización;
- recursos disponibles en el equipo.

Por este motivo, la decisión sobre utilizar LINQ, PLINQ o DataFrames se realizará a partir de las mediciones obtenidas y no únicamente de una suposición teórica.

La comparación definitiva se realizará una vez estén completadas las 30 consultas LINQ y la implementación de PLINQ.
