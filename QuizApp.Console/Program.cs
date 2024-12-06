using System;
using System.Collections.Generic;

public class Program
{
    private static User loggedInUser = null;
    private static QuizService quizService = null;
    private static UserService userService = null;

    private static IRepository<User> userRepository = new JsonRepository<User>("users.json");
    private static IRepository<Quiz> quizRepository = new JsonRepository<Quiz>("quizzes.json");

    static void Main(string[] args)
    {
        quizService = new QuizService(quizRepository, userRepository);
        userService = new UserService(userRepository);

        Console.WriteLine("Welcome to the Quiz App!");
        MainMenu();
    }

    static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");
        Console.Write("Choose an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Register();
                break;
            case "2":
                Login();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                MainMenu();
                break;
        }
    }

    static void Register()
    {
        Console.Clear();
        Console.WriteLine("Register");

        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

        User user = new() { Username = username, Password = password, Score = 0 };
        if (userService.Register(user))
        {
            Console.WriteLine("Registration successful!");
            Console.ReadKey();
            MainMenu();
        }
        else
        {
            Console.WriteLine("Username already exists!");
            Console.ReadKey();
            MainMenu();
        }
    }

    static void Login()
    {
        Console.Clear();
        Console.WriteLine("Login");

        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

        loggedInUser = userService.Login(username, password);

        if (loggedInUser != null)
        {
            Console.WriteLine("Login successful!");
            UserMenu();
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
            Console.ReadKey();
            MainMenu();
        }
    }

    static void UserMenu()
    {
        Console.Clear();
        Console.WriteLine($"Welcome, {loggedInUser.Username}!");
        Console.WriteLine("1. Create Quiz");
        Console.WriteLine("2. Solve Quiz");
        Console.WriteLine("3. View Top 10 Users");
        Console.WriteLine("4. Edit Quiz");
        Console.WriteLine("5. Logout");
        Console.Write("Choose an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                CreateQuiz();
                break;
            case "2":
                SolveQuiz();
                break;
            case "3":
                ViewTop10Users();
                break;
            case "4":
                EditQuiz();
                break;
            case "5":
                loggedInUser = null;
                MainMenu();
                break;
            default:
                UserMenu();
                break;
        }
    }

    static void CreateQuiz()
    {
        Console.Clear();
        Console.WriteLine("Create a new Quiz");

        Console.Write("Quiz Title: ");
        var title = Console.ReadLine();

        var questions = new List<Question>();
        Console.WriteLine("Enter questions for the quiz (type 'done' to finish):");

        while (true)
        {
            if (questions.Count >= 5)
            {
                Console.WriteLine("You can only add up to 5 questions. Quiz creation is complete.");
                break;
            }

            Console.Write("Enter question text: ");
            var questionText = Console.ReadLine();
            if (questionText.ToLower() == "done") break;

            var options = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                Console.Write($"Enter option {i + 1}: ");
                options.Add(Console.ReadLine());
            }

            Console.Write("Enter the number of the correct option (1-4): ");
            var correctOption = int.Parse(Console.ReadLine()) - 1;

            Question question = new()
            {
                Text = questionText,
                Options = options,
                CorrectOptionIndex = correctOption
            };

            questions.Add(question);
        }

        Quiz quiz = new()
        {
            Title = title,
            CreatorUsername = loggedInUser.Username,
            Questions = questions
        };

        quizService.CreateQuiz(quiz);
        Console.WriteLine("Quiz created successfully!");
        Console.ReadKey();
        UserMenu();
    }

    static void SolveQuiz()
    {
        Console.Clear();
        var allQuizzes = quizService.GetAllQuizzesExceptUser(loggedInUser.Username);

        if (allQuizzes.Count == 0)
        {
            Console.WriteLine("No quizzes available to solve.");
            Console.ReadKey();
            UserMenu();
            return;
        }

        Console.WriteLine("Available Quizzes:");
        Console.WriteLine("Attention! You Will Have Two Minutes to Complete The Chosen Quiz!");

        for (int i = 0; i < allQuizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {allQuizzes[i].Title} (Created by {allQuizzes[i].CreatorUsername})");
        }

        Console.Write("Choose a quiz to solve (number): ");
        var quizChoice = int.Parse(Console.ReadLine()) - 1;

        if (quizChoice < 0 || quizChoice >= allQuizzes.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            UserMenu();
            return;
        }

        var quiz = allQuizzes[quizChoice];
        int score = 0;
        var startTime = DateTime.Now;

        foreach (var question in quiz.Questions)
        {
            if ((DateTime.Now - startTime).TotalMinutes > 2)
            {
                Console.WriteLine("Time's up! You failed the quiz.");
                Console.ReadKey();
                UserMenu();
                return;
            }

            Console.Clear();
            Console.WriteLine(question.Text);
            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {question.Options[i]}");
            }

            Console.Write("Your answer (1-4): ");
            var answer = Console.ReadLine();

            if (int.TryParse(answer, out int answerIndex) && answerIndex - 1 == question.CorrectOptionIndex)
            {
                score += 20;
            }
            else
            {
                score -= 20;
            }
        }

        Console.WriteLine($"Quiz completed! Your score: {score}");
        userService.UpdateUserScore(loggedInUser, score);
        Console.ReadKey();
        UserMenu();
    }

    static void ViewTop10Users()
    {
        Console.Clear();
        var topUsers = userService.GetTop10Users();

        Console.WriteLine("Top 10 Users:");
        foreach (var user in topUsers)
        {
            Console.WriteLine($"{user.Username}: {user.Score} points");
        }
        Console.ReadKey();
        UserMenu();
    }

    static void EditQuiz()
    {
        Console.Clear();
        var quizzes = quizService.GetQuizzesForUser(loggedInUser.Username);

        if (quizzes.Count == 0)
        {
            Console.WriteLine("You have no quizzes to edit.");
            Console.ReadKey();
            UserMenu();
            return;
        }

        Console.WriteLine("Your quizzes:");
        for (int i = 0; i < quizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {quizzes[i].Title}");
        }

        Console.Write("Choose a quiz to edit (number): ");
        var quizChoice = int.Parse(Console.ReadLine()) - 1;

        if (quizChoice < 0 || quizChoice >= quizzes.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            UserMenu();
            return;
        }

        var quiz = quizzes[quizChoice];
        var questions = quiz.Questions;

        if (questions.Count >= 5)
        {
            Console.WriteLine("You cannot add more questions. There are already 5 questions in this quiz.");
        }

        for (int i = 0; i < questions.Count; i++)
        {
            Console.Clear();
            Console.WriteLine($"Editing Question {i + 1}: {questions[i].Text}");

            Console.Write("Enter new question text (or press Enter to keep the same): ");
            var newText = Console.ReadLine();
            if (!string.IsNullOrEmpty(newText))
            {
                questions[i].Text = newText;
            }

            for (int j = 0; j < 4; j++)
            {
                Console.Write($"Enter new option {j + 1} (or press Enter to keep the same): ");
                var newOption = Console.ReadLine();
                if (!string.IsNullOrEmpty(newOption))
                {
                    questions[i].Options[j] = newOption;
                }
            }

            Console.Write("Enter correct option (1-4) (or press Enter to keep the same): ");
            var correctOptionInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(correctOptionInput))
            {
                var correctOptionIndex = int.Parse(correctOptionInput) - 1;
                questions[i].CorrectOptionIndex = correctOptionIndex;
            }
        }

        quizService.EditQuiz(quiz.Id, loggedInUser.Username, questions);

        Console.WriteLine("Quiz updated successfully!");
        Console.ReadKey();
        UserMenu();
    }
}

