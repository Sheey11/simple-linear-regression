using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleLinearRegression {
    public class Model {
        public double LearingRate { get; set; }
        private double a, b;
        public int CurrentEpoch { get; private set; }
        public int CurrentStep { get; private set; }
        public double Loss { get; private set; }

        private Point[] TrainDataset;

        public EventHandler OnTrain;
        public EventHandler OnTrainFinished;

        public Model(double learningRate) {
            LearingRate = learningRate;
        }

        public async void Train(Point[] dataset, int epoch) {
            TrainDataset = dataset;
            a = -1;
            b = 5;
            await Task.Run(() => {
                for (int e = 0; e <= epoch; e++) {
                    for (int s = 0; s < dataset.Length; s++) {
                        AdjastParm();
                        CurrentStep = s + 1;
                        Loss = Cost();
                        OnTrain(null, null);
                    }
                    CurrentEpoch = e + 1;
                }
            });
            OnTrainFinished(null, null);
        }
        void AdjastParm() {
            // gradient descent
            var temp_a = AverageSumOf((p) => { return (Hypophysis(p.X) - p.Y) * p.X; }, TrainDataset);
            var temp_b = AverageSumOf((p) => { return Hypophysis(p.X) - p.Y; }, TrainDataset);
            a = a - temp_a * LearingRate;
            b = b - temp_b * LearingRate;
        }
        double Hypophysis(double x) {
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
            return AverageSumOf((p) => { return Math.Pow(Hypophysis(p.X) - p.Y, 2); }, TrainDataset);
        }
    }
}
