namespace MyApi.Models
{
    public class MangaById
    {
        public Data data { get; set; }   
    }
    public class Data
    {
        public int id { get; set; }
        public string type { get; set; }
        public Attributes attributes { get; set; }
    }
    public class Attributes
    {
        public string slug { get; set; }
        public string description { get; set; }
        public string canonicalTitle { get; set; }
        public double? averageRating { get; set; }
        public int? userCount { get; set; }
        public string startDate { get; set; }
        public string? endDate { get; set; }
        public string subtype { get; set; }
        public string status { get; set; }
        public PosterImage posterImage { get; set; }
        public int? chapterCount { get; set; }
        public int? volumeCount { get; set; }
    }
    public class PosterImage
    {
        public string original { get; set; }
    }
}
