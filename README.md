# 🚀 Medium Clone - Modern Blog Platform

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15.4.4-black.svg)](https://nextjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue.svg)](https://www.typescriptlang.org/)
[![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.4.17-38B2AC.svg)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Modern bir blog platformu olan Medium'un klonu. Clean Architecture pattern'i kullanılarak geliştirilmiş, .NET 9 backend ve Next.js frontend ile oluşturulmuş kapsamlı bir web uygulaması.

## ✨ Özellikler

### 🔐 Kimlik Doğrulama & Yetkilendirme
- JWT token tabanlı kimlik doğrulama
- Role-based authorization (User, Author, Admin)
- Refresh token mekanizması
- Güvenli şifre validasyonu

### 📝 İçerik Yönetimi
- CRUD işlemleri (Create, Read, Update, Delete)
- Draft/Published durum yönetimi
- Zengin metin editörü
- Resim yükleme desteği
- SEO optimizasyonu (meta tags, slugs)

### 🏷️ Sosyal Özellikler
- Post ve yorum beğenme sistemi
- İç içe yorum sistemi
- Kullanıcı takip sistemi
- Gerçek zamanlı bildirimler

### 🔍 Arama & Keşfetme
- Elasticsearch ile tam metin arama
- Kategori bazlı filtreleme
- Etiket bazlı filtreleme
- Yazar bazlı filtreleme

### ⚡ Gerçek Zamanlı Özellikler
- SignalR ile gerçek zamanlı bildirimler
- Canlı yorum güncellemeleri
- Kullanıcı varlık göstergeleri

## 🏗️ Mimari

Bu proje **Clean Architecture** pattern'i kullanılarak geliştirilmiştir:

```
📁 Blog.Domain          # Entities, Business Rules
📁 Blog.Application     # Use Cases, DTOs, Interfaces
📁 Blog.Infrastructure # Data Access, External Services
📁 Blog.API           # Web API, Controllers
📁 blog-frontend      # Next.js Frontend
```

### Teknoloji Stack'i

#### Backend (.NET 9)
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM
- **Identity**: Authentication & Authorization
- **SignalR**: Real-time communication
- **JWT**: Token-based authentication
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation

#### Frontend (Next.js 15)
- **React 19**: UI library
- **TypeScript**: Type safety
- **TailwindCSS**: Utility-first CSS
- **Lucide React**: Icons
- **Axios**: HTTP client
- **React Hook Form**: Form handling
- **Zod**: Schema validation

#### Database & Storage
- **SQL Server**: Ana veritabanı
- **Elasticsearch**: Full-text search
- **RabbitMQ**: Message broker

## 🚀 Kurulum

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### 1. Repository'yi Klonlayın

```bash
git clone https://github.com/yourusername/medium-clone.git
cd medium-clone
```

### 2. Infrastructure Servislerini Başlatın

```bash
docker-compose up -d
```

Bu komut şu servisleri başlatacak:
- SQL Server (Port: 1433)
- RabbitMQ (Port: 5672, Management UI: 15672)
- Elasticsearch (Port: 9200)
- Kibana (Port: 5601)

### 3. Backend API'yi Çalıştırın

```bash
cd Blog.API
dotnet restore
dotnet run
```

API şu adreste çalışacak: `https://localhost:7237`

### 4. Frontend'i Çalıştırın

```bash
cd blog-frontend
npm install
npm run dev
```

Frontend şu adreste çalışacak: `http://localhost:3000`

## 📊 Veritabanı Şeması

### Ana Entity'ler

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
```
POST /api/auth/register     # Kullanıcı kaydı
POST /api/auth/login        # Kullanıcı girişi
POST /api/auth/refresh-token # Token yenileme
POST /api/auth/logout       # Çıkış yapma
GET  /api/auth/me          # Mevcut kullanıcı bilgileri
```

### Posts
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
```
GET    /api/comments/post/{postId}  # Post yorumları
POST   /api/comments                # Yorum ekle
PUT    /api/comments/{id}           # Yorum güncelle
DELETE /api/comments/{id}           # Yorum sil
```

### Search
```
GET /api/search?query={query}       # Tam metin arama
```

## 🐳 Docker Deployment

### Production Build

```bash
# Backend build
docker build -t medium-clone-api ./Blog.API

# Frontend build
docker build -t medium-clone-frontend ./blog-frontend

# Run with docker-compose
docker-compose -f docker-compose.prod.yml up -d
```

### Environment Variables

```env
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

## 🧪 Test

### Backend Tests

```bash
# Unit tests
dotnet test

# Integration tests
dotnet test --filter Category=Integration
```

### Frontend Tests

```bash
cd blog-frontend
npm test
```

## 📈 Performance Optimizations

- **Database Indexing**: Optimized queries
- **Pagination**: Large dataset handling
- **Caching**: Memory caching
- **Async/Await**: Non-blocking operations
- **SignalR**: Real-time without polling

## 🔒 Security Features

- **JWT Authentication**: Secure token-based auth
- **Role-based Authorization**: Fine-grained permissions
- **Input Validation**: Server-side validation
- **CORS**: Cross-origin resource sharing
- **HTTPS**: Secure communication

## 🤝 Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

### Development Guidelines

- Clean Architecture prensiplerini takip edin
- Unit testler yazın
- Code review sürecine katılın
- Conventional commits kullanın

## 📝 License

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🙏 Teşekkürler

- [ASP.NET Core](https://dotnet.microsoft.com/) - Backend framework
- [Next.js](https://nextjs.org/) - Frontend framework
- [TailwindCSS](https://tailwindcss.com/) - CSS framework
- [Entity Framework](https://docs.microsoft.com/en-us/ef/) - ORM
- [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr) - Real-time communication

## 📞 İletişim

- **Proje Linki**: [https://github.com/yourusername/medium-clone](https://github.com/yourusername/medium-clone)
- **Issues**: [https://github.com/yourusername/medium-clone/issues](https://github.com/yourusername/medium-clone/issues)

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın! 