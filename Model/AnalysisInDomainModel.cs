
using Android.App;
using System.Collections.Generic;

namespace XamarinRecogCeleb.Model
{
    public class AnalysisInDomainModel
    {
        public Metadata metadata { get; set; }
        public string requestId { get; set; }
        public Result result { get; set; }
    }
    public class Metadata
    {
        public string format { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
    public class Result
    {
        public List<Celebrity> celebrities { get; set; }
    }

    public class Celebrity
    {
        public string name { get; set; }
        public double confidence { get; set; }
        public FaceRectangle faceRectangle { get; set; }
    }

    public class FaceRectangle
    {
        public int left { get; set; }
        public int width { get; set; }
        public int top { get; set; }
        public int height { get; set; }
    }
}