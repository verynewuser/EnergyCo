# EnergyCo

## Instructions to run
This a .NET 10 solution created with Visual Studio 2026 and a Clean Architecture template. When the API project is run (or deployed), it exposes three endpoints to fetch products and discounts details. The project needs a database to conenct. The connection string must be updated in `appsettings.json` file before running the project. When run first time, the project will create seeding tables. Running the project (through F5 etc.) will appear a Scalar UI page, which is a modern alternative to Swagger UI.

## API endpoints
### POST `/api/v1/DiscountController`
The endpoint takes JSON input containing customer's cart data, and returns the earned points & discount.
### GET `/api/v1/Product`
It will return a list of products and the relevant discounts
### GET `/api/v1/PointsPromotion`
It will return the promotions.

## Technical Details
* The project is built using .NET 10 and Visual Studio 2026. I used SQL Server Express editon on my PC.
* For interactive API documentation I used Scalar instead of Swagger. Scalar is a modern alternate to Swagger, and recommended by Microsoft.
* EntityFramework Core is used as an ORM tool to connect with the database.
* CQRS design pattern was used to segregate commands and queries, with the help of MediatR library
* AutoMapper is used for seamless mapping between entities and DTOs (I faced an issue with it and wrote a developer comment there)
* FluentValidation library is used for input data validation
* For unit testing I chose NUnit and Moq libraries. (I have experience with xunit and Substitute libraries as well )

## Potential Improvements
* Caching can be used to improve performance. I reckon the discounts and promotions are not changed every day, so instead of DB queries against every API call, I can implement a caching layer to return relevant discounts and promotions for products.
* I didn't see any merit of employing an event-driven architecture here because the business requirement is simple synchronous request-response.
* Logging can be improved further in production environments.
* For further performance boost, JSON endpoints can be swapped with GraphQL endpoints, but the data sets are not huge in this case.
* Authentication and athorizaton can be implemented in the API to secure it. We can use OAuth and JWT with role-based auth in it.
