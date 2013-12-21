using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Office.Interop.Excel;


/*
 * 
 * Reference : http://forum.codecall.net/topic/71788-reading-excel-files-in-c/
 */

namespace Sudoku
{
    class IOHelper
    {

        private static ApplicationClass appExcel;
        private static Workbook newWorkbook = null;
        private static _Worksheet objsheet = null;



        public IOHelper(String path)
        {
            excel_init(path);
        }


        public int[,] GetGrid()
        {
            string[] cell_addresses = new string[] { "B5", "B6", "B7", "B8", "B9", "B10", "B11", "B12", "B13", "C5", "C6", "C7", "C8", "C9", "C10", "C11", "C12", "C13", "D5", "D6", "D7", "D8", "D9", "D10", "D11", "D12", "D13", "E5", "E6", "E7", "E8", "E9", "E10", "E11", "E12", "E13", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "G5", "G6", "G7", "G8", "G9", "G10", "G11", "G12", "G13", "H5", "H6", "H7", "H8", "H9", "H10", "H11", "H12", "H13", "I5", "I6", "I7", "I8", "I9", "I10", "I11", "I12", "I13", "J5", "J6", "J7", "J8", "J9", "J10", "J11", "J12", "J13" };
            
            int[,] grid = new int[9,9];

            int address_iterator = 0;

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string cell_address = cell_addresses[address_iterator++];
                    string colValueStr = excel_getValue(cell_address);
                    int colValue = String.IsNullOrEmpty(colValueStr) ? 0 : Convert.ToInt32(colValueStr);
                    grid[row,col] = colValue;
                   
                }
            }

            return grid;
        }


        public void WriteSolution(int[,] grid)
        {
            var range = objsheet.get_Range("B5:J13");
            range.Value2 = grid;
            excel_close();

        }




        //Method to initialize opening Excel
        void excel_init(String path)
        {
            appExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();

            if (System.IO.File.Exists(path))
            {
                // then go and load this into excel
                newWorkbook = appExcel.Workbooks.Open(path, true, true);
                objsheet = (_Worksheet)appExcel.ActiveWorkbook.ActiveSheet;
            }
            else
            {
                Console.WriteLine("Unable to open file!");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appExcel);
                appExcel = null;
                //System.Windows.Forms.Application.Exit();
            }

        }

        //Method to get value; cellname is A1,A2, or B1,B2 etc...in excel.
        string excel_getValue(string cellname)
        {
            string value = string.Empty;
            try
            {
                value = objsheet.get_Range(cellname).get_Value().ToString();


            }
            catch
            {
                value = "";
            }

            return value;
        }

        //Method to close excel connection
        void excel_close()
        {
            if (appExcel != null)
            {
                try
                {

                    newWorkbook.Close();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(appExcel);
                    appExcel = null;
                    objsheet = null;
                }
                catch (Exception ex)
                {
                    appExcel = null;
                    //MessageBox.Show("Unable to release the Object " + ex.ToString());
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

    }
}
