using Microsoft.Win32;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Epure;
using netDxf.Entities;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;


namespace TestDxfWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        DxfDocument dxfDocument;
        private EpureM epureM;
        private EpureM epureM1;
        private EpureM epureM2;
        private EpureM epureM3;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                dxfDocument = DxfDocument.Load(dialog.FileName);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            epureM = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M").ToArray());
            epureM1 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M1").ToArray());
            epureM2 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M2").ToArray());
            epureM3 = new EpureM(dxfDocument.Lines.Where(x => x.Layer.Name == "M3").ToArray());
            epureM.PlotEpure();



            #region  Coefficients

            var coefficients = new double[9];

            var d11 = EpureM.MultiplicationEpure(epureM1, epureM1);
            var d12 = EpureM.MultiplicationEpure(epureM1, epureM2);
            var d13 = EpureM.MultiplicationEpure(epureM1, epureM3);
            var d22 = EpureM.MultiplicationEpure(epureM2, epureM2);
            var d23 = EpureM.MultiplicationEpure(epureM2, epureM3);
            var d33 = EpureM.MultiplicationEpure(epureM3, epureM3);
            var d1p = EpureM.MultiplicationEpure(epureM, epureM1);
            var d2p = EpureM.MultiplicationEpure(epureM, epureM2);
            var d3p = EpureM.MultiplicationEpure(epureM, epureM3);

            coefficients[0] = d11;
            coefficients[1] = d12;
            coefficients[2] = d13;
            coefficients[3] = d22;
            coefficients[4] = d23;
            coefficients[5] = d33;
            coefficients[6] = d1p;
            coefficients[7] = d2p;
            coefficients[8] = d3p;

            #endregion


            
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


            
            double.TryParse((worksheet.Cells[8, 6] as Excel.Range)?.Value.ToString(),out double x1);
            double.TryParse((worksheet.Cells[9, 6] as Excel.Range)?.Value.ToString(), out double x2);
            double.TryParse((worksheet.Cells[10, 6] as Excel.Range)?.Value.ToString(), out double x3);

            EpureM epureM1X = epureM1.MultiplicationEpure(x1);
            EpureM epureM2X = epureM2.MultiplicationEpure(x2);
            EpureM epureM3X = epureM3.MultiplicationEpure(x3);

            EpureM epureOk = EpureM.SumEpureM(epureM1X, epureM2X, epureM3X, epureM);

            Word.Application app = new Word.Application();

            var doc = app.Documents.Add(@"C:\Users\Николай\Desktop\Шаблон.docx");
            doc.Activate();

            var wBookmarks = doc.Bookmarks;

            foreach (Word.Bookmark mark in wBookmarks)
            {
                if (mark.Name == "d11" || mark.Name == "d11_")
                    mark.Range.Text = d11.ToString("+#.###;-#.###");

                if (mark.Name == "d12" || mark.Name == "d12_" || mark.Name == "d21_")
                    mark.Range.Text = d12.ToString("+#.###;-#.###");

                if (mark.Name == "d13" || mark.Name == "d13_" || mark.Name == "d31_")
                    mark.Range.Text = d13.ToString("+#.###;-#.###");

                if (mark.Name == "d22" || mark.Name == "d22_")
                    mark.Range.Text = d22.ToString("+#.###;-#.###");

                if (mark.Name == "d23" || mark.Name == "d23_" || mark.Name == "d32_")
                    mark.Range.Text = d23.ToString("+#.###;-#.###");

                if (mark.Name == "d33" || mark.Name == "d33_")
                    mark.Range.Text = d33.ToString("+#.###;-#.###");

                if (mark.Name == "d1p" || mark.Name == "d1p_")
                    mark.Range.Text = d1p.ToString("+#.###;-#.###");

                if (mark.Name == "d2p" || mark.Name == "d2p_")
                    mark.Range.Text = d2p.ToString("+#.###;-#.###");

                if (mark.Name == "d3p" || mark.Name == "d3p_")
                    mark.Range.Text = d3p.ToString("+#.###;-#.###");

                if (mark.Name == "x1")
                    mark.Range.Text = x1.ToString("+#.###;-#.###");

                if (mark.Name == "x2")
                    mark.Range.Text = x2.ToString("+#.###;-#.###");

                if (mark.Name == "x3")
                    mark.Range.Text = x3.ToString("+#.###;-#.###");
            }

            doc.SaveAs(@"e:\Програмирование\Перемножение эпюр\Шаблон.docx");
            doc.Close();

        }
    }
}
