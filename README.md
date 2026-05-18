# 🛒 Multi-Vendor E-Commerce System (ShopHub)

A comprehensive desktop-based **Multi-Vendor E-Commerce Management System** built with **C# Windows Forms** and **Microsoft SQL Server**. ShopHub provides a complete digital marketplace where multiple vendors can sell products, customers can shop seamlessly, and administrators can oversee the entire platform from a centralized dashboard.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [System Architecture](#-system-architecture)
- [Database Schema](#-database-schema)
- [Installation](#-installation)
- [Getting Started](#-getting-started)
- [Default Login Credentials](#-default-login-credentials)
- [Project Structure](#-project-structure)
- [User Roles](#-user-roles)
- [Screenshots](#-screenshots)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 🌟 Overview

**ShopHub** is a fully-featured multi-vendor e-commerce platform that simulates real-world online marketplaces like Amazon or eBay. The system supports **three distinct user roles** — Admin, Vendor, and Customer — each with tailored dashboards, permissions, and functionalities.

The application emphasizes **role-based access control (RBAC)**, **secure data management**, **modern UI design**, and a **modular code architecture** for scalability and maintainability.

---

## ✨ Features

### 👤 Customer Features
- 🔐 Secure registration & login
- 🏠 Browse products with category filters & keyword search
- 🛒 Shopping cart management (add, update, remove, clear)
- 💳 Multiple payment options (Cash on Delivery, Credit Card, Debit Card, UPI, Net Banking)
- 📦 Place orders with shipping address
- 📋 View order history & track order status
- ⭐ Write product reviews & ratings (1–5 stars)
- 👨‍💼 Manage personal profile and change password

### 🏪 Vendor Features
- 📊 Real-time dashboard with key metrics (Products, Orders, Revenue, Reviews)
- 📦 Full product management (Add, Edit, Delete, View)
- 🛍️ Order tracking & status updates (Pending → Processing → Shipped → Delivered)
- 📈 Sales reports & top-selling product analytics
- 🏬 Store customization (name, description, phone, address)
- 🔒 Approval-based vendor onboarding

### 🛡️ Admin Features
- 📊 Centralized dashboard with platform-wide statistics
- 👥 User management (view, search, filter, deactivate, delete)
- ✅ Vendor approval system (Approve / Reject)
- 📦 Product oversight (toggle active/inactive)
- 🛒 View all orders & update statuses
- 🏷️ Manage product categories (CRUD)
- ⭐ Moderate customer reviews
- 📊 Comprehensive sales reports (Revenue by Vendor, Top Products)

---

## 🛠️ Tech Stack

| Component         | Technology                          |
| ----------------- | ----------------------------------- |
| **Frontend (UI)** | C# Windows Forms (.NET Framework 4.7.2) |
| **Backend**       | C# (.NET Framework)                 |
| **Database**      | Microsoft SQL Server (LocalDB)      |
| **IDE**           | Visual Studio 2019 / 2022           |
| **Data Access**   | ADO.NET (System.Data.SqlClient)     |
| **Architecture**  | Layered (Forms → Helper → Database) |

---

## 🏗️ System Architecture

```
┌──────────────────────────────────────────────────┐
│              Presentation Layer                  │
│  (LoginForm, Dashboards, Management Forms)       │
└──────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────┐
│              Business Logic Layer                │
│  (Form Event Handlers, Validation, Workflow)     │
└──────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────┐
│              Data Access Layer                   │
│  (DatabaseHelper.cs - Centralized DB Operations) │
└──────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────┐
│              Database Layer                      │
│        (SQL Server - MultiVendorDB)              │
└──────────────────────────────────────────────────┘
```

---

## 🗄️ Database Schema

The application uses **8 interrelated tables**:

| Table         | Description                                       |
| ------------- | ------------------------------------------------- |
| `Users`       | Master user accounts with roles                   |
| `Vendors`     | Vendor profiles (extends Users)                   |
| `Customers`   | Customer profiles (extends Users)                 |
| `Categories`  | Product categories                                |
| `Products`    | Product catalog (linked to Vendors & Categories)  |
| `Orders`      | Customer orders with status tracking              |
| `OrderItems`  | Order line items (junction table)                 |
| `Reviews`     | Product ratings & reviews                         |

### Entity Relationships
- One **User** → One **Vendor** OR One **Customer** (1:1)
- One **Vendor** → Many **Products** (1:N)
- One **Category** → Many **Products** (1:N)
- One **Customer** → Many **Orders** (1:N)
- One **Order** → Many **OrderItems** (1:N)
- One **Product** → Many **Reviews** (1:N)

---

## 🚀 Installation

### Prerequisites
- ✅ **Visual Studio 2019/2022** (with .NET Desktop Development workload)
- ✅ **SQL Server LocalDB** (typically installed with Visual Studio)
- ✅ **.NET Framework 4.7.2** or higher
- ✅ **Windows OS** (Windows 10/11 recommended)

### Step 1: Clone the Repository
```bash
git clone https://github.com/your-username/MultiVendorEcommerce.git
cd MultiVendorEcommerce
```

### Step 2: Set Up the Database
1. Open **SQL Server Management Studio (SSMS)** or **Visual Studio's SQL Server Object Explorer**.
2. Connect to `(localdb)\MSSQLLocalDB`.
3. Open and execute the script `SQLQuery1.sql` located in the project folder.
4. This will create the `MultiVendorDB` database with all required tables and seed data.

### Step 3: Configure Connection String
The default connection string in `App.config` is:
```xml
<connectionStrings>
  <add name="MultiVendorDB"
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MultiVendorDB;Integrated Security=True;Encrypt=False;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
> 💡 Modify if you're using a different SQL Server instance.

### Step 4: Build & Run
1. Open `MultiVendorEcommerce.sln` in Visual Studio.
2. Restore NuGet packages (if prompted).
3. Press `F5` or click **Start** to build and run the application.

---

## 🎬 Getting Started

When you launch the app:
1. A **database connection test** is performed.
2. If successful, the **Login Form** opens.
3. Log in using one of the default credentials below, or click **Register** to create a new account.

---

## 🔑 Default Login Credentials

| Role     | Username   | Password   |
| -------- | ---------- | ---------- |
| Admin    | `admin`    | `admin123` |
| Vendor   | `vendor`   | `vendor123`|
| Customer | `customer` | `cust123`  |

---

## 📁 Project Structure

```
MultiVendorEcommerce/
│
├── 📄 Program.cs                      # Application entry point
├── 📄 App.config                      # Connection string configuration
├── 📄 DatabaseHelper.cs               # Centralized DB operations
├── 📄 SQLQuery1.sql                   # Database schema & seed data
│
├── 🔐 Authentication/
│   ├── LoginForm.cs                   # User login UI
│   └── RegisterForm.cs                # New user registration
│
├── 🛡️ Admin/
│   ├── AdminDashboard.cs              # Admin main dashboard
│   ├── ManageUsersForm.cs             # User management
│   ├── ManageVendorsForm.cs           # Vendor approval
│   ├── ManageProductsForm.cs          # Product oversight
│   ├── ManageOrdersForm.cs            # Order tracking
│   ├── ManageCategoriesForm.cs        # Category CRUD
│   └── ManageReviewsForm.cs           # Review moderation
│
├── 🏪 Vendor/
│   ├── VendorDashboard.cs             # Vendor main dashboard
│   ├── VendorProductsForm.cs          # Product management
│   ├── VendorOrdersForm.cs            # Order tracking
│   └── VendorSettingsForm.cs          # Store settings
│
└── 🛒 Customer/
    ├── CustomerDashboard.cs           # Shopping interface
    ├── CustomerOrdersForm.cs          # Order history
    ├── CustomerProfileForm.cs         # Profile management
    └── WriteReviewForm.cs             # Product reviews
```

---

## 👥 User Roles

### 🛡️ Administrator
- Full system access
- Approves/rejects vendors
- Manages users, categories, products, orders, reviews
- Views analytics & reports

### 🏪 Vendor
- Manages own product inventory
- Tracks & updates own orders
- Views sales analytics
- Customizes storefront

### 🛒 Customer
- Browses & purchases products
- Manages shopping cart
- Tracks orders
- Writes product reviews
- Updates profile

---

## 📸 Screenshots

> *(Add your screenshots here)*

| Login Page | Customer Dashboard | Vendor Dashboard | Admin Dashboard |
|------------|--------------------|-----------------|----------------|
| ![Login](screenshots/login.png) | ![Customer](screenshots/customer.png) | ![Vendor](screenshots/vendor.png) | ![Admin](screenshots/admin.png) |

---

## 🎨 UI/UX Highlights

- 🎨 **Color-coded role-based theming**:
  - 🛡️ Admin → Deep Indigo Blue
  - 🏪 Vendor → Teal Green
  - 🛒 Customer → Vibrant Pink
- 🕒 Real-time clock display in dashboards
- 📊 Statistical cards with visual indicators
- 📋 Sortable & filterable data grids
- ✅ Toast-style confirmation dialogs
- 🌐 Modern flat-UI design with hover effects

---

## 🛡️ Security Features

- ✅ Parameterized SQL queries (SQL injection prevention)
- ✅ Role-based access control
- ✅ Active/inactive user toggling
- ✅ Vendor verification workflow
- ✅ Password masking (with toggle visibility)
- ✅ Form-level input validation

---

## 🚧 Future Enhancements

- 🔒 Password hashing (BCrypt / SHA-256)
- 🖼️ Product image upload support
- 📧 Email notifications for orders
- 💳 Real payment gateway integration
- 📱 Web/Mobile version using ASP.NET Core + Blazor
- 📊 Advanced charting (using LiveCharts or ChartJS)
- 🌐 Multi-language localization
- 📦 Bulk product import via CSV/Excel

---

## 🤝 Contributing

Contributions are welcome! Follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/AmazingFeature`
3. Commit your changes: `git commit -m 'Add some AmazingFeature'`
4. Push to the branch: `git push origin feature/AmazingFeature`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

**Your Name**  
📧 Email: your.email@example.com  
🐙 GitHub: [@your-username](https://github.com/your-username)  
🔗 LinkedIn: [Your Profile](https://linkedin.com/in/your-profile)

**Project Repository:** [https://github.com/your-username/MultiVendorEcommerce](https://github.com/your-username/MultiVendorEcommerce)

---

## 🙏 Acknowledgments

- Microsoft .NET Framework Documentation
- SQL Server Documentation
- Open-source community for inspiration
- All contributors who helped shape this project

---

<div align="center">

### ⭐ If you found this project helpful, please give it a star!

Made with ❤️ using C# & SQL Server

</div>
