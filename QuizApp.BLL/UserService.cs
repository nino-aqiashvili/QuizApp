public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public bool Register(User user)
    {
        if (_userRepository.GetAll().Any(u => u.Username == user.Username))
        {
            return false;
        }

        _userRepository.Add(user);
        return true;
    }

    public User Login(string username, string password)
    {
        return _userRepository.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public void UpdateUserScore(User user, int newScore)
    {
        if (newScore > user.Score)
        {
            user.Score = newScore;
            _userRepository.Update(u => u.Id == user.Id, user);
        }
    }

    public List<User> GetTop10Users()
    {
        return _userRepository.GetAll().OrderByDescending(u => u.Score).Take(10).ToList();
    }
}
