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
        #region Properties
        public double Length { get; set; }
        public double StartMoment { get; set; }
        public double MiddleMoment { get; set; }
        public double EndMoment { get; set; }
        public int Rigidity { get; set; }
        public double QStart => (EndMoment - StartMoment + Load * Math.Pow(Length, 2) / 2) / Length;

        public double QEnd => (EndMoment - StartMoment - Load * Math.Pow(Length, 2) / 2) / Length;
        //Добавить метод
        public double Load { get; set; }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }

        #endregion

        #region Constructors
        
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
        #endregion

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

        private IEnumerable<Line> AddArrow(Vector2 startPoint,  Directions direction)
        {
            Vector2 point1;
            Vector2 point2;
            switch (direction)
            {
                case Directions.Top:
                    point1 = new Vector2(startPoint .X- 1, startPoint.Y - 1);
                    point2 = new Vector2(startPoint.X + 1, startPoint.Y - 1);
                    break;
                case Directions.Right:
                    point1 = new Vector2(startPoint.X - 1, startPoint.Y + 1);
                    point2 = new Vector2(startPoint.X - 1, startPoint.Y - 1);
                    break;
                case Directions.Bottom:
                    point1 = new Vector2(startPoint.X - 1, startPoint.Y + 1);
                    point2 = new Vector2(startPoint.X + 1, startPoint.Y + 1);
                    break;
                default:
                    point1 = new Vector2(startPoint.X + 1, startPoint.Y + 1);
                    point2 = new Vector2(startPoint.X + 1, startPoint.Y - 1);
                    break;
            }
            return new List<Line>() {new Line(startPoint, point1), new Line(startPoint, point2)};
        }

        private IEnumerable<Line> AddMomentLines(Vector2 centerPoint, double moment)
        {
            var rightPoint = new Vector2(centerPoint.X+2, centerPoint.Y);
            var leftPoint = new Vector2(centerPoint.X-2, centerPoint.Y);
            Vector2 rightPoint2;
            Vector2 leftPoint2;
            if (moment > 0)
            {
                rightPoint2 = new Vector2(rightPoint.X, rightPoint.Y+3);
                leftPoint2 = new Vector2(leftPoint.X, leftPoint.Y-3);
                var  line1 = new Line(rightPoint, rightPoint2);
                var line2 = new Line(leftPoint, leftPoint2);
                var arrow1 = AddArrow(rightPoint2, Directions.Top);
                var arrow2 = AddArrow(leftPoint2, Directions.Bottom);
                var lines = new List<Line>() {line1, line2};
                return lines.Union(arrow1).Union(arrow2).ToList();
            }
            else if (moment < 0)
            {
                rightPoint2 = new Vector2(rightPoint.X, rightPoint.Y - 3);
                leftPoint2 = new Vector2(leftPoint.X, leftPoint.Y + 3);
                var line1 = new Line(rightPoint, rightPoint2);
                var line2 = new Line(leftPoint, leftPoint2);
                var arrow1 = AddArrow(rightPoint2, Directions.Bottom);
                var arrow2 = AddArrow(leftPoint2, Directions.Top);
                var lines = new List<Line>() { line1, line2 };
                return lines.Union(arrow1).Union(arrow2).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Добавляет линии нагрузки
        /// </summary>
        /// <param name="startPoint">Нижняя точка стержня</param>
        /// <param name="endPoint">Верхняя точка стержна</param>
        /// <param name="direction">Направление нагрузки</param>
        /// <returns></returns>
        private IEnumerable<Line> AddLoad(Vector2 startPoint, Vector2 endPoint, Directions direction)
        {
            var length = endPoint.Y - startPoint.Y;
            var mainLine = new Line(startPoint, endPoint);
            var firstLinePoint = new Vector2(endPoint.X, endPoint.Y+length/4);
            var secondLinePoint = new Vector2(endPoint.X, endPoint.Y+length/2);
            var thirdLinePoint = new Vector2(endPoint.X, startPoint.Y-length/4);
            Vector2 firstLinePoint2 = Vector2.Zero;
            Vector2 secondLinePoint2 = Vector2.Zero;
            Vector2 thirdLinePoint2 = Vector2.Zero;
            //Доделать для других направлений
            switch (direction)
            {
                case Directions.Right:
                    firstLinePoint2 = new Vector2(endPoint.X-1, firstLinePoint.Y);
                    secondLinePoint2 = new Vector2(endPoint.X-1, secondLinePoint.Y);
                    thirdLinePoint2 = new Vector2(endPoint.X-1, thirdLinePoint.Y);
                    break;
            }
            var firstLine = new Line(firstLinePoint, firstLinePoint2);
            var secondLine = new Line(secondLinePoint, secondLinePoint2);
            var thirdLine = new Line(thirdLinePoint, thirdLinePoint2);

            var firstArrow = AddArrow(firstLinePoint, Directions.Right);
            var secondArrow = AddArrow(secondLinePoint, Directions.Right);
            var thirdArrow = AddArrow(thirdLinePoint, Directions.Right);
            var lines = new List<Line>() {firstLine, secondLine, thirdLine};
            return lines.Union(firstArrow).Union(secondArrow).Union(thirdArrow).ToList();

        }

        //Добавить нагрузку
        public void PrintQ(int number)
        {
            
            var pivot = new Line(Vector2.Zero, new Vector2(0, 10));
            var lineEnd = new Line(new Vector2(-2,10), new Vector2(4,10));
            var lineStart = new Line(new Vector2(-4, 0), new Vector2(2, 0));
            var arrowEnd = AddArrow(new Vector2(4, 10), Directions.Right);
            var arrowStart = AddArrow(new Vector2(-4, 0), Directions.Left);
            var endMoment = AddMomentLines(new Vector2(0, 10), EndMoment) ?? new List<Line>();
            var startMoment = AddMomentLines(new Vector2(0, 0), StartMoment) ?? new List<Line>();
            var lines = new List<Line>() {pivot, lineEnd, lineStart};
            lines = lines.Union(arrowEnd).Union(arrowStart).Union(endMoment).Union(startMoment).ToList();
            foreach (var line in lines)
            {
                line.Lineweight = Lineweight.W30;
            }

            var textEnd = new Text("Q"+number.ToString()+"к", new Vector2(5,10), 1);
            var textStart = new Text("Q" + number.ToString() + "н", new Vector2(-7, 0), 1);
            var textEndMoment = new Text(EndMoment.ToString("#.###; #.###"), new Vector2(-8,12),1);
            var textStartMoment = new Text(StartMoment.ToString("#.###; #.###"), new Vector2(5, 0), 1);
            var textLength = new Text(Length.ToString()+"м", new Vector2(1,5), 1);

            List<Line> load = new List<Line>();
            if (Load != 0)
            {
                load = AddLoad(StartPoint, EndPoint, Directions.Right).ToList();
            }

            List<EntityObject> objects = new List<EntityObject>(){textEnd, textStart, textEndMoment, textStartMoment, textLength};
            objects = objects.Union(lines).Union(load).ToList();

            DxfDocument printQ = new DxfDocument();
            printQ.AddEntity(objects);
            printQ.Save(@"e:\Програмирование\Перемножение эпюр\printQ.dxf");

        }

    }
}
