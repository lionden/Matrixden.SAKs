using OpenCvSharp;

namespace Matrixden.SAK.ML.Yolo.Models
{
    public class DetectionResult
    {
        public int Index { get; set; }
        public string Label { get; set; }
        public double Score { get; set; }
        public RotatedRect Rect { get; set; }

        public DetectionResult() { }
        public DetectionResult(int index, double score, RotatedRect rect, string label = "") : this()
        {
            Index = index;
            Score = score;
            Rect = rect;
            Label = label;
        }

        public override string ToString()
        {
            if (this == default || Rect == default)
                return string.Empty;
            else
                return $"Index: {Index}; Label: {Label}; Score: {Score}; Rect: {string.Join(", ", Rect.Points())}";
        }
    }
}
