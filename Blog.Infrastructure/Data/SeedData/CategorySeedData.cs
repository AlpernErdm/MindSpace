using Blog.Domain.Entities;

namespace Blog.Infrastructure.Data.SeedData;

public static class CategorySeedData
{
    public static readonly Category[] Categories = new[]
    {
        new Category 
        { 
            Name = "Teknoloji", 
            Slug = "teknoloji",
            Description = "En son teknoloji trendleri, yenilikler ve gelişmeleri hakkında güncel bilgiler",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2570/2570579.png",
            Color = "#3B82F6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Yapay Zeka", 
            Slug = "yapay-zeka",
            Description = "AI teknolojileri, machine learning ve deep learning uygulamaları",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#EC4899",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Blockchain", 
            Slug = "blockchain",
            Description = "Blockchain teknolojisi, kripto para ve decentralized uygulamalar",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/6001/6001368.png",
            Color = "#F7931A",
            PostCount = 0
        },
        new Category 
        { 
            Name = "IoT", 
            Slug = "iot",
            Description = "Internet of Things, akıllı cihazlar ve bağlantılı sistemler",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3059/3059997.png",
            Color = "#10B981",
            PostCount = 0
        },
        new Category 
        { 
            Name = "5G Teknolojisi", 
            Slug = "5g-teknolojisi",
            Description = "5G ağları, mobil teknolojiler ve gelecek nesil iletişim",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3059/3059997.png",
            Color = "#8B5CF6",
            PostCount = 0
        },

        // Yazılım Geliştirme Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Yazılım Geliştirme", 
            Slug = "yazilim-gelistirme",
            Description = "Programlama dilleri, yazılım mimarisi, design patterns ve best practices",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1006/1006771.png",
            Color = "#10B981",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Clean Code", 
            Slug = "clean-code",
            Description = "Temiz kod yazma prensipleri, refactoring ve kod kalitesi",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1006/1006771.png",
            Color = "#059669",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Design Patterns", 
            Slug = "design-patterns",
            Description = "Yazılım tasarım desenleri ve uygulama örnekleri",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1006/1006771.png",
            Color = "#DC2626",
            PostCount = 0
        },
        new Category 
        { 
            Name = "SOLID Prensipleri", 
            Slug = "solid-prensipleri",
            Description = "SOLID prensipleri ve object-oriented tasarım",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1006/1006771.png",
            Color = "#7C3AED",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Test Driven Development", 
            Slug = "test-driven-development",
            Description = "TDD yaklaşımı, unit testing ve test stratejileri",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1006/1006771.png",
            Color = "#F59E0B",
            PostCount = 0
        },

        // Web Development Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Web Development", 
            Slug = "web-development",
            Description = "Frontend ve backend web geliştirme teknolojileri, frameworks ve araçlar",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1005/1005141.png",
            Color = "#F59E0B",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Frontend Development", 
            Slug = "frontend-development",
            Description = "HTML, CSS, JavaScript ve modern frontend framework'leri",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1005/1005141.png",
            Color = "#3B82F6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Backend Development", 
            Slug = "backend-development",
            Description = "Server-side programming, API development ve database yönetimi",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1005/1005141.png",
            Color = "#EF4444",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Full Stack Development", 
            Slug = "full-stack-development",
            Description = "End-to-end web uygulama geliştirme ve deployment",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1005/1005141.png",
            Color = "#10B981",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Progressive Web Apps", 
            Slug = "progressive-web-apps",
            Description = "PWA teknolojileri ve modern web uygulamaları",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1005/1005141.png",
            Color = "#8B5CF6",
            PostCount = 0
        },

        // Mobile Development Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Mobile Development", 
            Slug = "mobile-development",
            Description = "iOS, Android ve cross-platform mobile uygulama geliştirme rehberleri",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2991/2991110.png",
            Color = "#8B5CF6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "iOS Development", 
            Slug = "ios-development",
            Description = "Swift, SwiftUI ve iOS uygulama geliştirme",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2991/2991110.png",
            Color = "#000000",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Android Development", 
            Slug = "android-development",
            Description = "Kotlin, Jetpack Compose ve Android uygulama geliştirme",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2991/2991110.png",
            Color = "#3DDC84",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Cross-Platform Development", 
            Slug = "cross-platform-development",
            Description = "React Native, Flutter ve Xamarin ile cross-platform geliştirme",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2991/2991110.png",
            Color = "#02569B",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Mobile UI/UX", 
            Slug = "mobile-ui-ux",
            Description = "Mobile kullanıcı arayüzü tasarımı ve kullanıcı deneyimi",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2991/2991110.png",
            Color = "#EC4899",
            PostCount = 0
        },

        // DevOps Kategorisi - 5 seed data
        new Category 
        { 
            Name = "DevOps", 
            Slug = "devops",
            Description = "CI/CD pipeline'ları, containerization, deployment ve infrastructure as code",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/888/888839.png",
            Color = "#EF4444",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Continuous Integration", 
            Slug = "continuous-integration",
            Description = "CI/CD pipeline'ları, automated testing ve build processes",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/888/888839.png",
            Color = "#FF6B6B",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Containerization", 
            Slug = "containerization",
            Description = "Docker, Kubernetes ve container orchestration",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/888/888839.png",
            Color = "#2496ED",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Infrastructure as Code", 
            Slug = "infrastructure-as-code",
            Description = "Terraform, Ansible ve infrastructure automation",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/888/888839.png",
            Color = "#7C3AED",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Monitoring & Logging", 
            Slug = "monitoring-logging",
            Description = "Application monitoring, logging ve observability",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/888/888839.png",
            Color = "#10B981",
            PostCount = 0
        },

        // Artificial Intelligence Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Artificial Intelligence", 
            Slug = "artificial-intelligence",
            Description = "AI, machine learning, deep learning ve data science konuları",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#EC4899",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Machine Learning", 
            Slug = "machine-learning",
            Description = "ML algoritmaları, model training ve prediction",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#FF6B6B",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Deep Learning", 
            Slug = "deep-learning",
            Description = "Neural networks, CNN, RNN ve deep learning frameworks",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#00D4FF",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Natural Language Processing", 
            Slug = "natural-language-processing",
            Description = "NLP, text processing ve language models",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#8B5CF6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Computer Vision", 
            Slug = "computer-vision",
            Description = "Image processing, object detection ve computer vision",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2103/2103633.png",
            Color = "#F59E0B",
            PostCount = 0
        },

        // Cloud Computing Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Cloud Computing", 
            Slug = "cloud-computing",
            Description = "AWS, Azure, Google Cloud ve cloud architecture patterns",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067356.png",
            Color = "#06B6D4",
            PostCount = 0
        },
        new Category 
        { 
            Name = "AWS Services", 
            Slug = "aws-services",
            Description = "Amazon Web Services ve cloud computing çözümleri",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067356.png",
            Color = "#FF9900",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Azure Platform", 
            Slug = "azure-platform",
            Description = "Microsoft Azure ve enterprise cloud solutions",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067356.png",
            Color = "#0089D6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Google Cloud", 
            Slug = "google-cloud",
            Description = "Google Cloud Platform ve AI-first cloud services",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067356.png",
            Color = "#4285F4",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Serverless Computing", 
            Slug = "serverless-computing",
            Description = "Serverless architecture ve function-as-a-service",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067356.png",
            Color = "#10B981",
            PostCount = 0
        },

        // Database Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Database", 
            Slug = "database",
            Description = "SQL, NoSQL, database design, optimization ve management",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/4248/4248443.png",
            Color = "#84CC16",
            PostCount = 0
        },
        new Category 
        { 
            Name = "SQL Databases", 
            Slug = "sql-databases",
            Description = "Relational databases, SQL ve ACID properties",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/4248/4248443.png",
            Color = "#E48E00",
            PostCount = 0
        },
        new Category 
        { 
            Name = "NoSQL Databases", 
            Slug = "nosql-databases",
            Description = "Non-relational databases ve distributed data storage",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/4248/4248443.png",
            Color = "#4DB33D",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Database Design", 
            Slug = "database-design",
            Description = "Database modeling, normalization ve schema design",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/4248/4248443.png",
            Color = "#336791",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Database Performance", 
            Slug = "database-performance",
            Description = "Query optimization, indexing ve performance tuning",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/4248/4248443.png",
            Color = "#DC2626",
            PostCount = 0
        },

        // Security Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Security", 
            Slug = "security",
            Description = "Cybersecurity, application security, penetration testing ve best practices",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2092/2092663.png",
            Color = "#DC2626",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Application Security", 
            Slug = "application-security",
            Description = "Web application security, OWASP ve secure coding",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2092/2092663.png",
            Color = "#EF4444",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Network Security", 
            Slug = "network-security",
            Description = "Network protection, firewall ve intrusion detection",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2092/2092663.png",
            Color = "#7C3AED",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Cryptography", 
            Slug = "cryptography",
            Description = "Encryption, hashing ve cryptographic protocols",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2092/2092663.png",
            Color = "#F59E0B",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Penetration Testing", 
            Slug = "penetration-testing",
            Description = "Ethical hacking, vulnerability assessment ve security testing",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/2092/2092663.png",
            Color = "#10B981",
            PostCount = 0
        },

        // Career Kategorisi - 5 seed data
        new Category 
        { 
            Name = "Career", 
            Slug = "career",
            Description = "Kariyer gelişimi, mülakat tüyoları, professional development ve networking",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png",
            Color = "#059669",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Job Interviews", 
            Slug = "job-interviews",
            Description = "Mülakat hazırlığı, teknik sorular ve interview strategies",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png",
            Color = "#DC2626",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Skill Development", 
            Slug = "skill-development",
            Description = "Yazılım becerileri geliştirme ve learning paths",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png",
            Color = "#3B82F6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Leadership", 
            Slug = "leadership",
            Description = "Tech leadership, team management ve soft skills",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png",
            Color = "#8B5CF6",
            PostCount = 0
        },
        new Category 
        { 
            Name = "Freelancing", 
            Slug = "freelancing",
            Description = "Freelance yazılım geliştirme ve remote work",
            IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png",
            Color = "#F59E0B",
            PostCount = 0
        }
    };
} 