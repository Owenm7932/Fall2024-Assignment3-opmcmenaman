namespace Fall2024_Assignment3_opmcmenaman.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }               // Actor details
        public List<Movie> Movies { get; set; }         // Movies the actor has acted in
        public List<Movie> AllMovies { get; set; }
        public List<Tweet> AITweets { get; set; }      // AI-generated tweets about the actor
        public double OverallSentiment { get; set; }    // Overall sentiment score
    }
}
