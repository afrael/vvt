# Small API Demo

A dotnet webapi application that processes a CSV file, persists the records to a SQL Server database and applies a few validations.

## Assumptions

1. All employees that have a blank hire date will have their hire date set to 01/01/01 `Datetime.MinValue`.
2. All employee that have a blank ManagerEmployeeNumber are considered top-level managers.
3. The employee number is unique, the first one seen will be the one considered valid.

## Validations

1. An empty file will be rejected.
2. A file with missing file headers will be rejected.
3. A file with missing columns will be rejected.
4. A manager and its employees must belong to the same company.
5. An employee can only be affiliated to one company.