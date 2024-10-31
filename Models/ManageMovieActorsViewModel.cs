namespace Fall2024_Assignment3_opmcmenaman.Models
{
    public class ManageMovieActorsViewModel
    {
        public Movie Movie { get; set; }
        public List<Actor> AssignedActors { get; set; }
        public List<Actor> AvailableActors { get; set; }
    }

}
