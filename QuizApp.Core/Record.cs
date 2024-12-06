namespace QuizApp.Core
{
    public class Record
    {
        public string Username { get; set; }

        public int BestScore { get; set; }

        // პარამეტრიანი კონსტრუქტორი, საიდანაც იქმნება ახალი რეკორდი
        public Record(string username, int bestScore)
        {
            Username = username;
            BestScore = bestScore;
        }

        public Record() { }
    }
}
