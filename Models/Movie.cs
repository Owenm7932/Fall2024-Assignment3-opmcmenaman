namespace Fall2024_Assignment3_opmcmenaman.Models
{
    public class Movie
    {
        public string MovieId { get; set; }
        public string Title { get; set; }
        public string IMDBLink { get; set; }
        public string Genre { get; set; }
        public string YearOfRelease { get; set; }
        public string PosterURL { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

}
