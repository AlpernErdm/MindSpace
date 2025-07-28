using Blog.Domain.Entities;

namespace Blog.Infrastructure.Data.SeedData;

/// <summary>
/// Post seed data - Her kategori için 5 yazı
/// </summary>
public static class PostSeedData
{
    public static readonly PostTemplate[] PostTemplates = new[]
    {
        // Teknoloji Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "2024'te Teknoloji Trendleri ve Gelecek Tahminleri", 
            Content = GetTeknolojiContent1(), 
            CategoryName = "Teknoloji", 
            TagNames = new[] { "teknoloji", "trend", "2024", "innovation" } 
        },
        new PostTemplate 
        { 
            Title = "Quantum Computing: Geleceğin Hesaplama Gücü", 
            Content = GetTeknolojiContent2(), 
            CategoryName = "Teknoloji", 
            TagNames = new[] { "quantum", "computing", "teknoloji", "future" } 
        },
        new PostTemplate 
        { 
            Title = "Metaverse: Sanal Dünyaların Yükselişi", 
            Content = GetTeknolojiContent3(), 
            CategoryName = "Teknoloji", 
            TagNames = new[] { "metaverse", "vr", "ar", "teknoloji" } 
        },
        new PostTemplate 
        { 
            Title = "Edge Computing: Bulut Bilişimin Yeni Yüzü", 
            Content = GetTeknolojiContent4(), 
            CategoryName = "Teknoloji", 
            TagNames = new[] { "edge", "computing", "cloud", "teknoloji" } 
        },
        new PostTemplate 
        { 
            Title = "Green Tech: Sürdürülebilir Teknoloji Çözümleri", 
            Content = GetTeknolojiContent5(), 
            CategoryName = "Teknoloji", 
            TagNames = new[] { "green", "tech", "sustainability", "teknoloji" } 
        },

        // Yapay Zeka Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "ChatGPT ve Generative AI'nin İş Dünyasına Etkisi", 
            Content = GetAIContent1(), 
            CategoryName = "Yapay Zeka", 
            TagNames = new[] { "chatgpt", "ai", "generative", "business" } 
        },
        new PostTemplate 
        { 
            Title = "Computer Vision: Görüntü İşleme Teknolojileri", 
            Content = GetAIContent2(), 
            CategoryName = "Yapay Zeka", 
            TagNames = new[] { "computer", "vision", "ai", "image" } 
        },
        new PostTemplate 
        { 
            Title = "Natural Language Processing: Dil İşleme Teknolojileri", 
            Content = GetAIContent3(), 
            CategoryName = "Yapay Zeka", 
            TagNames = new[] { "nlp", "language", "ai", "processing" } 
        },
        new PostTemplate 
        { 
            Title = "AI Ethics: Yapay Zeka ve Etik Sorunlar", 
            Content = GetAIContent4(), 
            CategoryName = "Yapay Zeka", 
            TagNames = new[] { "ai", "ethics", "responsible", "ai" } 
        },
        new PostTemplate 
        { 
            Title = "Machine Learning Model Deployment Stratejileri", 
            Content = GetAIContent5(), 
            CategoryName = "Yapay Zeka", 
            TagNames = new[] { "ml", "deployment", "ai", "production" } 
        },

        // Blockchain Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Bitcoin ve Kripto Para Ekosistemi", 
            Content = GetBlockchainContent1(), 
            CategoryName = "Blockchain", 
            TagNames = new[] { "bitcoin", "crypto", "blockchain", "finance" } 
        },
        new PostTemplate 
        { 
            Title = "Smart Contracts: Akıllı Sözleşme Teknolojisi", 
            Content = GetBlockchainContent2(), 
            CategoryName = "Blockchain", 
            TagNames = new[] { "smart", "contracts", "ethereum", "blockchain" } 
        },
        new PostTemplate 
        { 
            Title = "DeFi: Merkeziyetsiz Finans Sistemleri", 
            Content = GetBlockchainContent3(), 
            CategoryName = "Blockchain", 
            TagNames = new[] { "defi", "finance", "blockchain", "crypto" } 
        },
        new PostTemplate 
        { 
            Title = "NFT: Dijital Varlık Teknolojisi", 
            Content = GetBlockchainContent4(), 
            CategoryName = "Blockchain", 
            TagNames = new[] { "nft", "digital", "assets", "blockchain" } 
        },
        new PostTemplate 
        { 
            Title = "Web3: İnternetin Geleceği", 
            Content = GetBlockchainContent5(), 
            CategoryName = "Blockchain", 
            TagNames = new[] { "web3", "blockchain", "internet", "future" } 
        },

        // IoT Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "IoT Cihazları ve Akıllı Ev Sistemleri", 
            Content = GetIoTContent1(), 
            CategoryName = "IoT", 
            TagNames = new[] { "iot", "smart", "home", "devices" } 
        },
        new PostTemplate 
        { 
            Title = "Industrial IoT: Endüstri 4.0 ve Akıllı Fabrikalar", 
            Content = GetIoTContent2(), 
            CategoryName = "IoT", 
            TagNames = new[] { "iiot", "industry", "4.0", "smart" } 
        },
        new PostTemplate 
        { 
            Title = "IoT Güvenlik: Bağlantılı Cihazların Güvenliği", 
            Content = GetIoTContent3(), 
            CategoryName = "IoT", 
            TagNames = new[] { "iot", "security", "cybersecurity", "devices" } 
        },
        new PostTemplate 
        { 
            Title = "IoT Veri Analizi ve Big Data", 
            Content = GetIoTContent4(), 
            CategoryName = "IoT", 
            TagNames = new[] { "iot", "data", "analytics", "bigdata" } 
        },
        new PostTemplate 
        { 
            Title = "IoT Protokol Mimarisi ve Standartları", 
            Content = GetIoTContent5(), 
            CategoryName = "IoT", 
            TagNames = new[] { "iot", "protocols", "architecture", "standards" } 
        },

        // 5G Teknolojisi Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "5G Teknolojisi ve Mobil İletişimin Geleceği", 
            Content = Get5GContent1(), 
            CategoryName = "5G Teknolojisi", 
            TagNames = new[] { "5g", "mobile", "telecom", "network" } 
        },
        new PostTemplate 
        { 
            Title = "5G Network Slicing ve Sanal Ağlar", 
            Content = Get5GContent2(), 
            CategoryName = "5G Teknolojisi", 
            TagNames = new[] { "5g", "slicing", "virtual", "network" } 
        },
        new PostTemplate 
        { 
            Title = "5G ve IoT: Bağlantılı Cihazların Gücü", 
            Content = Get5GContent3(), 
            CategoryName = "5G Teknolojisi", 
            TagNames = new[] { "5g", "iot", "connected", "devices" } 
        },
        new PostTemplate 
        { 
            Title = "5G Güvenlik ve Gizlilik Konuları", 
            Content = Get5GContent4(), 
            CategoryName = "5G Teknolojisi", 
            TagNames = new[] { "5g", "security", "privacy", "network" } 
        },
        new PostTemplate 
        { 
            Title = "5G Radyo Teknolojileri ve Spektrum Yönetimi", 
            Content = Get5GContent5(), 
            CategoryName = "5G Teknolojisi", 
            TagNames = new[] { "5g", "radio", "spectrum", "technology" } 
        },

        // Yazılım Geliştirme Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Clean Code Prensipleri ve Best Practices", 
            Content = GetSoftwareDevContent1(), 
            CategoryName = "Yazılım Geliştirme", 
            TagNames = new[] { "clean", "code", "principles", "best", "practices" } 
        },
        new PostTemplate 
        { 
            Title = "Design Patterns: Yazılım Tasarım Desenleri", 
            Content = GetSoftwareDevContent2(), 
            CategoryName = "Yazılım Geliştirme", 
            TagNames = new[] { "design", "patterns", "software", "architecture" } 
        },
        new PostTemplate 
        { 
            Title = "SOLID Prensipleri: Object-Oriented Tasarım", 
            Content = GetSoftwareDevContent3(), 
            CategoryName = "Yazılım Geliştirme", 
            TagNames = new[] { "solid", "principles", "oop", "design" } 
        },
        new PostTemplate 
        { 
            Title = "Test Driven Development (TDD) Yaklaşımı", 
            Content = GetSoftwareDevContent4(), 
            CategoryName = "Yazılım Geliştirme", 
            TagNames = new[] { "tdd", "testing", "development", "agile" } 
        },
        new PostTemplate 
        { 
            Title = "Refactoring: Kod İyileştirme Teknikleri", 
            Content = GetSoftwareDevContent5(), 
            CategoryName = "Yazılım Geliştirme", 
            TagNames = new[] { "refactoring", "code", "improvement", "maintenance" } 
        },

        // Web Development Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Modern Web Development: Frontend ve Backend", 
            Content = GetWebDevContent1(), 
            CategoryName = "Web Development", 
            TagNames = new[] { "web", "development", "frontend", "backend" } 
        },
        new PostTemplate 
        { 
            Title = "Progressive Web Apps (PWA) Teknolojisi", 
            Content = GetWebDevContent2(), 
            CategoryName = "Web Development", 
            TagNames = new[] { "pwa", "progressive", "web", "apps" } 
        },
        new PostTemplate 
        { 
            Title = "Web Performance Optimization Teknikleri", 
            Content = GetWebDevContent3(), 
            CategoryName = "Web Development", 
            TagNames = new[] { "performance", "optimization", "web", "speed" } 
        },
        new PostTemplate 
        { 
            Title = "Web Accessibility: Erişilebilir Web Tasarımı", 
            Content = GetWebDevContent4(), 
            CategoryName = "Web Development", 
            TagNames = new[] { "accessibility", "web", "design", "inclusive" } 
        },
        new PostTemplate 
        { 
            Title = "Web Security: Güvenli Web Uygulamaları", 
            Content = GetWebDevContent5(), 
            CategoryName = "Web Development", 
            TagNames = new[] { "security", "web", "applications", "cybersecurity" } 
        },

        // Mobile Development Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Cross-Platform Mobile Development Stratejileri", 
            Content = GetMobileDevContent1(), 
            CategoryName = "Mobile Development", 
            TagNames = new[] { "mobile", "cross-platform", "development", "strategy" } 
        },
        new PostTemplate 
        { 
            Title = "Native vs Hybrid Mobile App Development", 
            Content = GetMobileDevContent2(), 
            CategoryName = "Mobile Development", 
            TagNames = new[] { "native", "hybrid", "mobile", "development" } 
        },
        new PostTemplate 
        { 
            Title = "Mobile App Performance Optimization", 
            Content = GetMobileDevContent3(), 
            CategoryName = "Mobile Development", 
            TagNames = new[] { "mobile", "performance", "optimization", "apps" } 
        },
        new PostTemplate 
        { 
            Title = "Mobile App Security ve Privacy", 
            Content = GetMobileDevContent4(), 
            CategoryName = "Mobile Development", 
            TagNames = new[] { "mobile", "security", "privacy", "apps" } 
        },
        new PostTemplate 
        { 
            Title = "Mobile App Monetization Stratejileri", 
            Content = GetMobileDevContent5(), 
            CategoryName = "Mobile Development", 
            TagNames = new[] { "mobile", "monetization", "strategy", "business" } 
        },

        // DevOps Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "CI/CD Pipeline: Continuous Integration ve Deployment", 
            Content = GetDevOpsContent1(), 
            CategoryName = "DevOps", 
            TagNames = new[] { "ci-cd", "pipeline", "continuous", "integration" } 
        },
        new PostTemplate 
        { 
            Title = "Container Orchestration: Kubernetes ve Docker", 
            Content = GetDevOpsContent2(), 
            CategoryName = "DevOps", 
            TagNames = new[] { "kubernetes", "docker", "containers", "orchestration" } 
        },
        new PostTemplate 
        { 
            Title = "Infrastructure as Code (IaC) Best Practices", 
            Content = GetDevOpsContent3(), 
            CategoryName = "DevOps", 
            TagNames = new[] { "iac", "infrastructure", "code", "automation" } 
        },
        new PostTemplate 
        { 
            Title = "Monitoring ve Observability: Sistem İzleme", 
            Content = GetDevOpsContent4(), 
            CategoryName = "DevOps", 
            TagNames = new[] { "monitoring", "observability", "logging", "metrics" } 
        },
        new PostTemplate 
        { 
            Title = "DevOps Security: Güvenli DevOps Süreçleri", 
            Content = GetDevOpsContent5(), 
            CategoryName = "DevOps", 
            TagNames = new[] { "devops", "security", "devsecops", "automation" } 
        },

        // Artificial Intelligence Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Deep Learning: Neural Networks ve AI", 
            Content = GetAITechContent1(), 
            CategoryName = "Artificial Intelligence", 
            TagNames = new[] { "deep", "learning", "neural", "networks" } 
        },
        new PostTemplate 
        { 
            Title = "Machine Learning Algorithms: Temel Algoritmalar", 
            Content = GetAITechContent2(), 
            CategoryName = "Artificial Intelligence", 
            TagNames = new[] { "machine", "learning", "algorithms", "ai" } 
        },
        new PostTemplate 
        { 
            Title = "Natural Language Processing: Dil İşleme", 
            Content = GetAITechContent3(), 
            CategoryName = "Artificial Intelligence", 
            TagNames = new[] { "nlp", "natural", "language", "processing" } 
        },
        new PostTemplate 
        { 
            Title = "Computer Vision: Görüntü İşleme ve AI", 
            Content = GetAITechContent4(), 
            CategoryName = "Artificial Intelligence", 
            TagNames = new[] { "computer", "vision", "image", "processing" } 
        },
        new PostTemplate 
        { 
            Title = "AI Model Deployment ve Production", 
            Content = GetAITechContent5(), 
            CategoryName = "Artificial Intelligence", 
            TagNames = new[] { "ai", "model", "deployment", "production" } 
        },

        // Cloud Computing Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Multi-Cloud Strategy: Çoklu Bulut Yaklaşımı", 
            Content = GetCloudContent1(), 
            CategoryName = "Cloud Computing", 
            TagNames = new[] { "multi-cloud", "strategy", "cloud", "architecture" } 
        },
        new PostTemplate 
        { 
            Title = "Serverless Computing: Fonksiyon Tabanlı Mimari", 
            Content = GetCloudContent2(), 
            CategoryName = "Cloud Computing", 
            TagNames = new[] { "serverless", "functions", "cloud", "architecture" } 
        },
        new PostTemplate 
        { 
            Title = "Cloud Security: Bulut Güvenlik Best Practices", 
            Content = GetCloudContent3(), 
            CategoryName = "Cloud Computing", 
            TagNames = new[] { "cloud", "security", "best", "practices" } 
        },
        new PostTemplate 
        { 
            Title = "Cloud Cost Optimization: Maliyet Optimizasyonu", 
            Content = GetCloudContent4(), 
            CategoryName = "Cloud Computing", 
            TagNames = new[] { "cloud", "cost", "optimization", "management" } 
        },
        new PostTemplate 
        { 
            Title = "Edge Computing: Bulut Bilişimin Sınırları", 
            Content = GetCloudContent5(), 
            CategoryName = "Cloud Computing", 
            TagNames = new[] { "edge", "computing", "cloud", "distributed" } 
        },

        // Database Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Database Design: Veritabanı Tasarım Prensipleri", 
            Content = GetDatabaseContent1(), 
            CategoryName = "Database", 
            TagNames = new[] { "database", "design", "principles", "modeling" } 
        },
        new PostTemplate 
        { 
            Title = "NoSQL Databases: Modern Veri Depolama", 
            Content = GetDatabaseContent2(), 
            CategoryName = "Database", 
            TagNames = new[] { "nosql", "database", "modern", "storage" } 
        },
        new PostTemplate 
        { 
            Title = "Database Performance Tuning ve Optimization", 
            Content = GetDatabaseContent3(), 
            CategoryName = "Database", 
            TagNames = new[] { "database", "performance", "tuning", "optimization" } 
        },
        new PostTemplate 
        { 
            Title = "Database Security: Veritabanı Güvenliği", 
            Content = GetDatabaseContent4(), 
            CategoryName = "Database", 
            TagNames = new[] { "database", "security", "protection", "encryption" } 
        },
        new PostTemplate 
        { 
            Title = "Big Data: Büyük Veri Teknolojileri", 
            Content = GetDatabaseContent5(), 
            CategoryName = "Database", 
            TagNames = new[] { "big", "data", "analytics", "processing" } 
        },

        // Security Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Cybersecurity: Siber Güvenlik Temelleri", 
            Content = GetSecurityContent1(), 
            CategoryName = "Security", 
            TagNames = new[] { "cybersecurity", "security", "protection", "threats" } 
        },
        new PostTemplate 
        { 
            Title = "Penetration Testing: Güvenlik Testleri", 
            Content = GetSecurityContent2(), 
            CategoryName = "Security", 
            TagNames = new[] { "penetration", "testing", "security", "vulnerability" } 
        },
        new PostTemplate 
        { 
            Title = "Cryptography: Şifreleme Teknolojileri", 
            Content = GetSecurityContent3(), 
            CategoryName = "Security", 
            TagNames = new[] { "cryptography", "encryption", "security", "algorithms" } 
        },
        new PostTemplate 
        { 
            Title = "Network Security: Ağ Güvenliği", 
            Content = GetSecurityContent4(), 
            CategoryName = "Security", 
            TagNames = new[] { "network", "security", "firewall", "protection" } 
        },
        new PostTemplate 
        { 
            Title = "Application Security: Uygulama Güvenliği", 
            Content = GetSecurityContent5(), 
            CategoryName = "Security", 
            TagNames = new[] { "application", "security", "owasp", "vulnerabilities" } 
        },

        // Career Kategorisi - 5 yazı
        new PostTemplate 
        { 
            Title = "Tech Career Path: Kariyer Yol Haritası", 
            Content = GetCareerContent1(), 
            CategoryName = "Career", 
            TagNames = new[] { "career", "path", "tech", "development" } 
        },
        new PostTemplate 
        { 
            Title = "Technical Interview Preparation: Mülakat Hazırlığı", 
            Content = GetCareerContent2(), 
            CategoryName = "Career", 
            TagNames = new[] { "interview", "preparation", "technical", "career" } 
        },
        new PostTemplate 
        { 
            Title = "Soft Skills for Developers: Geliştirici Becerileri", 
            Content = GetCareerContent3(), 
            CategoryName = "Career", 
            TagNames = new[] { "soft", "skills", "developers", "communication" } 
        },
        new PostTemplate 
        { 
            Title = "Remote Work: Uzaktan Çalışma Rehberi", 
            Content = GetCareerContent4(), 
            CategoryName = "Career", 
            TagNames = new[] { "remote", "work", "career", "productivity" } 
        },
        new PostTemplate 
        { 
            Title = "Tech Leadership: Teknoloji Liderliği", 
            Content = GetCareerContent5(), 
            CategoryName = "Career", 
            TagNames = new[] { "leadership", "tech", "management", "career" } 
        }
    };

    public class PostTemplate
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string[] TagNames { get; set; } = Array.Empty<string>();
    }

    #region Content Generation Methods

    private static string GetTeknolojiContent1() => "2024'te teknoloji dünyası hızla değişmeye devam ediyor...";
    private static string GetTeknolojiContent2() => "Quantum computing, klasik bilgisayarların çözemediği problemleri...";
    private static string GetTeknolojiContent3() => "Metaverse, sanal ve artırılmış gerçeklik teknolojilerinin...";
    private static string GetTeknolojiContent4() => "Edge computing, veri işleme ve depolama işlemlerini...";
    private static string GetTeknolojiContent5() => "Green tech, sürdürülebilir teknoloji çözümleri...";

    private static string GetAIContent1() => "ChatGPT ve diğer generative AI araçları...";
    private static string GetAIContent2() => "Computer vision, bilgisayarların görüntüleri anlamasını...";
    private static string GetAIContent3() => "Natural Language Processing, insan dilini anlayan...";
    private static string GetAIContent4() => "AI ethics, yapay zeka teknolojilerinin etik kullanımı...";
    private static string GetAIContent5() => "Machine learning modellerinin production ortamına...";

    private static string GetBlockchainContent1() => "Bitcoin, ilk kripto para birimi olarak...";
    private static string GetBlockchainContent2() => "Smart contracts, blockchain üzerinde çalışan...";
    private static string GetBlockchainContent3() => "DeFi, merkeziyetsiz finans sistemleri...";
    private static string GetBlockchainContent4() => "NFT, benzersiz dijital varlıklar...";
    private static string GetBlockchainContent5() => "Web3, internetin geleceği olarak görülen...";

    private static string GetIoTContent1() => "IoT cihazları, günlük hayatımızın bir parçası...";
    private static string GetIoTContent2() => "Industrial IoT, endüstriyel süreçlerin...";
    private static string GetIoTContent3() => "IoT güvenlik, bağlantılı cihazların...";
    private static string GetIoTContent4() => "IoT veri analizi, büyük miktarda veri...";
    private static string GetIoTContent5() => "IoT protokol mimarisi, cihazlar arası...";

    private static string Get5GContent1() => "5G teknolojisi, mobil iletişimin yeni nesli...";
    private static string Get5GContent2() => "5G network slicing, sanal ağ segmentleri...";
    private static string Get5GContent3() => "5G ve IoT, bağlantılı cihazların gücü...";
    private static string Get5GContent4() => "5G güvenlik, yeni nesil ağların...";
    private static string Get5GContent5() => "5G radyo teknolojileri, spektrum yönetimi...";

    private static string GetSoftwareDevContent1() => "Clean code, okunabilir ve sürdürülebilir...";
    private static string GetSoftwareDevContent2() => "Design patterns, yazılım tasarımında...";
    private static string GetSoftwareDevContent3() => "SOLID prensipleri, object-oriented tasarımda...";
    private static string GetSoftwareDevContent4() => "TDD, test odaklı geliştirme yaklaşımı...";
    private static string GetSoftwareDevContent5() => "Refactoring, mevcut kodu iyileştirme...";

    private static string GetWebDevContent1() => "Modern web development, frontend ve backend...";
    private static string GetWebDevContent2() => "PWA, web uygulamalarının mobil deneyimi...";
    private static string GetWebDevContent3() => "Web performance, kullanıcı deneyimini...";
    private static string GetWebDevContent4() => "Web accessibility, herkes için erişilebilir...";
    private static string GetWebDevContent5() => "Web security, güvenli web uygulamaları...";

    private static string GetMobileDevContent1() => "Cross-platform development, tek kod tabanı...";
    private static string GetMobileDevContent2() => "Native vs hybrid, mobil uygulama geliştirmede...";
    private static string GetMobileDevContent3() => "Mobile performance, uygulama hızını...";
    private static string GetMobileDevContent4() => "Mobile security, kullanıcı verilerinin...";
    private static string GetMobileDevContent5() => "Mobile monetization, gelir elde etme...";

    private static string GetDevOpsContent1() => "CI/CD pipeline, sürekli entegrasyon ve...";
    private static string GetDevOpsContent2() => "Container orchestration, Kubernetes ve...";
    private static string GetDevOpsContent3() => "Infrastructure as Code, altyapıyı kod...";
    private static string GetDevOpsContent4() => "Monitoring ve observability, sistem izleme...";
    private static string GetDevOpsContent5() => "DevOps security, güvenli DevOps süreçleri...";

    private static string GetAITechContent1() => "Deep learning, neural networks ve yapay...";
    private static string GetAITechContent2() => "Machine learning algorithms, temel...";
    private static string GetAITechContent3() => "Natural Language Processing, dil işleme...";
    private static string GetAITechContent4() => "Computer vision, görüntü işleme ve...";
    private static string GetAITechContent5() => "AI model deployment, production ortamına...";

    private static string GetCloudContent1() => "Multi-cloud strategy, çoklu bulut yaklaşımı...";
    private static string GetCloudContent2() => "Serverless computing, fonksiyon tabanlı...";
    private static string GetCloudContent3() => "Cloud security, bulut güvenlik best...";
    private static string GetCloudContent4() => "Cloud cost optimization, maliyet optimizasyonu...";
    private static string GetCloudContent5() => "Edge computing, bulut bilişimin sınırları...";

    private static string GetDatabaseContent1() => "Database design, veritabanı tasarım...";
    private static string GetDatabaseContent2() => "NoSQL databases, modern veri depolama...";
    private static string GetDatabaseContent3() => "Database performance tuning, optimizasyon...";
    private static string GetDatabaseContent4() => "Database security, veritabanı güvenliği...";
    private static string GetDatabaseContent5() => "Big data, büyük veri teknolojileri...";

    private static string GetSecurityContent1() => "Cybersecurity, siber güvenlik temelleri...";
    private static string GetSecurityContent2() => "Penetration testing, güvenlik testleri...";
    private static string GetSecurityContent3() => "Cryptography, şifreleme teknolojileri...";
    private static string GetSecurityContent4() => "Network security, ağ güvenliği...";
    private static string GetSecurityContent5() => "Application security, uygulama güvenliği...";

    private static string GetCareerContent1() => "Tech career path, kariyer yol haritası...";
    private static string GetCareerContent2() => "Technical interview preparation, mülakat...";
    private static string GetCareerContent3() => "Soft skills for developers, geliştirici...";
    private static string GetCareerContent4() => "Remote work, uzaktan çalışma rehberi...";
    private static string GetCareerContent5() => "Tech leadership, teknoloji liderliği...";

    #endregion
} 