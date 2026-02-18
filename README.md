# My Store - Product Inventory Management

A full-stack product inventory management application that synchronizes products from FakeStoreAPI and allows custom price management.

## 🏗️ Architecture

```
my-store/
├── dotnet/                 # .NET 8 Web API Backend
│   ├── Controllers/        # API Controllers
│   ├── Database/          # MongoDB Context
│   ├── Models/            # Data Models
│   └── Program.cs         # Application entry point
│
└── ui/                    # Next.js 14 Frontend
    ├── src/
    │   ├── app/          # Next.js App Router
    │   ├── components/   # React Components
    │   └── lib/          # Utilities
    └── .next/            # Build output
```

## 🚀 Tech Stack

### Backend
- **.NET 8** - Web API framework
- **MongoDB** - Document database (via MongoDB.Driver)
- **FakeStoreAPI** - External product data source

### Frontend
- **Next.js 14** - React framework with App Router
- **TypeScript** - Type-safe JavaScript
- **Tailwind CSS** - Utility-first styling
- **shadcn/ui** - UI component library
- **Sonner** - Toast notifications

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud)

## 🛠️ Setup & Installation

### 1. Clone the Repository

```bash
git clone https://github.com/khizarahmedb/my-store.git
cd my-store
```

### 2. Backend Setup

```bash
cd dotnet

# Restore dependencies
dotnet restore

# Set environment variables (or use user-secrets)
export MongoDbSettings__ConnectionString="mongodb://localhost:27017"
export MongoDbSettings__DatabaseName="my-store"

# Run the API
dotnet run
```

The API will be available at `http://localhost:5177`

### 3. Frontend Setup

```bash
cd ui

# Install dependencies
npm install

# Run development server
npm run dev
```

The UI will be available at `http://localhost:3000`

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/products` | Get all products (merged from DB + FakeStoreAPI) |
| GET | `/products/{id}` | Get single product by ID |
| GET | `/products/initialize` | Fetch and sync products from FakeStoreAPI |
| PUT | `/products/{id}` | Update product price |

## 📸 Features

### Current Features
- ✅ Product inventory display with grid layout
- ✅ Real-time price editing with dialog modal
- ✅ Product data synchronization from FakeStoreAPI
- ✅ MongoDB persistence for custom pricing
- ✅ Toast notifications for user feedback
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Loading states and error handling

### Product Model
```csharp
public class ProductModel
{
    public ObjectId id { get; set; }
    public int ApiId { get; set; }
    public string description { get; set; }
    public double price { get; set; }
    public string category { get; set; }
    public string image { get; set; }
}
```

## 🧪 Testing

### Backend Tests
```bash
cd dotnet
dotnet test
```

### Frontend Tests
```bash
cd ui
npm test
```

## 🚢 Deployment

### Backend Deployment
```bash
cd dotnet
dotnet publish -c Release
```

### Frontend Deployment (Vercel)
```bash
cd ui
vercel --prod
```

---

## 💡 Notes

- The application uses FakeStoreAPI as the initial data source
- Product images are served from the external API
- Custom pricing is stored in MongoDB and takes precedence over API prices
- The `initialize` endpoint syncs new products from FakeStoreAPI without duplicating existing ones
