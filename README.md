# .NET Coding Challenge
This is a coding challenge for .NET developers under the assumption that they are familiar with .NET Core, EF Core, MVC and REST.

## Overview

The objective of this challenge is to create an API that manages customers and their respective users.

## Requirements

1. You must use .NET Core (Version 8.0 or newer) and C#
2. The API must consume and return JSON data
3. The GET API should utilize the following OData parameters to query data 
    - $select
    - $filter
    - $top
    - $skip
    - $orderby
4. The data must be stored in a persistent database using EF Core and SQLite 
5. You do not need to implement Authorization/Authentication or consider any other security implications 
6. You are free to use any NuGet package if you can justify its usage

## Specification

The application must expose two RESTful API endpoints providing standard CRUD functionality for the **Customer** and **User** entities.

### Customer
#### URL Path
**GET**
```
/api/customer
/api/customer/{id}
```
**POST**
```
/api/customer
```
**PATCH**
```
/api/customer/{id}
```
**DELETE**
```
/api/customer/{id}
```

#### Data Format
| Field Name | Data Type | Required | Unique | Validation      |
|--|--|-------|--------|-----------------|
| Name | `string` | true  | true   | max length 255  |
| Website | `string` | false | false  | max length 255  |

#### Additional Information
- If a customer is deleted all associated users must also be deleted
- The customer response objects must not contain associated users
- Creating or updating a customer must not affect any user

### User
#### URL Path
**GET**
```
/api/customer/{id}/user
/api/customer/{id}/user/{id}
```
**POST**
```
/api/customer/{id}/user
```
**PATCH**
```
/api/customer/{id}/user/{id}
```
**DELETE**
```
/api/customer/{id}/user/{id}
```

#### Data Format
| Field Name    | Data Type  | Required | Unique | Validation                       |
|---------------|------------|----------|--------|----------------------------------|
| DisplayName   | `string`   | true     | true   | max length 255                   |
| FirstName     | `string`   | true     | false  | max length 255                   |
| LastName      | `string`   | true     | false  | max length 255                   |
| Email         | `string`   | true     | true   | typical email address validation |
| Date of Birth | `DateTime` | true     | false  |  |

#### Additional Information
- A user must always belong to a single customer
- The user response objects must only contain the associated customer's Id, no additional customer information

## Assessment Criteria

- The provided code should be near production quality
- The provided code should be testable
- The API should return plausible HTTP codes

We will not provide a strict time limit, but we recommend to spend between 2-6 hours. Please clone this repository and push your results to a private repo when you are ready to share your work with us. 


SOLUTION How a Request Would Flow (Once Fully Built):

Client (browser, Postman, etc.)
    │
    │  HTTP request: GET /api/Customer/abc-123/User
    ▼
Program.cs
    │  Routes request to the right controller
    ▼
UserController.List(customerId: "abc-123")
    │
    │  Calls a service or DbContext
    ▼
EF Core (DbContext) — not built yet
    │
    │  Runs SQL against SQLite
    ▼
SQLite Database — not set up yet
    │
    │  Returns User rows where CustomerId = abc-123
    ▼
UserController
    │  Maps entities to JSON DTOs
    ▼
Client receives JSON response