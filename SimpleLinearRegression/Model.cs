using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleLinearRegression {
    public class Model {
        public double LearingRate { get; set; }
        public double a { get; private set; }
        public double b { get; private set; }
        public int CurrentEpoch { get; private set; }
        public int CurrentStep { get; private set; }
        public double Loss { get { return Cost(); } }

        private Point[] TrainDataset;

        public EventHandler OnTrain;
        //public EventHandler OnTrainFinished;

        public Model(double learningRate) {
            LearingRate = learningRate;
        }

        public void Train(Point[] dataset, int epoch) {
            TrainDataset = dataset;
            a = 5;
            b = 1;
            for (int e = 0; e < epoch; e++) {
                CurrentEpoch = e + 1;
                for (int s = 0; s < dataset.Length; s++) {
                    AdjastParm();
                    CurrentStep = s + 1;
                    OnTrain(null, null);
                }
            }
            //OnTrainFinished(null, null);
        }
        void AdjastParm() {
            // gradient descent
            var temp_a = AverageSumOf((p) => { return (Hypothysis(p.X) - p.Y) * p.X; }, TrainDataset);
            var temp_b = AverageSumOf((p) => { return Hypothysis(p.X) - p.Y; }, TrainDataset);
            a = a - temp_a * LearingRate;
            b = b - temp_b * LearingRate;
        }
        double Hypothysis(double x) {
            return a * x + b;
        }
        double AverageSumOf(Func<Point, double> func, Point[] parms) {
            var r = 0d;
            foreach (Point p in parms) {
                r += func(p);
            }
            return r / parms.Length;
        }
        double Cost() {
            return AverageSumOf((p) => { return Math.Pow(Hypothysis(p.X) - p.Y, 2); }, TrainDataset);
        }
    }
}
