using Blog.Domain.Entities;

namespace Blog.Infrastructure.Data.SeedData;

public static class TagSeedData
{
    public static readonly Tag[] Tags = new[]
    {
        // Programming Languages - 5 seed data
        new Tag { Name = "csharp", Slug = "csharp", Description = "Microsoft'un modern programlama dili", Color = "#178600", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "dotnet", Slug = "dotnet", Description = ".NET platformu ve ecosystem", Color = "#512BD4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "javascript", Slug = "javascript", Description = "Web'in programlama dili", Color = "#F7DF1E", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "typescript", Slug = "typescript", Description = "Typed JavaScript superset", Color = "#3178C6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "python", Slug = "python", Description = "Versatile programming language", Color = "#3776AB", PostCount = 0, FollowerCount = 0 },

        // Web Frameworks - 5 seed data
        new Tag { Name = "aspnetcore", Slug = "aspnetcore", Description = "Cross-platform web framework", Color = "#512BD4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "react", Slug = "react", Description = "Facebook'un UI library'si", Color = "#61DAFB", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "angular", Slug = "angular", Description = "Google'ın full-featured framework'ü", Color = "#DD0031", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "vue", Slug = "vue", Description = "Progressive JavaScript framework", Color = "#4FC08D", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "nodejs", Slug = "nodejs", Description = "Server-side JavaScript runtime", Color = "#339933", PostCount = 0, FollowerCount = 0 },

        // Backend Technologies - 5 seed data
        new Tag { Name = "java", Slug = "java", Description = "Enterprise programming language", Color = "#ED8B00", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "go", Slug = "go", Description = "Google'ın systems programming dili", Color = "#00ADD8", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "rust", Slug = "rust", Description = "Systems programming language", Color = "#000000", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "php", Slug = "php", Description = "Server-side scripting language", Color = "#777BB4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "ruby", Slug = "ruby", Description = "Dynamic programming language", Color = "#CC342D", PostCount = 0, FollowerCount = 0 },

        // Container & Orchestration - 5 seed data
        new Tag { Name = "docker", Slug = "docker", Description = "Containerization platform", Color = "#2496ED", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "kubernetes", Slug = "kubernetes", Description = "Container orchestration platform", Color = "#326CE5", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "rancher", Slug = "rancher", Description = "Kubernetes management platform", Color = "#0075A8", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "helm", Slug = "helm", Description = "Kubernetes package manager", Color = "#0DB9F0", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "openshift", Slug = "openshift", Description = "Enterprise Kubernetes platform", Color = "#EE0000", PostCount = 0, FollowerCount = 0 },

        // Cloud Platforms - 5 seed data
        new Tag { Name = "aws", Slug = "aws", Description = "Amazon Web Services cloud platform", Color = "#FF9900", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "azure", Slug = "azure", Description = "Microsoft cloud platform", Color = "#0089D6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "gcp", Slug = "gcp", Description = "Google Cloud Platform", Color = "#4285F4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "digitalocean", Slug = "digitalocean", Description = "Cloud infrastructure provider", Color = "#0080FF", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "heroku", Slug = "heroku", Description = "Cloud application platform", Color = "#430098", PostCount = 0, FollowerCount = 0 },

        // Architecture Patterns - 5 seed data
        new Tag { Name = "microservices", Slug = "microservices", Description = "Distributed system architecture", Color = "#FF6B6B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "monolith", Slug = "monolith", Description = "Monolithic application architecture", Color = "#4ECDC4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "serverless", Slug = "serverless", Description = "Serverless computing architecture", Color = "#10B981", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "event-driven", Slug = "event-driven", Description = "Event-driven architecture patterns", Color = "#8B5CF6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "ddd", Slug = "ddd", Description = "Domain-Driven Design", Color = "#F59E0B", PostCount = 0, FollowerCount = 0 },

        // API Technologies - 5 seed data
        new Tag { Name = "api", Slug = "api", Description = "Application Programming Interface", Color = "#4ECDC4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "rest", Slug = "rest", Description = "Representational State Transfer", Color = "#45B7D1", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "graphql", Slug = "graphql", Description = "Query language for APIs", Color = "#E10098", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "grpc", Slug = "grpc", Description = "High-performance RPC framework", Color = "#00ADD8", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "soap", Slug = "soap", Description = "Simple Object Access Protocol", Color = "#FF6600", PostCount = 0, FollowerCount = 0 },

        // Database Technologies - 5 seed data
        new Tag { Name = "database", Slug = "database", Description = "Data storage and management", Color = "#336791", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "sql", Slug = "sql", Description = "Structured Query Language", Color = "#E48E00", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "nosql", Slug = "nosql", Description = "Non-relational databases", Color = "#4DB33D", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "mongodb", Slug = "mongodb", Description = "Document-oriented database", Color = "#47A248", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "postgresql", Slug = "postgresql", Description = "Advanced open source database", Color = "#336791", PostCount = 0, FollowerCount = 0 },

        // Caching & Search - 5 seed data
        new Tag { Name = "redis", Slug = "redis", Description = "In-memory data structure store", Color = "#DC382D", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "elasticsearch", Slug = "elasticsearch", Description = "Search and analytics engine", Color = "#FED10A", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "memcached", Slug = "memcached", Description = "Distributed memory caching system", Color = "#FF6600", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "solr", Slug = "solr", Description = "Enterprise search platform", Color = "#D9411E", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "cassandra", Slug = "cassandra", Description = "Distributed NoSQL database", Color = "#1287B1", PostCount = 0, FollowerCount = 0 },

        // Message Brokers - 5 seed data
        new Tag { Name = "rabbitmq", Slug = "rabbitmq", Description = "Message broker software", Color = "#FF6600", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "kafka", Slug = "kafka", Description = "Distributed streaming platform", Color = "#231F20", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "activemq", Slug = "activemq", Description = "Apache ActiveMQ message broker", Color = "#D93A35", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "nats", Slug = "nats", Description = "Cloud native messaging system", Color = "#20BF6B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "pulsar", Slug = "pulsar", Description = "Apache Pulsar messaging platform", Color = "#188FFF", PostCount = 0, FollowerCount = 0 },

        // Real-time Communication - 5 seed data
        new Tag { Name = "signalr", Slug = "signalr", Description = "Real-time communication library", Color = "#512BD4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "websockets", Slug = "websockets", Description = "WebSocket protocol implementation", Color = "#4ECDC4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "socketio", Slug = "socketio", Description = "Real-time bidirectional communication", Color = "#010101", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "pusher", Slug = "pusher", Description = "Real-time messaging service", Color = "#300D4F", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "firebase", Slug = "firebase", Description = "Google's mobile platform", Color = "#FFCA28", PostCount = 0, FollowerCount = 0 },

        // AI & Machine Learning - 5 seed data
        new Tag { Name = "machinelearning", Slug = "machinelearning", Description = "AI subset for pattern recognition", Color = "#FF6B6B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "ai", Slug = "ai", Description = "Artificial Intelligence", Color = "#00D4FF", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "tensorflow", Slug = "tensorflow", Description = "Open source ML framework", Color = "#FF6F00", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "pytorch", Slug = "pytorch", Description = "Deep learning framework", Color = "#EE4C2C", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "scikit-learn", Slug = "scikit-learn", Description = "Machine learning library", Color = "#F7931E", PostCount = 0, FollowerCount = 0 },

        // Blockchain & Crypto - 5 seed data
        new Tag { Name = "blockchain", Slug = "blockchain", Description = "Distributed ledger technology", Color = "#F7931A", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "ethereum", Slug = "ethereum", Description = "Blockchain platform", Color = "#627EEA", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "bitcoin", Slug = "bitcoin", Description = "Cryptocurrency and blockchain", Color = "#F7931A", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "solidity", Slug = "solidity", Description = "Smart contract language", Color = "#363636", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "web3", Slug = "web3", Description = "Decentralized web technologies", Color = "#F16822", PostCount = 0, FollowerCount = 0 },

        // Frontend Technologies - 5 seed data
        new Tag { Name = "frontend", Slug = "frontend", Description = "Client-side development", Color = "#61DAFB", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "html", Slug = "html", Description = "HyperText Markup Language", Color = "#E34F26", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "css", Slug = "css", Description = "Cascading Style Sheets", Color = "#1572B6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "sass", Slug = "sass", Description = "CSS preprocessor", Color = "#CC6699", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "tailwindcss", Slug = "tailwindcss", Description = "Utility-first CSS framework", Color = "#06B6D4", PostCount = 0, FollowerCount = 0 },

        // Backend Technologies - 5 seed data
        new Tag { Name = "backend", Slug = "backend", Description = "Server-side development", Color = "#FF6B6B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "express", Slug = "express", Description = "Node.js web framework", Color = "#000000", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "django", Slug = "django", Description = "Python web framework", Color = "#092E20", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "spring", Slug = "spring", Description = "Java application framework", Color = "#6DB33F", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "laravel", Slug = "laravel", Description = "PHP web framework", Color = "#FF2D20", PostCount = 0, FollowerCount = 0 },

        // Full Stack Development - 5 seed data
        new Tag { Name = "fullstack", Slug = "fullstack", Description = "Full-stack development", Color = "#4ECDC4", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "mern", Slug = "mern", Description = "MongoDB, Express, React, Node.js", Color = "#61DAFB", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "mean", Slug = "mean", Description = "MongoDB, Express, Angular, Node.js", Color = "#DD0031", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "jamstack", Slug = "jamstack", Description = "JavaScript, APIs, and Markup", Color = "#F0047F", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "serverless", Slug = "serverless", Description = "Serverless architecture", Color = "#10B981", PostCount = 0, FollowerCount = 0 },

        // Mobile Development - 5 seed data
        new Tag { Name = "mobile", Slug = "mobile", Description = "Mobile application development", Color = "#8B5CF6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "ios", Slug = "ios", Description = "Apple mobile platform", Color = "#000000", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "android", Slug = "android", Description = "Google mobile platform", Color = "#3DDC84", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "flutter", Slug = "flutter", Description = "Google's UI toolkit", Color = "#02569B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "reactnative", Slug = "reactnative", Description = "React for mobile development", Color = "#61DAFB", PostCount = 0, FollowerCount = 0 },

        // Security & Testing - 5 seed data
        new Tag { Name = "security", Slug = "security", Description = "Cybersecurity and protection", Color = "#DC2626", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "testing", Slug = "testing", Description = "Software testing methodologies", Color = "#10B981", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "junit", Slug = "junit", Description = "Java unit testing framework", Color = "#25A162", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "jest", Slug = "jest", Description = "JavaScript testing framework", Color = "#C21325", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "selenium", Slug = "selenium", Description = "Web browser automation", Color = "#43B02A", PostCount = 0, FollowerCount = 0 },

        // DevOps & CI/CD - 5 seed data
        new Tag { Name = "ci-cd", Slug = "ci-cd", Description = "Continuous Integration/Deployment", Color = "#FF6B6B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "jenkins", Slug = "jenkins", Description = "Automation server", Color = "#D33833", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "gitlab", Slug = "gitlab", Description = "DevOps platform", Color = "#FC6D26", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "github", Slug = "github", Description = "Code hosting platform", Color = "#181717", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "terraform", Slug = "terraform", Description = "Infrastructure as code", Color = "#7C3AED", PostCount = 0, FollowerCount = 0 },

        // Agile & Project Management - 5 seed data
        new Tag { Name = "agile", Slug = "agile", Description = "Agile development methodology", Color = "#F59E0B", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "scrum", Slug = "scrum", Description = "Agile framework for teams", Color = "#8B5CF6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "kanban", Slug = "kanban", Description = "Visual project management", Color = "#00ADD8", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "jira", Slug = "jira", Description = "Project management tool", Color = "#0052CC", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "confluence", Slug = "confluence", Description = "Team collaboration platform", Color = "#172B4D", PostCount = 0, FollowerCount = 0 },

        // Career & Professional Development - 5 seed data
        new Tag { Name = "career", Slug = "career", Description = "Professional development", Color = "#059669", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "interview", Slug = "interview", Description = "Job interview preparation", Color = "#DC2626", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "resume", Slug = "resume", Description = "CV writing and optimization", Color = "#3B82F6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "networking", Slug = "networking", Description = "Professional networking", Color = "#8B5CF6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "mentorship", Slug = "mentorship", Description = "Career mentorship and guidance", Color = "#F59E0B", PostCount = 0, FollowerCount = 0 },

        // Learning & Tutorials - 5 seed data
        new Tag { Name = "tutorial", Slug = "tutorial", Description = "Step-by-step learning guides", Color = "#3B82F6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "beginners", Slug = "beginners", Description = "Content for new developers", Color = "#10B981", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "advanced", Slug = "advanced", Description = "Advanced programming concepts", Color = "#DC2626", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "best-practices", Slug = "best-practices", Description = "Programming best practices", Color = "#8B5CF6", PostCount = 0, FollowerCount = 0 },
        new Tag { Name = "code-review", Slug = "code-review", Description = "Code review techniques", Color = "#F59E0B", PostCount = 0, FollowerCount = 0 }
    };
} 