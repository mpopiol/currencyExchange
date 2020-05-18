# Currency Exchange Rate API
REST api that allows the access to ECB exchange rate while caching the responses in memory (request caching and memory caching). Before launching the project, you must run all migrations first.

## Architecture: 
Clean architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

### Reason for architecture choice: 
- small project that should be independent of frameworks/database, 
- business requirements are enclosed in lowest layer possible

## Used technologies:
- Object-Object Mapping with AutoMapper
- Data access with Entity Framework Core
- Web API using ASP&#46;NET Core
- Inversion of Control with Autofac
- HTTP request handling with RestSharp
- Logging with Serilog
- Automated testing with xUnit&#46;net, Moq, and FluentAssertions
