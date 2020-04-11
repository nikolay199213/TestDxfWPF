using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epure;
using netDxf;
using netDxf.Entities;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DxfDocument dxfDocument;
            dxfDocument = DxfDocument.Load(@"C:\Users\Николай\Desktop\шаблон_Эпюр.dxf");
            var test = dxfDocument.Blocks.Items.Where(x => x.Name == "тест1").First().AttributeDefinitions.Where(x => x.Key == "ТЕСТОВЫЙ_АТРИБУТ").First().Value.Value;
            var epureM = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M").ToArray());
            var epureM1 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M1").ToArray());
            var epureM2 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M2").ToArray());
            var epureM3 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M3").ToArray());

            var d11 = EpureM.MultiplicationEpure(epureM1, epureM1);
            var d12 = EpureM.MultiplicationEpure(epureM1, epureM2);
            var d13 = EpureM.MultiplicationEpure(epureM1, epureM3);
            var d22 = EpureM.MultiplicationEpure(epureM2, epureM2);
            var d23 = EpureM.MultiplicationEpure(epureM2, epureM3);
            var d33 = EpureM.MultiplicationEpure(epureM3, epureM3);
            var d1p = EpureM.MultiplicationEpure(epureM, epureM1);
            var d2p = EpureM.MultiplicationEpure(epureM, epureM2);
            var d3p = EpureM.MultiplicationEpure(epureM, epureM3);

            Excel.Workbook workbook;
            Excel.Worksheet worksheet;

            Excel.Application application = new Excel.Application();

            workbook = application.Workbooks.Open(@"e:\Програмирование\Перемножение эпюр\матрица.xlsx");

            worksheet = workbook.Worksheets.get_Item(1);
            worksheet.Cells[3, 2] = d11;
            worksheet.Cells[4, 3] = d22;
            worksheet.Cells[5, 4] = d33;
            worksheet.Cells[3, 3] = d12;
            worksheet.Cells[3, 4] = d13;
            worksheet.Cells[4, 4] = d23;
            worksheet.Cells[3, 6] = -d1p;
            worksheet.Cells[4, 6] = -d2p;
            worksheet.Cells[5, 6] = -d3p;



            double.TryParse((worksheet.Cells[8, 6] as Excel.Range)?.Value.ToString(), out double x1);
            double.TryParse((worksheet.Cells[9, 6] as Excel.Range)?.Value.ToString(), out double x2);
            double.TryParse((worksheet.Cells[10, 6] as Excel.Range)?.Value.ToString(), out double x3);

            EpureM epureM1X = epureM1.MultiplicationEpure(x1);
            EpureM epureM2X = epureM2.MultiplicationEpure(x2);
            EpureM epureM3X = epureM3.MultiplicationEpure(x3);

            EpureM epureOk = EpureM.SumEpureM(epureM1X, epureM2X, epureM3X, epureM);
            epureOk.AddLoad(20,1);

            epureOk.PrinyQ();
            //epureOk.PrintM();
            Point point = new Point();
        }
    }
}
