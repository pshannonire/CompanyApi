# CompanyAPI

> A clean, robust, and testable .NET 8 Web API for managing company data, supporting JWT authentication, full CRUD operations, and advanced querying with pagination, sorting, and validation.

---

## Instructions to run
- In Visual Studio, select the IIS Express launch profile and run the project.
	- The API will be available at: https://localhost:44374/
- Obtain a bearer token from https://localhost:44374/api/Auth/login 
- Example POST https://localhost:44374/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}

- Use the bearer token retrieved to call all the other endpoints
- Example using Postman:
		- Set the method to GET
		- URL: GET https://localhost:44374/api/companies/1
        - Go to Authorization tab
        - Choose Bearer Token
        - Paste your token


##  Overview

**CompanyAPI** is an ASP.NET Core Web API designed for scalability, maintainability, and clarity.
It allows clients to get/add and update companies, with secure authentication, validation, and a layered clean architecture

---

##  Key Architectural Decisions

### 1. **Clean Architecture / Onion Architecture**
- **Separation of Concerns**: Codebase is divided into **Domain**, **Application**, **Infrastructure**, and **API** layers.
- **Dependencies** only point inward—core logic (Domain, Application) never depends on Infrastructure or external frameworks.
- **CQRS & Mediator Pattern**: Commands and Queries are handled separately using MediatR promoting single-responsibility and testability.

### 2. **Validation with FluentValidation**
- **Every request** is validated via xFluentValidation
- Rules for pagination, sorting, search, and DTO properties are enforced server-side.

### 3. **Authentication**
- **JWT Bearer tokens** secure all endpoints except


### 4. **API Design Principles**
- **RESTful routes**: ; GET/POST/PUT semantics are clear.
- **Standard HTTP responses**: `200 OK`, `201 Created`, `400 Bad Request`, `401 Unauthorized`, `404 Not Found`.
- **Paginated and filterable endpoints** for large datasets.

### 5. **Testing**
- **Integration tests** ( cover all key scenarios including:
  - Authentication
  - CRUD and search/pagination
- **Unit tests** (xUnit, fluentValidation, moq)

---

##  Tech Stack

- **.NET 8 / ASP.NET Core**
- **Entity Framework Core** (with in-memory for tests and for actual db)
- **MediatR** (CQRS, separation of concerns)
- **FluentValidation** 
- **JWT Bearer Authentication**
- **xUnit** + **FluentAssertions** + **Moq**
- **Serilog** (logging)
- **Swagger**

---

##  Future Improvements 
- add caching to endpoints via IMemoryCache or Redis Cache to increase the speed of data retreival
- add domain events to eventually enable sending events via Kafka
- create more in depth authentication, different roles for users of api etc.
- add docker support 

