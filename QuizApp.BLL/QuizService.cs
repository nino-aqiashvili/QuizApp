public class QuizService
{
    private readonly IRepository<Quiz> _quizRepository;
    private readonly IRepository<User> _userRepository;

    public QuizService(IRepository<Quiz> quizRepository, IRepository<User> userRepository)
    {
        _quizRepository = quizRepository;
        _userRepository = userRepository;
    }

    public void CreateQuiz(Quiz quiz)
    {
        _quizRepository.Add(quiz);
    }

    public void EditQuiz(string quizId, string username, List<Question> updatedQuestions)
    {
        var quiz = _quizRepository.GetById(q => q.Id == quizId);

        if (quiz == null || quiz.CreatorUsername != username)
        {
            throw new UnauthorizedAccessException("You can only edit your own quizzes.");
        }

        quiz.Questions = updatedQuestions;
        _quizRepository.Update(q => q.Id == quizId, quiz);
    }

    public List<Quiz> GetAllQuizzesExceptUser(string username)
    {
        return _quizRepository.GetAll().Where(q => q.CreatorUsername != username).ToList();
    }

    public List<Quiz> GetQuizzesForUser(string username)
    {
        return _quizRepository.GetAll().Where(q => q.CreatorUsername == username).ToList();
    }
}
