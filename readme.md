# How to Run the Application

## MySQL Database Setup

You can either run MySQL locally or use Docker to set up a local MySQL instance.

### Using Docker:
To set up MySQL with Docker, run the following commands:

```bash
$ docker pull mysql
$ docker run -d --name mysql-container -e MYSQL_ROOT_PASSWORD=password -p 3306:3306 mysql:latest
```
Then, connect to the MySQL database:
```bash
$ mysql -h 127.0.0.1 -P 3306 -u root -p
```
Create Database and Table:
Once you're connected to MySQL, execute the following SQL commands to create the database and table:
```mysql
CREATE DATABASE SmsMessageDatabase;
USE SmsMessageDatabase;

CREATE TABLE SmsMessages (
Id CHAR(36) NOT NULL PRIMARY KEY,
PhoneNumber VARCHAR(15) NOT NULL,
SentAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```
### Running the Application
To run the application locally:
1. Navigate to the project directory:
```bash
$ cd SMS-Rate-Limiting
```
2. Build the application:
```bash
$ dotnet build
```
3. Run the application:
```bash
$ dotnet run
```
Once the application is running, you can access the API documentation via Swagger at:
```
http://localhost:5264/swagger/index.html
```
### Running the Tests
To run the tests locally:
1. Navigate to the tests directory:
```bash
$cd Tests
```
2. Run the tests
```bash
$dotnet test
```

