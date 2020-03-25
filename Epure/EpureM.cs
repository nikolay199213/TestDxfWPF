using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epure
{
    public class EpureM
    {
        private EpureLine[] epureLines;

        public int Length => epureLines.Length;

        public EpureM(Line[] lines)
        {
            epureLines = new EpureLine[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                epureLines[i] = new EpureLine(lines[i]);
            }
        }

        public EpureM(int length)
        {
            epureLines = new EpureLine[length];
            for (int i = 0; i < length; i++)
            {
                epureLines[i] = new EpureLine();
            }
        }

        /// <summary>
        /// Перемножение эпюр(массивов участков)
        /// </summary>
        /// <returns></returns>
        public static double MultiplicationEpure(EpureM m1EpureLines, EpureM m2EpureLines)
        {
            double sum = 0;
            for (int i = 0; i < m1EpureLines.epureLines.Length; i++)
            {
                sum += EpureLine.MultiplicationEpureLine(m1EpureLines.epureLines[i], m2EpureLines.epureLines[i]);
            }

            return sum;
        }

        /// <summary>
        /// Перемножение эпюры на число
        /// </summary>
        /// <param name="mEpure"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public EpureM MultiplicationEpure(double x)
        {
            EpureM newEpureM = new EpureM(this.Length);
            for (int i = 0; i < this.Length; i++)
            {
                newEpureM.epureLines[i] = this.epureLines[i].MultiplicationEpureLine(x);
            }

            return newEpureM;
        }

        public static EpureM SumEpureM(params EpureM[] epureMs)
        {
            EpureM newEpureM = new EpureM(epureMs[0].Length);
            for (int i = 0; i < newEpureM.Length; i++)
            {
                for (int j = 0; j < epureMs.Length; j++)
                {
                    newEpureM.epureLines[i] = EpureLine.SumEpureLine(epureMs[j].epureLines[i], newEpureM.epureLines[i]);
                }
            }

            return newEpureM;
        }

        public void PlotEpure()
        {
            DxfDocument dxfDocument = new DxfDocument();
            //Печать рамы
            for (int i = 0; i < epureLines.Length; i++)
            {
                Line line = new Line(epureLines[i].StartPoint, epureLines[i].EndPoint);
                dxfDocument.AddEntity(line);
            }

            //Печать эпюр. Нужно добавить кф отображения, доработать угол
            for (int i = 0; i < epureLines.Length; i++)
            {
                double coefficient = 15;
                var x = Math.Abs(epureLines[i].StartPoint.X - epureLines[i].EndPoint.X);
                var y = Math.Abs(epureLines[i].StartPoint.Y - epureLines[i].EndPoint.Y);
                var hypotenuse = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                var cos = x / hypotenuse;
                var sin = y / hypotenuse;
                //var startMoment = Math.Sqrt(Math.Pow(epureLines[i].StartMoment, 2) / 2);
                var pointStartMoment = new Vector2(epureLines[i].StartPoint.X + epureLines[i].StartMoment * sin / coefficient,
                    epureLines[i].StartPoint.Y + epureLines[i].StartMoment * cos / coefficient);
                //var middleMoment = Math.Sqrt(Math.Pow(epureLines[i].MiddleMoment, 2) / 2);
                var pointMiddle = new Vector2((epureLines[i].StartPoint.X + epureLines[i].EndPoint.X) / 2,
                    (epureLines[i].StartPoint.Y + epureLines[i].EndPoint.Y) / 2);
                var pointMidlleMoment = new Vector2((epureLines[i].StartPoint.X + epureLines[i].EndPoint.X) / 2 + epureLines[i].MiddleMoment * sin / coefficient,
                    (epureLines[i].StartPoint.Y + epureLines[i].EndPoint.Y) / 2 + epureLines[i].MiddleMoment * cos / coefficient);
                //var endtMoment = Math.Sqrt(Math.Pow(epureLines[i].EndMoment, 2) / 2);
                var pointEndMoment = new Vector2(epureLines[i].EndPoint.X + epureLines[i].EndMoment * sin / coefficient,
                    epureLines[i].EndPoint.Y + epureLines[i].EndMoment * cos / coefficient);
                Line line1 = new Line(epureLines[i].StartPoint, pointStartMoment);
                Line line2 = new Line(pointMiddle, pointMidlleMoment);
                Line line3 = new Line(epureLines[i].EndPoint, pointEndMoment);
                Line line4 = new Line(pointStartMoment, pointMidlleMoment);
                Line line5 = new Line(pointMidlleMoment, pointEndMoment);
                dxfDocument.AddEntity(line1);
                dxfDocument.AddEntity(line2);
                dxfDocument.AddEntity(line3);
                dxfDocument.AddEntity(line4);
                dxfDocument.AddEntity(line5);

            }

            dxfDocument.Save(@"e:\Програмирование\Перемножение эпюр\plot.dxf");
        }
    }
}
