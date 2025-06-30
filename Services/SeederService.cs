using devlife_backend.Data;
using devlife_backend.Models.Casino;
using devlife_backend.Models.Games;
using MongoDB.Driver;

namespace devlife_backend.Services
{
    public class SeederService
    {
        private readonly DevLifeDbContext _context;
        private readonly IMongoDatabase _mongoDb;

        public SeederService(DevLifeDbContext context, IMongoDatabase mongoDb)
        {
            _context = context;
            _mongoDb = mongoDb;
        }

        public async Task SeedAllDataAsync()
        {
            try
            {
                await SeedMongoDataAsync();
                Console.WriteLine("✅ All game data seeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Seeding failed: {ex.Message}");
                // Don't throw - let the app continue
            }
        }

        private async Task SeedMongoDataAsync()
        {
            try
            {
                // Seed horoscopes
                var horoscopes = _mongoDb.GetCollection<MongoHoroscope>("horoscopes");
                if (await horoscopes.CountDocumentsAsync(_ => true) == 0)
                {
                    await horoscopes.InsertManyAsync(new[]
                    {
                        new MongoHoroscope { ZodiacSign = "Gemini", Message = "დღეს კარგი დღეა კოდისთვის!", LuckyTech = "TypeScript", CodingTip = "იფიქრე კოდის შესახებ" },
                        new MongoHoroscope { ZodiacSign = "Leo", Message = "შენი ლიდერობა კოდ რევიუში გამოჩნდება!", LuckyTech = "Angular", CodingTip = "ეჩვენე შენი ელეგანტური გადაწყვეტები" },
                        new MongoHoroscope { ZodiacSign = "Scorpio", Message = "ღრმად ჩაკონცენტრირდი კომპლექსურ ალგორითმებზე!", LuckyTech = "Python", CodingTip = "ენდე შენს გამომძიებლურ ინსტინქტებს" },
                        new MongoHoroscope { ZodiacSign = "Aries", Message = "Today your code will compile on the first try! Your aggressive debugging style pays off 🔥", LuckyTech = "Angular", CodingTip = "Trust your instincts when refactoring" },
                        new MongoHoroscope { ZodiacSign = "Taurus", Message = "Slow and steady debugging wins the race today. Your methodical approach catches all bugs 🐢", LuckyTech = ".NET", CodingTip = "Build solid foundations that last" },
                        new MongoHoroscope { ZodiacSign = "Cancer", Message = "Perfect day to mentor a junior developer. Your nurturing nature shines 🤗", LuckyTech = "Angular", CodingTip = "Focus on user experience and empathy" },
                        new MongoHoroscope { ZodiacSign = "Virgo", Message = "Your attention to detail catches bugs others miss. Legendary debugging session ahead 🔍", LuckyTech = ".NET", CodingTip = "Clean, documented code is worth the time" },
                        new MongoHoroscope { ZodiacSign = "Libra", Message = "Perfect balance between new features and bug fixes today ⚖️", LuckyTech = "React", CodingTip = "Harmony in code architecture" },
                        new MongoHoroscope { ZodiacSign = "Sagittarius", Message = "Explore new technologies today. Your adventurous spirit leads to breakthroughs 🏹", LuckyTech = "Vue.js", CodingTip = "Don't fear trying new frameworks" },
                        new MongoHoroscope { ZodiacSign = "Capricorn", Message = "Disciplined coding approach yields great results. Structure your day well 🏔️", LuckyTech = "Java", CodingTip = "Plan before you code" },
                        new MongoHoroscope { ZodiacSign = "Aquarius", Message = "Revolutionary ideas flow today. Your unique approach solves old problems 💡", LuckyTech = "Node.js", CodingTip = "Think outside conventional patterns" },
                        new MongoHoroscope { ZodiacSign = "Pisces", Message = "Intuitive coding session ahead. Let creativity guide your solutions 🐠", LuckyTech = "Flutter", CodingTip = "Trust your creative instincts" }
                    });
                    Console.WriteLine("✅ Horoscopes seeded");
                }

                // Seed code snippets
                var snippets = _mongoDb.GetCollection<MongoCodeSnippet>("code_snippets");
                if (await snippets.CountDocumentsAsync(_ => true) == 0)
                {
                    await snippets.InsertManyAsync(new[]
                    {
                        new MongoCodeSnippet
                        {
                            Language = "javascript",
                            Difficulty = 1,
                            CorrectCode = "function sum(a, b) { return a + b; }",
                            BuggyCode = "function sum(a, b { return a + b; }",
                            Explanation = "Missing closing parenthesis",
                            TechStacks = new List<string> { "JavaScript", "Angular" }
                        },
                        new MongoCodeSnippet
                        {
                            Language = "csharp",
                            Difficulty = 1,
                            CorrectCode = "string message = \"Hello World\"; Console.WriteLine(message);",
                            BuggyCode = "string message = \"Hello World\" Console.WriteLine(message);",
                            Explanation = "Missing semicolon after string declaration",
                            TechStacks = new List<string> { ".NET", "C#" }
                        }
                    });
                    Console.WriteLine("✅ Code snippets seeded");
                }

                // Seed code challenges
                var challenges = _mongoDb.GetCollection<CodeChallenge>("code_challenges");
                if (await challenges.CountDocumentsAsync(_ => true) == 0)
                {
                    await challenges.InsertManyAsync(new[]
                    {
                        new CodeChallenge
                        {
                            Title = "FizzBuzz Classic",
                            Description = "Write a program that prints numbers 1-100, but 'Fizz' for multiples of 3, 'Buzz' for multiples of 5",
                            Difficulty = 1,
                            Category = "algorithms",
                            Languages = new List<string> { "javascript", "python", "csharp" }
                        },
                        new CodeChallenge
                        {
                            Title = "Palindrome Checker",
                            Description = "Create a function that checks if a string reads the same forwards and backwards",
                            Difficulty = 2,
                            Category = "string-manipulation",
                            Languages = new List<string> { "javascript", "python", "csharp" }
                        }
                    });
                    Console.WriteLine("✅ Code challenges seeded");
                }

                // Seed dating profiles
                var profiles = _mongoDb.GetCollection<DatingProfile>("dating_profiles");
                if (await profiles.CountDocumentsAsync(_ => true) == 0)
                {
                    await profiles.InsertManyAsync(new[]
                    {
                        new DatingProfile
                        {
                            Name = "Alex Chen",
                            Bio = "Full-stack developer who loves clean code and coffee ☕",
                            TechStack = new List<string> { "Angular", "Node.js", "TypeScript", "MongoDB" },
                            ExperienceLevel = "Middle",
                            ZodiacSign = "Gemini",
                            Avatar = "/avatars/alex.jpg"
                        },
                        new DatingProfile
                        {
                            Name = "Sarah Johnson",
                            Bio = "Backend wizard specializing in .NET architectures 🚀",
                            TechStack = new List<string> { ".NET", "Azure", "SQL Server", "Docker" },
                            ExperienceLevel = "Senior",
                            ZodiacSign = "Virgo",
                            Avatar = "/avatars/sarah.jpg"
                        }
                    });
                    Console.WriteLine("✅ Dating profiles seeded");
                }

                // Seed meeting excuses
                var excuses = _mongoDb.GetCollection<MeetingExcuse>("meeting_excuses");
                if (await excuses.CountDocumentsAsync(_ => true) == 0)
                {
                    await excuses.InsertManyAsync(new[]
                    {
                        new MeetingExcuse
                        {
                            Category = "technical",
                            Excuse = "სერვერს ცეცხლი გაუჩნდა და მე უნდა გავაქრო ალი!",
                            Believability = 8,
                            Tags = new List<string> { "urgent", "production", "fire" }
                        },
                        new MeetingExcuse
                        {
                            Category = "personal",
                            Excuse = "ჩემს კატამ production-ში deploy გააკეთა კლავიატურაზე გავლით",
                            Believability = 6,
                            Tags = new List<string> { "pets", "deployment", "accident" }
                        }
                    });
                    Console.WriteLine("✅ Meeting excuses seeded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ MongoDB seeding failed: {ex.Message}");
            }
        }
    }
}