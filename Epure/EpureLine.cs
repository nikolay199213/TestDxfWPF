using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Line> AddArrow(double startPointX, double startPointY, Directions direction)
        {
            Vector2 startPoint = new Vector2(startPointX, startPointY);
            Vector2 point1;
            Vector2 point2;
            switch (direction)
            {
                case Directions.Top:
                    point1 = new Vector2(startPointX-1, startPointY-1);
                    point2 = new Vector2(startPointX+1, startPointY-1);
                    break;
                case Directions.Right:
                    point1 = new Vector2(startPointX - 1, startPointY + 1);
                    point2 = new Vector2(startPointX - 1, startPointY - 1);
                    break;
                case Directions.Bottom:
                    point1 = new Vector2(startPointX - 1, startPointY + 1);
                    point2 = new Vector2(startPointX + 1, startPointY - 1);
                    break;
                default:
                    point1 = new Vector2(startPointX + 1, startPointY + 1);
                    point2 = new Vector2(startPointX + 1, startPointY - 1);
                    break;
            }
            return new List<Line>() {new Line(startPoint, point1), new Line(startPoint, point2)};
        }

        public void PrintQ(int number)
        {
            
            var pivot = new Line(Vector2.Zero, new Vector2(0, 8));
            var lineEnd = new Line(new Vector2(-3,8), new Vector2(3,8));
            var arrowEnd1 = new Line(new Vector2(2,9), new Vector2(3,8));
            var arrowEnd2 = new Line(new Vector2(2, 7), new Vector2(3, 8));
            var lineStart = new Line(new Vector2(-3, 0), new Vector2(3, 0));
            var arrowStart1 = new Line(new Vector2(-2, 1), new Vector2(-3, 0));
            var arrowStart2 = new Line(new Vector2(-2, -1), new Vector2(-3, 0));
            var lines = new List<Line>() {pivot, lineEnd, lineStart, arrowEnd1, arrowEnd2, arrowStart1, arrowStart2};
            foreach (var line in lines)
            {
                line.Lineweight = Lineweight.W30;
            }

            var textEnd = new Text("Q"+number.ToString()+"к", new Vector2(2,8), 0);
            var textStart = new Text("Q" + number.ToString() + "н", new Vector2(-2, 0), 0);

        }

    }
}
