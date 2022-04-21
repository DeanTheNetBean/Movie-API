namespace Movie_API.Models
{
    public class Stat
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public double AverageWatchDurationS { get; set; }
        public int Watches { get; set; }       
        public int ReleaseYear { get; set; }
    }
}
