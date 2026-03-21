# Announcement board — Frontend

An ASP.NET Core MVC web application for the Announcement board project. It provides the user interface for browsing, filtering, and managing announcements. All data is loaded from and saved through a separate [ASP.NET Core API backend](https://announcement-board-b4cndjerc6bsawf9.polandcentral-01.azurewebsites.net/swagger/index.html) (Swagger UI).

The frontend application provides a user interface for interacting with the Announcement Board system, supporting full CRUD operations. Users can create, view, update, and delete announcements through a structured and user-friendly interface. Each announcement includes a title, description, creation date, and status (active or inactive).

Beyond the requirements defined in the technical specification, several enhancements were implemented to improve the overall structure, performance, and maintainability of the application:

## Database Structure & Optimization

Separate tables were introduced for Categories and SubCategories to normalize the data model and improve scalability.

To optimize filtering and sorting operations, a composite index was added to the Announcements table.

Additionally, the Announcements table was extended with an UpdatedDate field. This allows tracking modifications separately from the creation timestamp, which aligns with common OLTP practices and enables displaying the last update time after editing an announcement.

## Data Access

Database interaction is implemented using stored procedures in combination with Dapper.

## Deployment & Architecture

The application is deployed with the frontend and backend running as separate services.

A CI/CD pipeline is configured to automate the build and deployment process, ensuring consistent delivery across environments.

## Authentication & Authorization

The application uses Google-based authentication. Users sign in on the client side and receive a token, which is then included in API requests. The server validates this token and identifies the user, creating a record in the database if necessary.

Access to functionality is restricted based on authentication status. Creating, updating, and deleting data is available only to authenticated users, while read operations remain publicly accessible.
