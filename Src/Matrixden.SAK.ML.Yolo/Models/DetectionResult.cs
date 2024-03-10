using OpenCvSharp;

namespace Matrixden.SAK.ML.Yolo.Models
{
    public class DetectionResult
    {
        public RotatedRect Rect { get; set; }
        public double Score { get; set; }
        public string Label { get; set; }

        public DetectionResult() { }
        public DetectionResult(RotatedRect rect, double score, string label) : this()
        {
            Rect = rect;
            Score = score;
            Label = label;
        }

        public override string ToString()
        {
            if (this == default || Rect == default)
                return string.Empty;
            else
                return $"Label: {Label}; Score: {Score}; Rect: {string.Join(", ", Rect.Points())}";
        }
    }
}
