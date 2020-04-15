using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Bot_liverpool_VT.Class
{
   
    class Automation
    {
      
        static bool find = false;

        public void WaitUntilElementExistsByXPath(IWebDriver driver, string xpath, int loopTime)
        {
            
            int pasadas = 0;
            do
            {
                try
                {
                    driver.FindElement(By.XPath(xpath));
                    find = true;
                }
                catch (Exception)
                {
                    find = false;
                    pasadas++;
                    if (pasadas >= loopTime)
                    {
                       
                        //driver.Close();
                        //driver.Quit();
                        find = true;
                        driver.SwitchTo().DefaultContent();
                        driver.FindElement(By.XPath(xpath));
                    }

                }

            } while (find == false);
        }

        public void WaitUntilElementExistsById(IWebDriver driver, string id, int loopTime)
        {
            //bool find = false;
            int pasadas = 0;
            do
            {
                try
                {
                    driver.FindElement(By.Id(id));
                    find = true;
                }
                catch (Exception)
                {
                    find = false;
                    pasadas++;
                    if (pasadas == loopTime)
                    {
                        driver.FindElement(By.Id(id));
                        find = true;

                        ///driver.Close();
                        //driver.Quit();
                    }

                }

            } while (find == false);
        }

        public DataTable TableByXPath(IWebDriver Driver, string XPathElement, string TagRow, string TagCell, string[] Columns)
        {

            IWebElement Table;
            ReadOnlyCollection<IWebElement> Rows;
            ReadOnlyCollection<IWebElement> Cells;
            string[] Datos = new string[Columns.Count()];

            DataTable dt = new DataTable();
            foreach (var item in Columns)
                dt.Columns.Add(new DataColumn(item));

            WaitUntilElementExistsByXPath(Driver, XPathElement, 60);

            Table = Driver.FindElement(By.XPath(XPathElement));
            Rows = Table.FindElements(By.XPath(".//" + TagRow));

            foreach (var line in Rows)
            {

                Cells = line.FindElements(By.XPath(".//" + TagCell));

                for (int i = 0; i < Cells.Count; i++)
                {
                    Datos[i] = Cells[i].Text;
                }

                dt.Rows.Add(Datos);

            }

            return dt;
        }

        public void ClickElementTableByXPath(IWebDriver Driver, string XPathElement, string TagRow, string TagCell, string TagElementClick, int Row, int Col)
        {

            IWebElement Table;
            ReadOnlyCollection<IWebElement> Rows;
            ReadOnlyCollection<IWebElement> Cells;

            Table = Driver.FindElement(By.XPath(XPathElement));
            Rows = Table.FindElements(By.XPath(".//" + TagRow));
            Cells = Rows[Row].FindElements(By.XPath(".//" + TagCell));

            Cells[Col].FindElement(By.XPath(".//" + TagElementClick)).Click();

        }


        public void WaitLoad(IWebDriver driver, string xpath, int loopTime)
        {
            bool find = false;
            int pasadas = 0;
            do
            {
                try
                {
                    Thread.Sleep(500);
                    driver.FindElement(By.XPath(xpath));
                    find = true;
                }
                catch (Exception)
                {
                    find = false;
                    pasadas++;
                  
                }

            } while (find == false || pasadas == loopTime);
        }
    }
}
