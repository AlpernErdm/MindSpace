# 🔧 Blog API - .NET 9 Backend

Modern blog platformunun backend API'si. Clean Architecture pattern'i kullanılarak .NET 9 ile geliştirilmiştir.

## 🚀 Özellikler

### 🔐 Authentication & Authorization
- JWT token tabanlı kimlik doğrulama
- Role-based authorization (User, Author, Admin)
- Refresh token mekanizması
- ASP.NET Core Identity entegrasyonu

### 📝 Content Management
- CRUD operations for posts
- Draft/Published status management
- Rich text content support
- Image upload handling
- SEO optimization (meta tags, slugs)

### 🏷️ Social Features
- Like/Unlike posts and comments
- Nested comment system
- User following system
- Real-time notifications

### 🔍 Search & Discovery
- Full-text search with Elasticsearch
- Category-based filtering
- Tag-based filtering
- Author-based filtering

### ⚡ Real-time Features
- SignalR for real-time notifications
- Live comment updates
- User presence indicators

## 🏗️ Architecture

Bu proje Clean Architecture pattern'i kullanır:

```
📁 Blog.Domain          # Entities, Business Rules
📁 Blog.Application     # Use Cases, DTOs, Interfaces
📁 Blog.Infrastructure # Data Access, External Services
📁 Blog.API           # Web API, Controllers
```

### Technology Stack

- **.NET 9**: Runtime ve framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM
- **Identity**: Authentication & Authorization
- **SignalR**: Real-time communication
- **JWT**: Token-based authentication
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation

## 📦 Installation

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/) (optional)

### Setup

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/medium-clone.git
cd medium-clone
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure database connection**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MediumClone;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

4. **Run database migrations**
```bash
cd Blog.API
dotnet ef database update
```

5. **Start the application**
```bash
dotnet run
```

API şu adreste çalışacak: `https://localhost:7237`

## 📊 Database Schema

### Core Entities

```sql
-- Users (ASP.NET Identity)
Users (Id, UserName, Email, FirstName, LastName, Bio, etc.)

-- Posts
Posts (Id, Title, Slug, Content, AuthorId, CategoryId, Status, etc.)

-- Comments
Comments (Id, Content, PostId, AuthorId, ParentCommentId, etc.)

-- Likes
Likes (Id, UserId, PostId/CommentId, Type)

-- Categories & Tags
Categories (Id, Name, Slug, Description)
Tags (Id, Name, Slug, Description)
PostTags (PostId, TagId)

-- Social
UserFollows (FollowerId, FollowingId)
Notifications (Id, UserId, Type, Message, etc.)
```

## 🔧 API Endpoints

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
POST /api/auth/logout
GET  /api/auth/me
```

### Posts

```http
GET    /api/posts
GET    /api/posts/{id}
GET    /api/posts/slug/{slug}
POST   /api/posts
PUT    /api/posts/{id}
DELETE /api/posts/{id}
POST   /api/posts/{id}/publish
POST   /api/posts/{id}/unpublish
```

### Comments

```http
GET    /api/comments/post/{postId}
POST   /api/comments
PUT    /api/comments/{id}
DELETE /api/comments/{id}
```

### Search

```http
GET /api/search?query={query}
```

## 🔐 Authentication

### JWT Configuration

```json
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here",
    "Issuer": "https://localhost:7237",
    "Audience": "https://localhost:7237",
    "ExpiryInDays": 1
  }
}
```

### Token Structure

```json
{
  "sub": "user-id",
  "name": "username",
  "email": "user@example.com",
  "role": "Author",
  "exp": 1640995200,
  "iat": 1640908800
}
```

## ⚡ SignalR Integration

### Hub Configuration

```csharp
// Program.cs
builder.Services.AddSignalR();

app.MapHub<NotificationHub>("/notificationHub");
```

### Client Connection

```javascript
// Frontend
const connection = new HubConnectionBuilder()
  .withUrl("https://localhost:7237/notificationHub", {
    accessTokenFactory: () => token
  })
  .build();
```

## 🔍 Search with Elasticsearch

### Index Configuration

```csharp
// ElasticsearchService.cs
public async Task InitializeAsync()
{
    var indexName = "posts";
    var indexExists = await _client.Indices.ExistsAsync(indexName);
    
    if (!indexExists.Exists)
    {
        var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
            .Map<PostSearchDocument>(m => m
                .Properties(p => p
                    .Text(t => t.Name(n => n.Title))
                    .Text(t => t.Name(n => n.Content))
                    .Keyword(t => t.Name(n => n.AuthorId))
                )
            )
        );
    }
}
```

## 🐳 Docker Support

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Blog.API/Blog.API.csproj", "Blog.API/"]
COPY ["Blog.Application/Blog.Application.csproj", "Blog.Application/"]
COPY ["Blog.Infrastructure/Blog.Infrastructure.csproj", "Blog.Infrastructure/"]
COPY ["Blog.Domain/Blog.Domain.csproj", "Blog.Domain/"]
RUN dotnet restore "Blog.API/Blog.API.csproj"
COPY . .
WORKDIR "/src/Blog.API"
RUN dotnet build "Blog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.API.dll"]
```

### Docker Compose

```yaml
# docker-compose.yml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "7237:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MediumClone;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true
    depends_on:
      - sqlserver
      - elasticsearch
      - rabbitmq
```

## 🧪 Testing

### Unit Tests

```bash
# Run all tests
dotnet test

# Run specific project tests
dotnet test Blog.Application.Tests
dotnet test Blog.Infrastructure.Tests
```

### Integration Tests

```bash
# Run integration tests
dotnet test --filter Category=Integration
```

### API Tests

```bash
# Using the provided .http file
dotnet run --project Blog.API
# Then use the Blog.API.http file in VS Code or similar
```

## 📈 Performance

### Optimization Techniques

- **Database Indexing**: Optimized queries with EF Core
- **Pagination**: Large dataset handling
- **Caching**: Memory caching with IMemoryCache
- **Async/Await**: Non-blocking operations
- **SignalR**: Real-time without polling

### Monitoring

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContext<BlogDbContext>()
    .AddElasticsearch()
    .AddRabbitMQ();
```

## 🔒 Security

### CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### Input Validation

```csharp
// Using FluentValidation
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
            
        RuleFor(x => x.Content)
            .NotEmpty()
            .MinimumLength(10);
    }
}
```

## 📝 Logging

### Structured Logging

```csharp
// Program.cs
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddSeq();
});
```

### Log Levels

```json
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Blog.API": "Debug"
    }
  }
}
```

## 🚀 Deployment

### Production Configuration

```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql;Database=MediumClone;User Id=sa;Password=ProdPassword123!;TrustServerCertificate=true"
  },
  "JwtSettings": {
    "SecretKey": "production-secret-key"
  },
  "Elasticsearch": {
    "Url": "http://prod-elasticsearch:9200"
  }
}
```

### Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection=Server=localhost;Database=MediumClone;Trusted_Connection=true;TrustServerCertificate=true;

# JWT
JwtSettings__SecretKey=your-super-secret-key-here
JwtSettings__Issuer=https://localhost:7237
JwtSettings__Audience=https://localhost:7237

# Elasticsearch
Elasticsearch__Url=http://localhost:9200

# RabbitMQ
RabbitMQ__Host=localhost
RabbitMQ__Username=admin
RabbitMQ__Password=admin123
```

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

### Development Guidelines

- Follow Clean Architecture principles
- Write unit tests for business logic
- Use meaningful commit messages
- Follow C# coding conventions

## 📝 License

MIT License - see [LICENSE](LICENSE) for details.

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!
