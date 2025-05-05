# Problem

### Goal

Implement an algorithm to detect flight schedule changes in airline competitive landscape.

### Description

You are creating an application, to be used by airline agencies, for detecting cases of airlines introducing new or discontinuing existing regular flight schedules.
For example, Lufthansa is offering a flight from Berlin to Vienna every Monday at 7:30. In the case that Lufthansa decides to stop offering that flight, we need to be able to detect that, because that opens a business opportunity for flight agencies to offer alternative flights. The same applies to newly introduced flight schedules.

### Change detection algorithm

### `New flights`

A flight with departure time T is considered to be a new flight if no corresponding flight from same airline exists with departure time T’ = T - 7 days (+/- 30 minutes tolerance).

### `Discontinued flights`

A flight with departure time T is considered to be discontinued if no corresponding flight from same airline exists with departure time T’ = T + 7 days (+/- 30 minutes tolerance).

### Terminology

[The data is available for download at this link](https://drive.google.com/file/d/1y9lyaT3lXbK0ZhyI-Ys6vWxEM_XgQQ5Z/view?usp=sharing)

- Route : Unique combination of origin, destination and departure date.
- Flight : Concrete flight scheduled per airline. Flights are in many-to-one relationship with routes.
- Subscription : Origin/destination pair an agency is interested in.

### Input

You will be provided with the sample data set consisting of three CSV files with following structure:

| routes.csv          |  flights.csv   |   subscriptions.csv |
| ------------------- | :------------: | ------------------: |
| route_id            |   flight_id    |           agency_id |
| origin_city_id      |    route_id    |      origin_city_id |
| destination_city_id | departure_time | destination_city_id |
| departure_date      |  arrival_time  |                     |
|                     |   airline_id   |                     |

The data set intentionally omits city, airline and agency tables, because they’re not crucial for the implementation of the algorithm.

### Deliverables

1. Create a database with table structure to accommodate provided data and import the data into database tables as it is (no transformation needed – foreign keys already match).
2. Implement a data access layer (preferably using Entity Framework).
3. Implement a .NET console application taking three command line parameters:

    - start date (in yyyy-mm-dd format)
    - end date (in yyyy-mm-dd format)
    - agency id (used to filter the flights an agency is interested in, by applying subscriptions)

   <br/>

   ```bash
      Example call: YourProgram.exe 2018-01-01 2018-01-15 1
   ```

4. Output the results in results.csv file, containing following columns: - flight_id

    - origin_city_id
    - destination_city_id
    - departure_time
    - arrival_time
    - airline_id
    - status (New / Discontinued)

5. Display execution metrics (i.e. time taken to execute the change detection algorithm).

### Elaborate

- Database design (data types, keys, indexes)
- Overall structure of the application (layers, data flow, dependencies, (de)coupling) - Data access layer implementation
- Change detection algorithm implementation
- Data structures used
- Optimizations applied

---

# Solution

### Database Design

I am using **SQLite** as the database, The database consists of three main tables: `flights`, `routes`, and `subscriptions`. These tables store information related to flights, routes, and agency subscriptions, respectively.

#### Tables and Columns:

- **flights**
    - `id` (int: Primary Key)
    - `route_id` (int: Foreign Key to `route`)
    - `departure_time` (DateTime)
    - `arrival_time` (DateTime)
    - `airline_id` (int)

- **routes**
    - `id` (int: Primary Key)
    - `origin_city_id` (int)
    - `destination_city_id` (int)
    - `departure_date` (DateOnly)

- **subscriptions**
    - `agency_id` (int :Primary Key)
    - `origin_city_id` (int: Primary Key)
    - `destination_city_id` (int: Primary Key)

These are the C# datatypes. The SQLite datatypes are limited, so I used `INTEGER` for `DateTime` and `DateOnly` datatypes.

#### Relationships:
  - The **`flight`** table references the **`route`** table using the `route_id` foreign key, establishing a many-to-one relationship between flights and routes.

#### Indexes:
  - In addition to the indexes created based on the primary keys, I added the following:
    - `IX_routes_origin_city_id_destination_city_id` on `routes` includes [`origin_city_id`, `destination_city_id`]
    - `IX_flights_route_id_departure_time` on `flights` includes [`route_id`, `departure_time`] 

#### Database Diagram:

```
+---------------------+      +------------------+
|    routes           |      |   flights        |
+---------------------+      +------------------+
| id                  | <--- | route_id         |
| origin_city_id      |      | departure_time   |
| destination_city_id |      | arrival_time     |
| departure_date      |      | airline_id       |
+---------------------+      | id               |
                             +------------------+

        +---------------------+
        |   subscriptions     |
        +---------------------+
        | agency_id           |
        | origin_city_id      |
        | destination_city_id |
        +---------------------+
```

### Overall Structure of the Application

The application follows a clean, layered architecture with the following components:

1. **Flight.Domain**:
   - Contains core business logic and entities (`Flight`, `Route`, `Subscription`).
   - Decoupled from other layers, ensuring business logic is isolated.

2. **Flight.Application**:
   - Contains application services (e.g., the change detection algorithm).
   - Orchestrates use cases, applying business rules, while depending on the domain and infrastructure layers.

3. **Flight.Infrastructure**:
   - Implements data access via SQLite and Entity Framework.
   - Handles database operations, keeping domain and application layers decoupled from database concerns.

4. **Flight.Cli**:
   - Command-line interface for user interaction.
   - Accepts input, invokes application services, and outputs results to CSV.

#### Data Flow:
- **Flight.Cli** takes user input, passes it to **Flight.Application**.
- **Flight.Application** processes the input, requests data from **Flight.Infrastructure**.
- Results are returned to **Flight.Cli** for output.

#### Dependencies and Decoupling:
- **Flight.Domain** is fully independent.
- **Flight.Application** depends on **Flight.Domain**.
- **Flight.Infrastructure** handles data persistence and depends on **Flight.Application** and **Flight.Domain**.
- **Flight.Cli** interacts with all other layers, serving as the entry point.

This structure ensures decoupling, making the system flexible and maintainable.


### Change Detection Algorithm Implementation

The change detection algorithm is implemented in the `FlightService` class and is responsible for identifying new or discontinued flights based on the provided query parameters. Key aspects of the implementation include:

- **Input Parameters**: The method `DetectChangesForAgency` takes a `DetectFlightChangesQuery` object, containing the start date, end date, and agency ID. It also expands the date range to include a historical period and future plans, based on configurable options.

- **Flight Retrieval**: The system fetches all relevant flights from the repository using `flightRepository.GetInterestedAgencyFlights`, filtered by agency ID and the extended date range.

- **Change Detection**:
  - Flights are grouped by airline and route, making it easier to compare flights within the same origin-destination pair.
  - Parallel processing is used (`Parallel.ForEach`) to detect changes across multiple flights, optimizing performance.
  - A flight is considered **new** if no corresponding flight exists in the previous week (with a tolerance of +/- 30 minutes).
  - A flight is considered **discontinued** if no corresponding flight exists in the following week (also with a 30-minute tolerance).

- **Results**: Detected changes are added to a `ConcurrentBag` of `DetectFlightChangesResult`, which records the flight ID, origin and destination city, departure and arrival times, airline ID, and status (New/Discontinued).

This approach ensures timely detection of changes in flight schedules, allowing for efficient tracking of new and discontinued flights.


### Data Structures Used

The change detection algorithm leverages several key data structures to manage and process flight data efficiently:

- **`Dictionary<string, List<Flight>>`**:
  Flights are grouped by a composite key (airline ID, origin city ID, destination city ID) and stored in a dictionary. This allows for fast lookups when comparing flights from the same airline and route (by reducing the number of datetime comparisons), optimizing the change detection process.

- **`ConcurrentBag<DetectFlightChangesResult>`**:
  A thread-safe collection used to store detected flight changes (new or discontinued) during parallel processing. This ensures that multiple threads can safely add results concurrently without locking.

These data structures were chosen for their performance and thread-safety to ensure efficient and concurrent processing of large datasets.


### Optimizations Applied

Several optimizations were applied to ensure the efficient processing of flight data:

- **Efficient Flight Data Retrieval**:
   The database indexed are designed based on the query of retrieving flights based on an agency interesting routes, so the query is executed in a reasonable time.

- **Dictionary Lookup**:
   Flights are grouped by a composite key (`airline_id`, `origin_city_id`, `destination_city_id`) and stored in a dictionary. This allows for fast lookups when comparing flights, minimizing the need for repeated iterations over large collections.

- **Parallel Processing**:
   The use of `Parallel.ForEach` with a defined `MaxDegreeOfParallelism` allows concurrent processing of flight data. This reduces the overall execution time, especially when handling large datasets.

- **Thread-Safe Data Storage**:
   The use of `ConcurrentBag` for storing detected flight changes ensures thread-safe access during parallel execution without the overhead of locks, enhancing performance in a multi-threaded environment.

These optimizations collectively improve the performance and scalability of the flight change detection algorithm.

### Others
  - Using `EFCore.BulkExtensions` to speed up importing data from the CSV files.
  - Saving the change detection algorithm into a file via Decorator pattern instead of modifying the main service.
  - Displaying the algorithm execution metrics into a proxy class.
  - Using `appsettings.json` to store the settings.
  - Using dotnet diagnostic logging feature.
  - Reading the initial data from CSV files and importing them into the database as a pipeline instead of reading all items at the first and then importing to the database (to optimize memory usage).
  - Converting the result to CSV format and writing them into the CSV file like the importing style (previous item). 