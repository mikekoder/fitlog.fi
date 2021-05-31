using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Web
{
    public static class TrainingUtils
    {
        public static decimal Calculate1RM(decimal reps, decimal weights)
        {
            var oneRepMax = new[]
            {
                Epley((double)reps,(double)weights),
                Brzycki((double)reps,(double)weights),
                Lander((double)reps,(double)weights),
                Lombardi((double)reps,(double)weights),
                Mayhew((double)reps,(double)weights),
                OConner((double)reps,(double)weights),
                Wathan((double)reps,(double)weights)
            }.Average();

            return (decimal)oneRepMax;
        }
        public static decimal CalculateWeights(int reps, decimal oneRepMax)
        {
            var weights = new[]
            {
                EpleyReverse(reps,(double)oneRepMax),
                BrzyckiReverse(reps,(double)oneRepMax),
                LanderReverse(reps,(double)oneRepMax),
                LombardiReverse(reps,(double)oneRepMax),
                MayhewReverse(reps,(double)oneRepMax),
                OConnerReverse(reps,(double)oneRepMax),
                WathanReverse(reps,(double)oneRepMax)
            }.Average();

            return (decimal)weights;
        }
        private static double Epley(double r, double w)
        {
            return w * (1 + r / 30);
        }
        private static double Brzycki(double r, double w)
        {
            return w * 36 / (37 - r);
        }
        private static double Lander(double r, double w)
        {
            return (100 * w) / (101.3 - 2.67123 * r);
        }
        private static double Lombardi(double r, double w)
        {
            return w * Math.Pow(r, 0.1);
        }
        private static double Mayhew(double r, double w)
        {
            return (100 * w) / (52.2 + 41.9 * Math.Pow(Math.E, -0.055 * r));
        }
        private static double OConner(double r, double w)
        {
            return w * (1 + 0.025 * r);
        }
        private static double Wathan(double r, double w)
        {
            return (100 * w) / (48.8 + 53.8 * Math.Pow(Math.E, -0.075 * r));
        }
        private static double EpleyReverse(double r, double max)
        {
            return max / (1 + r / 30);
        }
        private static double BrzyckiReverse(double r, double max)
        {
            return max / (36 / (37 - r));
        }
        private static double LanderReverse(double r, double max)
        {
            return (max * (101.3 - 2.67123 * r)) / 100;
        }
        private static double LombardiReverse(double r, double max)
        {
            return max / Math.Pow(r, 0.1);
        }
        private static double MayhewReverse(double r, double max)
        {
            return (max * (52.2 + 41.9 * Math.Pow(Math.E, -0.055 * r))) / 100;
        }
        private static double OConnerReverse(double r, double max)
        {
            return max / (1 + 0.025 * r);
        }
        private static double WathanReverse(double r, double max)
        {
            return (max * (48.8 + 53.8 * Math.Pow(Math.E, -0.075 * r))) / 100;
        }
    }
}
