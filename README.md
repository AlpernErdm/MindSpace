
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15.4.4-black.svg)](https://nextjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue.svg)](https://www.typescriptlang.org/)
[![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.4.17-38B2AC.svg)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
Modern blog platformunun backend API'si. Clean Architecture pattern'i kullanılarak .NET 9 ile geliştirilmiştir.


- JWT token tabanlı kimlik doğrulama
- Role-based authorization (User, Author, Admin)
- Refresh token mekanizması







```
📁 Blog.Domain          # Entities, Business Rules
📁 Blog.Application     # Use Cases, DTOs, Interfaces
📁 Blog.Infrastructure # Data Access, External Services
📁 Blog.API           # Web API, Controllers
📁 blog-frontend      # Next.js Frontend
```


- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM
- **Identity**: Authentication & Authorization
- **SignalR**: Real-time communication
- **JWT**: Token-based authentication
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation



## 🚀 Kurulum

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)


1. **Clone the repository**
```bash
git clone https://github.com/yourusername/medium-clone.git
cd medium-clone
```

```bash
```


```bash
cd Blog.API
```

```bash
```




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
POST /api/auth/register     # Kullanıcı kaydı
POST /api/auth/login        # Kullanıcı girişi
POST /api/auth/refresh-token # Token yenileme
POST /api/auth/logout       # Çıkış yapma
GET  /api/auth/me          # Mevcut kullanıcı bilgileri
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
GET    /api/posts                    # Tüm yayınlanmış postlar
GET    /api/posts/{id}              # Post detayı (ID ile)
GET    /api/posts/slug/{slug}       # Post detayı (Slug ile)
POST   /api/posts                   # Yeni post oluştur
PUT    /api/posts/{id}              # Post güncelle
DELETE /api/posts/{id}              # Post sil
POST   /api/posts/{id}/publish      # Post yayınla
POST   /api/posts/{id}/unpublish    # Post'u draft'a çevir
```

### Comments

```http
GET    /api/comments/post/{postId}
POST   /api/comments
PUT    /api/comments/{id}
DELETE /api/comments/{id}
```
GET    /api/comments/post/{postId}  # Post yorumları
POST   /api/comments                # Yorum ekle
PUT    /api/comments/{id}           # Yorum güncelle
DELETE /api/comments/{id}           # Yorum sil
```

### Search

```http
GET /api/search?query={query}
```
```





```





```



```bash
dotnet test

dotnet test --filter Category=Integration
```


```bash
```


- **Pagination**: Large dataset handling
- **Async/Await**: Non-blocking operations
- **SignalR**: Real-time without polling













---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!
