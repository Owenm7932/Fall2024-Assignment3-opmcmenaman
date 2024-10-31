namespace Fall2024_Assignment3_opmcmenaman.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }         
        public List<Movie> Movies { get; set; }
        public List<Movie> AllMovies { get; set; }
        public List<Tweet> AITweets { get; set; }    
        public double OverallSentiment { get; set; } 
    }
}
