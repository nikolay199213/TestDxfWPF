using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epure
{
    public class EpureLine
    {
        public double Length { get; set; }
        public double StartMoment { get; set; }
        public double MiddleMoment { get; set; }
        public double EndMoment { get; set; }
        public int Rigidity { get; set; }
        public  double QStart => (EndMoment - StartMoment + Load * Math.Pow(Length, 2) / 2) / Length;

        public double QEnd => (EndMoment - StartMoment - Load * Math.Pow(Length, 2) / 2) / Length;
        //Добавить метод
        public  double Load { get; set; }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }

        public EpureLine()
        {

        }

        public EpureLine(EpureLine epureLine)
        {
            Length = epureLine.Length;
            StartPoint = epureLine.StartPoint;
            MiddleMoment = epureLine.MiddleMoment;
            EndMoment = epureLine.EndMoment;
            Rigidity = epureLine.Rigidity;
            StartPoint = epureLine.StartPoint;
            EndPoint = epureLine.EndPoint;

        }

        public EpureLine(Line line)
        {
            StartPoint = new Vector2(line.StartPoint.X, line.StartPoint.Y);
            EndPoint = new Vector2(line.EndPoint.X, line.EndPoint.Y);

            Length = Math.Sqrt(Math.Pow((EndPoint.X - StartPoint.X), 2) + Math.Pow((EndPoint.Y - StartPoint.Y), 2));
            StartMoment = line.StartPoint.Z;
            EndMoment = line.EndPoint.Z;
            MiddleMoment = line.Thickness;
            Rigidity = (int)line.LinetypeScale;
        }
        /// <summary>
        /// Перемножение участков эпюры методом Симпсона
        /// </summary>
        /// <param name="epure2"></param>
        /// <returns></returns>
        public static double MultiplicationEpureLine(EpureLine epure1, EpureLine epure2)
        {
            return (epure1.StartMoment * epure2.StartMoment + 4 * epure1.MiddleMoment * epure2.MiddleMoment +
                    epure1.EndMoment * epure2.EndMoment) * epure1.Length / (6 * epure1.Rigidity);
        }

        /// <summary>
        /// Перемножение участка эпюры на число
        /// </summary>
        /// <param name="x1"></param>
        /// <returns></returns>
        public EpureLine MultiplicationEpureLine(double x1)
        {
            return new EpureLine
            {
                StartMoment = this.StartMoment * x1,
                MiddleMoment = this.MiddleMoment * x1,
                EndMoment = this.EndMoment * x1,
                Length = this.Length,
                StartPoint = this.StartPoint,
                EndPoint = this.EndPoint,
                Rigidity = this.Rigidity
            };
        }

        /// <summary>
        /// Суммирование участков эпюр
        /// </summary>
        /// <param name="epureLine1"></param>
        /// <param name="epureLine2"></param>
        /// <returns></returns>
        public static EpureLine SumEpureLine(EpureLine epureLine1, EpureLine epureLine2)
        {
            EpureLine newEpureLine = new EpureLine()
            {
                Length = epureLine1.Length,
                Rigidity = epureLine1.Rigidity,
                StartPoint = epureLine1.StartPoint,
                EndPoint = epureLine1.EndPoint,
                StartMoment = epureLine1.StartMoment + epureLine2.StartMoment,
                MiddleMoment = epureLine1.MiddleMoment + epureLine2.MiddleMoment,
                EndMoment = epureLine1.EndMoment + epureLine2.EndMoment
            };

            return newEpureLine;
        }

        public void PlotQ()
        {

        }

    }
}
