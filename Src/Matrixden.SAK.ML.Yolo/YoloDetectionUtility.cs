using Matrixden.SAK.ML.Yolo.Models;
using Matrixden.SwissArmyKnives.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using Math = System.Math;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Matrixden.SAK.ML.Yolo
{
    public class YoloDetectionUtility
    {
        string _yoloPath;
        public double ScoreThreshold { get; set; }
        public double NmsThreshold { get; set; }
        public string[] Labels { get; set; }

        private YoloDetectionUtility()
        {
            Labels = [];
        }

        public YoloDetectionUtility(string yoloPath, ResizeModes resizeMode) : this()
        {
            YoloPath = yoloPath;
            ResizeMode = resizeMode;
        }

        public string YoloPath
        {
            get { return _yoloPath; }
            set { _yoloPath = value; }
        }

        public ResizeModes ResizeMode { get; set; }

        int YoloImageWidth, YoloImageHeight;
        double _factor = 1.0;
        /// <summary>
        /// ImageDetectionWithYolo
        /// </summary>
        /// <param name="image"></param>
        /// <returns>List`DetectionResult</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public OperationResult ImageDetectionWithYolo(Mat image)
        {
            if (!File.Exists(_yoloPath))
            {
                throw new FileNotFoundException("File not exist", _yoloPath);
            }

            SessionOptions options = new();
            options.AppendExecutionProvider_CPU(0);
            InferenceSession onnxSession = new(_yoloPath, options);
            var inputName = onnxSession.InputMetadata.Keys.First();
            YoloImageWidth = onnxSession.InputMetadata[inputName].Dimensions[3];
            YoloImageHeight = onnxSession.InputMetadata[inputName].Dimensions[2];

            // 
            using Mat resize_image = ResizeImage(image, YoloImageWidth, ResizeMode);
            // 构造输入数据
            var inputTensor = ImageToTensor(resize_image, onnxSession.InputMetadata[inputName].Dimensions);

            // 配置输入
            List<NamedOnnxValue> inputs =
                [
                    NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
                ];

            // 执行推理
            var results = onnxSession.Run(inputs);

            // 获取输出结果
            var outputName = onnxSession.OutputMetadata.Keys.First();
            var outputTensor = results[0].AsTensor<float>();
            var result_array = outputTensor.ToArray();

            // 处理输出结果
            return ProcessOutput(result_array, onnxSession.OutputMetadata[outputName].Dimensions);
        }

        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target_image_length"></param>
        /// <param name="resizeMode"></param>
        /// <returns></returns>
        private Mat ResizeImage(Mat image, int target_image_length, ResizeModes resizeMode = ResizeModes.PadDirectly)
        {
            int max_image_length = System.Math.Max(image.Width, image.Height);
            using Mat originalImage = image.Clone();
            Mat resized_image = new();
            if (resizeMode == ResizeModes.ZoomThenPad || System.Math.Max(originalImage.Width, originalImage.Height) > 1024)
            {
                _factor = max_image_length * 1.0 / target_image_length;
                using Mat max_image = new(max_image_length, max_image_length, originalImage.Type());
                originalImage.CopyTo(new(max_image, new Rect(0, 0, image.Width, image.Height)));

                // 将图片转为RGB通道
                using Mat image_rgb = new();
                Cv2.CvtColor(max_image, image_rgb, ColorConversionCodes.BGR2RGB);
                Cv2.Resize(image_rgb, resized_image, new Size(target_image_length, target_image_length));
#if DEBUG
                Console.WriteLine($"Temp images saved @'{Path.GetTempPath()}IRA'.");
                originalImage.SaveImage($"{Path.GetTempPath()}IRA\\{nameof(ResizeImage)}1.originalImage.png");
                max_image.SaveImage($"{Path.GetTempPath()}IRA\\{nameof(ResizeImage)}2.max_image.png");
                resized_image.SaveImage($"{Path.GetTempPath()}IRA\\{nameof(ResizeImage)}3.resized_image.png");
#endif
            }
            else
            {
                Cv2.CopyMakeBorder(originalImage, resized_image, 0, target_image_length - image.Height, 0, target_image_length - image.Width, BorderTypes.Constant, Scalar.Black);
#if DEBUG
                resized_image.SaveImage($"{Path.GetTempPath()}IRA\\{nameof(ResizeImage)}3.resized_image.png");
#endif
            }

            return resized_image;
        }

        private static DenseTensor<float> ImageToTensor(Mat resize_image, int[] inputDimensions)
        {
            DenseTensor<float> tensor = new(inputDimensions);
            for (int c = 0; c < inputDimensions[1]; c++)
            {
                for (int y = 0; y < resize_image.Height; y++)
                {
                    for (int x = 0; x < resize_image.Width; x++)
                    {
                        tensor[0, c, y, x] = resize_image.At<Vec3b>(y, x)[0] / 255f;
                    }
                }
            }

            return tensor;
        }

        private OperationResult ProcessOutput(float[] result_array, int[] outputDimensions)
        {
            Mat result_mat = new(rows: outputDimensions[1], cols: outputDimensions[2], MatType.CV_32F, result_array);
            result_mat = result_mat.T();
            List<Rect2d> position_boxes = [];
            List<int> class_ids = [];
            List<float> confidences = [];
            List<float> rotations = [];
            // Preprocessing output results
            for (int i = 0; i < result_mat.Rows; i++)
            {
                using Mat classes_scores = new(result_mat, new Rect(4, i, outputDimensions[1] - 5, 1));//当前检测类的列index=outputDimensions[1] - 5
                Point max_classId_point, min_classId_point;
                double max_score, min_score;
                // Obtain the maximum value and its position in a set of data
                Cv2.MinMaxLoc(classes_scores, out min_score, out max_score,
                    out min_classId_point, out max_classId_point);
                // Confidence level between 0 ~ 1
                // Obtain identification box information
                if (max_score > ScoreThreshold)
                {
                    float cx = result_mat.At<float>(i, 0);
                    float cy = result_mat.At<float>(i, 1);
                    float ow = result_mat.At<float>(i, 2);
                    float oh = result_mat.At<float>(i, 3);
                    double x = (cx - 0.5 * ow) * _factor;
                    double y = (cy - 0.5 * oh) * _factor;
                    double width = ow * _factor;
                    double height = oh * _factor;
                    Rect2d box = new()
                    {
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height
                    };

                    position_boxes.Add(box);
                    class_ids.Add(max_classId_point.X);
                    confidences.Add((float)max_score);
                    rotations.Add(result_mat.At<float>(i, outputDimensions[1] - 1));//最后一列，旋转角度
                }
            }

            // NMS 
            CvDnn.NMSBoxes(position_boxes, confidences, (float)ScoreThreshold, (float)NmsThreshold, out int[] indexes);
            List<DetectionResult> detectionResults = [];
            for (int i = 0; i < indexes.Length; i++)
            {
                int index = indexes[i];
                float w = (float)position_boxes[index].Width;
                float h = (float)position_boxes[index].Height;
                float x = (float)position_boxes[index].X + w / 2;
                float y = (float)position_boxes[index].Y + h / 2;
                float r = rotations[index];
                float w_ = w > h ? w : h;
                float h_ = w > h ? h : w;
                r = (float)((w > h ? r : (float)(r + System.Math.PI / 2)) % System.Math.PI);
                RotatedRect rotate = new(new Point2f(x, y), new Size2f(w_, h_), (float)(r * 180.0 / System.Math.PI));

                DetectionResult result = new(rotate, confidences[index], Labels.Length > index ? Labels[index] : string.Empty);
                Console.WriteLine($"YoloDetectionUtility.Result{i}: {result}");
                detectionResults.Add(result);
            }

            if (detectionResults.Count <= 0) return OperationResult.False;

            return new(detectionResults);
        }
    }

    public enum ResizeModes
    {
        ZoomThenPad,
        PadDirectly
    }
}
