# Restaurant Management System

Web-based application for managing restaurant operations, including table reservations, menu administration, and user management.  
The system also provides a RESTful API for interaction with external clients.

Built with ASP.NET Core 9.0 using Entity Framework Core and ASP.NET Identity.

---

## 📖 Project Description

This project demonstrates backend-focused development of a database-driven web application with authentication, role-based access control, and REST API integration.

The application is designed to support two main roles: **User** and **Administrator**, each with different levels of access.

---

## 🖼 Screenshots

<img src="wwwroot/images/home_1.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_1.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_2.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_4.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_7.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_10.jpg" width="600">
<img src="wwwroot/images/screensghot/Screenshot_11.jpg" width="600">

---

## 🚀 Features

### 👤 User
- Registration and authentication using ASP.NET Identity
- Table reservation functionality

### 🛠 Administrator
- User management (create, edit, delete)
- Restaurant menu management (add, edit, delete dishes)
- View and manage reservations
- Administrative dashboard for full content and user control

---

## 🛠 Tech Stack

| Component        | Technology                                   |
|------------------|----------------------------------------------|
| Language         | C#                                           |
| Framework        | ASP.NET Core 9.0                             |
| ORM              | Entity Framework Core                        |
| Database         | Microsoft SQL Server                         |
| Authentication  | ASP.NET Core Identity                        |
| API              | REST API (ASP.NET Core Web API)              |
| Frontend         | Razor Pages, Bootstrap 5, JavaScript         |
| Styling          | CSS                                          |
| Package Manager  | npm (Bootstrap)                              |

---

## 🧠 What I Implemented

- Backend architecture and business logic
- Authentication and role-based authorization
- RESTful API endpoints
- Database design and Entity Framework Core migrations
- Middleware configuration (sessions, authentication, authorization)
- Localization and culture configuration
- Asynchronous programming patterns in .NET

---

## ▶️ How to Run the Project

### 1. Clone the repository (EFIdentity branch)

```
git clone https://github.com/Tat-T/Restaurant.git
cd Restaurant
git checkout EFIdentity
```

### 2. Configure the database

Edit the connection string in appsettings.json:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"}
  ```

### 3. Apply migrations
```
dotnet ef database update
```

### 4. Run the application
```
dotnet run
```

Or open the project in Visual Studio and press F5.

The application will be available at:
```
https://localhost:5015
```

🔑 Test Accounts

Administrator:

Email: admin@mail.ru

Password: ******** *Test credentials available upon request*

User:

Email: lara@mail.ru

Password: ******** *Test credentials available upon request*

🔌 API Example

<img src="wwwroot/images/insomnia_2.jpg" width="600">

👩‍💻 Author

Tatyana Yantkova

Junior Software Engineer (.NET)
Focused on backend development and international projects