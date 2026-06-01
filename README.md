# 🚗 Car Rental System

<div align="center">

![ASP.NET](https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=c-sharp\&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-68217A?style=for-the-badge)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge\&logo=postgresql\&logoColor=white)
![Razor](https://img.shields.io/badge/Razor_Pages-5C2D91?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-In_Development-orange?style=for-the-badge)

</div>

---

## 📖 About

A modern **Car Rental System** built using **ASP.NET MVC**, **Entity Framework**, **PostgreSQL**, and **Razor**.

The project is designed with scalability and maintainability in mind, featuring a normalized relational database structure and a clean MVC architecture.

Users can browse vehicles, manage rentals, and handle car ownership through a structured rental workflow.

---

## ✨ Features

* 👤 User Authentication & Authorization
* 🔐 Role-Based Access Control
* 🚘 Car Listings & Management
* 📅 Rental Booking System
* 💰 Rental Cost Calculation
* ⛽ Fuel Type Management
* ⚙️ Transmission Categories
* 🚙 Vehicle Type System
* 🕒 Automatic Timestamps
* 🧩 Clean Relational Database Design

---

## 🛠️ Tech Stack

| Technology       | Purpose               |
| ---------------- | --------------------- |
| ASP.NET MVC      | Backend Framework     |
| Razor            | Server-side Rendering |
| Entity Framework | ORM                   |
| PostgreSQL       | Database              |
| C#               | Programming Language  |
| DBML             | Database Modeling     |

---

## 🧱 Database Entities

```txt id="v8nq1d"
users
roles
user_roles
cars
rentals
fuel
transmission
types
```

---

## 🔗 Relationships

* A user can own multiple cars
* A user can rent multiple cars
* Cars belong to:

  * one fuel type
  * one transmission type
  * one category
* Users can have multiple roles
* Rentals connect:

  * renter
  * owner
  * rented car

---

## 🚀 Getting Started

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/HasanAlasker/Sayarah-CarRental
cd CarRental
```

### 2️⃣ Configure Database

Update your `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=CarRentalDB;Username=postgres;Password=yourpassword"
}
```

### 3️⃣ Run Migrations

```bash
dotnet ef database update
```

### 4️⃣ Start the Application

```bash
dotnet run
```

---

## 📌 Future Improvements

* 💳 Online Payments
* ⭐ Reviews & Ratings
* 📍 Location Support
* 🖼️ Car Image Uploads
* 📬 Notifications
* 📅 Availability Calendar
* 📱 Mobile App Integration

---

## 📄 License

MIT License © Hasan Alasker
