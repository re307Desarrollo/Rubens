using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Bot_liverpool_VT.Class;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Interactions;
using System.IO;
using System.Diagnostics;

namespace Bot_liverpool_VT
{   //se le modifico tiempo de espera sleep para que pudiera aceeptar 
    //29_06_2019 actualizacion de proceso do{}while() para mejor eficiencia de la descarga
    public class Program
    {
        private static string User = System.Configuration.ConfigurationManager.AppSettings["User"].ToString();
        private static string Password = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
        private static string URL = System.Configuration.ConfigurationManager.AppSettings["URL"].ToString();
        private static string Seccion = System.Configuration.ConfigurationManager.AppSettings["Seccion"].ToString();
        private static string Dias = System.Configuration.ConfigurationManager.AppSettings["Dias"].ToString();
        static ChromeOptions op = new ChromeOptions();
        static Automation au = new Automation();
        static DateTime FechaProc;
        static DateTime FechaIni = DateTime.Today;
        static IWebDriver driver;
        static DateTime FechaC1 = DateTime.Today.AddDays(-1);
        static DataTable DTValida = new DataTable();
        static csSQL sql = new csSQL();
        static csSQL sqlP = new csSQL();
        //static DateTime FechaProc;
        static ProcessStartInfo execute = new ProcessStartInfo();
        static DataTable dt = new DataTable();
        static int fecharecuperacion;
        static int aux;
        static int diasP;
        static DateTime FechaProcRecupera;

       [STAThread]
        static void Main()
        {
            Program V_Liver = new Program();
            try
            {
                fecharecuperacion = 1;
                V_Liver.Bot_Concentrado();
                driver.Close();
                driver.Quit();
                MessageBox.Show("Bot Liverpool Ventas ejecutado de forma correcta....");
                
            }
            catch (Exception)
            {
                
                fecharecuperacion = diasP-1;
                aux = 1;
                V_Liver.Bot_Concentrado();
                driver.Close();
                driver.Quit();
                MessageBox.Show("Bot Liverpool Ventas ejecutado de forma correcta....");
            }

        }

      

        public void Bot_Concentrado()
        {

            Credenciales(); //se cuentra el inicio de session 
            Portal(FechaProc); //Interraccion con el portal 
        }

        public void Credenciales()
        {

            op.AddArgument("--unlimited-storage");
            op.AddUserProfilePreference("profile.content_settings.exceptions.automatic_downloads.*.setting", 1);
            op.AddUserProfilePreference("download.prompt_for_download", 0);
            op.AddUserProfilePreference("settings.labs.advanced_filesystem", 1);
            op.AddArgument("--incognito");
            driver = new ChromeDriver(op);
            driver.Navigate().GoToUrl(URL);


            driver.FindElement(By.Id("logonuidfield")).Clear();
            driver.FindElement(By.Id("logonuidfield")).SendKeys(User);
            driver.FindElement(By.Id("logonpassfield")).Clear();
            driver.FindElement(By.Id("logonpassfield")).SendKeys(Password);
            driver.FindElement(By.XPath("//input[@name='uidPasswordLogon']")).Click();
        }

        public void Portal(DateTime FechaProc)
        {
            int encuentra = 0;
            do
            {
                try
                {

                    // REPORTES

                    au.WaitUntilElementExistsByXPath(driver, "//td[@id='navNode_1_1']", 150);

                    string menu = driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Text;
                    driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();
                    menu = driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Text;
                    if (menu != "Reportes")
                    {
                        driver.SwitchTo().DefaultContent();
                        au.WaitUntilElementExistsByXPath(driver, "//td[@id='navNode_1_1']", 150);
                        driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();
                    }
                    driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();

                    // VENTAS DIARIAS
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                    //au.WaitUntilElementExistsByXPath(driver, "//div[@id='DetailedNavigationTree']/div[4]/a", 150);
                    string menu1 = driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Text;
                    driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                    menu1 = driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Text;
                    if (menu1 != "Ventas Diarias")
                        {
                        bool elementocorrecto = false;

                        do
                        {
                            driver.SwitchTo().DefaultContent();
                            driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                            //au.WaitUntilElementExistsByXPath(driver, "//div[@id='DetailedNavigationTree']/div[4]/a", 50);
                            string consulta = driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Text;
                            if (consulta == "Ventas Diarias")
                            {
                                elementocorrecto = true;
                                driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                            }
                            //driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                            
                        }
                        while (elementocorrecto == false);
                        }
                    Thread.Sleep(2000);
                    // SECCION
                    driver.SwitchTo().DefaultContent();
                    au.WaitUntilElementExistsById(driver, "ivuFrm_page0ivu2", 100);
                    driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                    au.WaitUntilElementExistsById(driver, "isolatedWorkArea", 100);
                    driver.SwitchTo().Frame(driver.FindElement(By.Id("isolatedWorkArea")));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);

                    //espera de elemento
                    //*[@id="VAR_VALUE_EXT_2"]
                    string veriventa = "";

                    ////******////
                    //Try portal lento
                    try {  veriventa = driver.FindElement(By.XPath("//*[@id='VARI']/table/tbody/tr/td/table/tbody/tr[1]/td")).Text; }
                    //se realiza la navegación ya que se refresca el driver
                    catch {
                        driver.Navigate().Refresh();
                        
                        driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();
                        menu = driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Text;
                        if (menu != "Reportes")
                        {
                            driver.SwitchTo().DefaultContent();
                            au.WaitUntilElementExistsByXPath(driver, "//td[@id='navNode_1_1']", 150);
                            driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();
                        }
                        driver.FindElement(By.XPath("//td[@id='navNode_1_1']")).Click();

                        // VENTAS DIARIAS
                        driver.SwitchTo().DefaultContent();
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                        //au.WaitUntilElementExistsByXPath(driver, "//div[@id='DetailedNavigationTree']/div[4]/a", 150);
                       
                        driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                        menu1 = driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Text;
                        if (menu1 != "Ventas Diarias")
                        {
                            bool elementocorrecto = false;

                            do
                            {
                                driver.SwitchTo().DefaultContent();
                                driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                                //au.WaitUntilElementExistsByXPath(driver, "//div[@id='DetailedNavigationTree']/div[4]/a", 50);
                                string consulta = driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Text;
                                if (consulta == "Ventas Diarias")
                                {
                                    elementocorrecto = true;
                                    driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                                }
                                //driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();

                            }
                            while (elementocorrecto == false);
                        }
                        Thread.Sleep(2000);
                        // SECCION
                        driver.SwitchTo().DefaultContent();
                        au.WaitUntilElementExistsById(driver, "ivuFrm_page0ivu2", 100);
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                        au.WaitUntilElementExistsById(driver, "isolatedWorkArea", 100);
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("isolatedWorkArea")));
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);

                        ////******////
                        //fin del catch
                    }
                    //navegacion normal
                    
                    if (veriventa != "Entrada de variables de BWR: Ventas Diarias")
                    {
                        driver.SwitchTo().DefaultContent();
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                        driver.FindElement(By.XPath("//div[@id='DetailedNavigationTree']/div[4]/a")).Click();
                        Thread.Sleep(500);
                        // SECCION
                        driver.SwitchTo().DefaultContent();
                        au.WaitUntilElementExistsById(driver, "ivuFrm_page0ivu2", 100);
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                        au.WaitUntilElementExistsById(driver, "isolatedWorkArea", 100);
                        driver.SwitchTo().Frame(driver.FindElement(By.Id("isolatedWorkArea")));
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);

                    }
                    au.WaitUntilElementExistsByXPath(driver, "//input[@id='VAR_VALUE_EXT_2']", 100);
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_EXT_2']")).Clear();
                    Thread.Sleep(2000);
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_EXT_2']")).SendKeys(Seccion);

                    encuentra = 1;

                }
                catch (Exception)
                {
                    encuentra = 0;
                }
            } while (encuentra==0);
            
            if(fecharecuperacion==1)
            {
                DiasPorcesa(1);
               
            }
            else
            {
                
                DiasPorcesa(fecharecuperacion);
            }
            
            
        }
        
        public void DiasPorcesa(int recuperacion)
        {
          string sUsuarioComple = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //(1)DEBIDO A QUE CUENTA CON UN PERIDO DE DESFASE 
            for (diasP = recuperacion; diasP <= int.Parse(Dias); diasP++)
            {


                FechaProc = DateTime.Today;

                if (diasP == -1)
                {
                    FechaProc = FechaProc.AddDays(diasP);
                }
                else if (diasP == 0)
                {
                    FechaProc = FechaProc.AddDays(-1);
                }
                else
                {
                    FechaProc = FechaProc.AddDays(-diasP);
                }

                string AnioProc = FechaProc.Year.ToString();
                string MesProc = FechaProc.Month.ToString();
                string DiaProc = FechaProc.Day.ToString();
                string Final = (DiaProc.PadLeft(2, '0') + "." + MesProc.PadLeft(2, '0') + "." + AnioProc);
                //string Final = "20.10.2019";
                if (diasP == 1||aux==1)
                {
                    aux = 0;

                    // SECCION
                    driver.SwitchTo().DefaultContent();
                    au.WaitUntilElementExistsById(driver, "ivuFrm_page0ivu2", 100);
                    driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                    au.WaitUntilElementExistsById(driver, "isolatedWorkArea", 100);
                    driver.SwitchTo().Frame(driver.FindElement(By.Id("isolatedWorkArea")));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);


                    //au.WaitUntilElementExistsByXPath(driver, "//input[@id='VAR_VALUE_LOW_EXT_14']", 50);
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_LOW_EXT_14']")).Clear();
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_LOW_EXT_14']")).SendKeys(Final);


                    // FECHA FINAL
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_HIGH_EXT_14']")).Clear();
                    driver.FindElement(By.XPath("//input[@id='VAR_VALUE_HIGH_EXT_14']")).SendKeys(Final);

                    // CONSULTAR
                    au.WaitUntilElementExistsByXPath(driver, "//*[@id='VARI']/table/tbody/tr/td/table/tbody/tr[5]/td/table/tbody/tr[1]/td[2]/table/tbody/tr/td/table/tbody/tr/td/a", 100);
                    driver.FindElement(By.XPath("//*[@id='VARI']/table/tbody/tr/td/table/tbody/tr[5]/td/table/tbody/tr[1]/td[2]/table/tbody/tr/td/table/tbody/tr/td/a")).Click();

                    ////////////////////////////////
                    //CLICK ARTICULOS GENERICOS
                    au.WaitUntilElementExistsByXPath(driver, "/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a", 200);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                    string title = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a")).GetAttribute("title");
                    driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a")).Click();
                    Thread.Sleep(1000);
                    title = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a")).GetAttribute("title");
                    if (title == "Desglosar en las líneas")
                    {
                        bool botoncorrecto = false;
                        do
                        {
                            Thread.Sleep(1500);
                            title = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a")).GetAttribute("title");
                           
                            if (title == "Eliminar desglose")
                            {
                                botoncorrecto = true;
                            }
                            if (title == "Desglosar en las líneas")
                            {
                                driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[6]/td[2]/a")).Click();
                            }
                            
                        } while (botoncorrecto==false);
                    }
                    //CLICK ARTICULO 
                    au.WaitUntilElementExistsByXPath(driver, "/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a", 100);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                    string title2 = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a")).GetAttribute("title");
                    driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a")).Click();
                    Thread.Sleep(1000);
                    title2 = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a")).GetAttribute("title");
                    if (title2== "Desglosar en las líneas")
                    {
                        bool botoncorrecto2 = false;
                        do
                        {
                            Thread.Sleep(1500);
                           
                            title2 = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a")).GetAttribute("title");
                            if (title2== "Eliminar desglose")
                            {
                                botoncorrecto2 = true;
                            }
                            if (title == "Desglosar en las líneas")
                            {
                                driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[7]/td[2]/a")).Click();
                            }
                        }
                        while (botoncorrecto2==false);
                    }
                }



                if (diasP != 1)
                {
                    
                    au.WaitUntilElementExistsByXPath(driver, "/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td[6]/a", 100);
                    driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td[6]/a")).Click();

                    driver.SwitchTo().Window(driver.WindowHandles[1]);
                    Thread.Sleep(2000);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);
                    
                    //au.WaitUntilElementExistsByXPath(driver, "//*[@id='SAPBWLOW']", 100);
                    driver.FindElement(By.XPath("//*[@id='SAPBWLOW']")).Clear();
                    driver.FindElement(By.XPath("//*[@id='SAPBWLOW']")).SendKeys(Final);
                    au.WaitUntilElementExistsByXPath(driver, "//*[@id='SAPBWHIGH']", 10);
                    driver.FindElement(By.XPath("//*[@id='SAPBWHIGH']")).Clear();
                    driver.FindElement(By.XPath("//*[@id='SAPBWHIGH']")).SendKeys(Final);
                    au.WaitUntilElementExistsByXPath(driver, "//*[@id='FILTER']/table/tbody/tr/td/table/tbody/tr[21]/td[2]/table/tbody/tr/td[1]/table/tbody/tr/td/table/tbody/tr/td/a", 100);
                    driver.FindElement(By.XPath("//*[@id='FILTER']/table/tbody/tr/td/table/tbody/tr[21]/td[2]/table/tbody/tr/td[1]/table/tbody/tr/td/table/tbody/tr/td/a")).Click();

                    
                  
                       

                    int encuentra =0;
                    int stop1 = 0;
                    stop1 = 0;
                    encuentra = 0;
                    do
                    {
                        try
                        {
                            driver.SwitchTo().Window(driver.WindowHandles[0]);
                           // Thread.Sleep(1500);
                            driver.SwitchTo().Frame(driver.FindElement(By.Id("ivuFrm_page0ivu2")));
                            driver.SwitchTo().Frame(driver.FindElement(By.Id("isolatedWorkArea")));
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1000);

                            encuentra = 1;
                        }
                        catch (Exception)
                        {
                            if (stop1 == 50)
                            {
                                MessageBox.Show("No encontre los elementos");
                                break;
                            }
                            stop1 = stop1 + 1;
                            encuentra = 0;
                            
                        }
                    } while (encuentra == 0);
                }


                //Click derecho en exportar  
                ///html/body/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table[2]/tbody/tr/td/table/tbody/tr/td
                Thread.Sleep(2000);
                au.WaitUntilElementExistsByXPath(driver, "/html/body/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table[2]/tbody/tr/td/table/tbody/tr[1]/td", 200);
                string nodata = driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table[2]/tbody/tr/td/table/tbody/tr[1]/td")).Text;
                if (!nodata.Equals("No existen datos adecuados"))
                {

                    au.WaitUntilElementExistsByXPath(driver, "/html/body/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table[2]/tbody/tr/td/table/tbody/tr[4]/td[3]/a", 200);

                    int encuentra_descarga = 0;
                    int stop;
                    stop = 0;
                    encuentra_descarga = 0;
                    do
                    {
                        try
                        {
                            Thread.Sleep(1000);
                            string fechaportal = driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td[4]")).Text;
                            string fechacambio = Final + ".." + Final;
                            if (fechaportal == fechacambio)
                            {
                                Actions actions = new Actions(driver);
                                actions.ContextClick(driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table[2]/tbody/tr/td/table/tbody/tr[3]/td[1]/a"))).Perform();
                                IWebElement element = (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("SAPBWJSgdo.m_pMenuMechanics.ShutdownMenu();SAPBWCExport('XLS');");
                                encuentra_descarga = 1;
                            }
                            if (fechaportal != fechacambio)
                            {
                                encuentra_descarga = 0;
                            }
                        }
                        catch (Exception)
                        {
                            if (stop == 50)
                            {

                                break;
                            }
                            stop = stop + 1;
                            encuentra_descarga = 0;
                        }
                    } while (encuentra_descarga == 0);

                    //Esperamos la descaraga


                    string NameFile = "";
                    int fileDownloaded = 0;
                    DirectoryInfo di = new DirectoryInfo(@sUsuarioComple + @"\Downloads\");
                    Thread.Sleep(5000);
                    if (File.Exists(Path.Combine(@sUsuarioComple + @"C:\tmp", NameFile)))
                    {
                        File.Delete(Path.Combine(@"C:\tmp\DatosVTLiverpool.txt"));
                        Thread.Sleep(100);
                        File.Delete(Path.Combine(@"C:\tmp", NameFile));
                        Thread.Sleep(100);
                    }
                    do
                    {
                        try
                        {
                            foreach (var fi in di.GetFiles("SAP*.xls"))
                            {
                                NameFile = fi.Name;
                            }

                            Thread.Sleep(4000);
                            if (File.Exists(Path.Combine(@sUsuarioComple + @"\Downloads", NameFile)))
                            {

                                if (File.Exists(Path.Combine(@sUsuarioComple + @"\Downloads", NameFile)))
                                {
                                    File.Copy(Path.Combine(@sUsuarioComple + @"\Downloads", NameFile), Path.Combine(@"C:\tmp", NameFile));
                                    Thread.Sleep(3000);
                                    fileDownloaded = 0;
                                    do
                                    {
                                        if (File.Exists(Path.Combine(@"C:\tmp", NameFile)))
                                        {
                                            fileDownloaded = 1;
                                        }
                                    } while (fileDownloaded == 0);

                                }

                            }
                            else
                            {
                                fileDownloaded = 0;
                            }
                        }
                        catch (Exception)
                        {

                            fileDownloaded = 0;
                        }

                    } while (fileDownloaded == 0);

                    Thread.Sleep(2000);
                    execute.UseShellExecute = true;
                    execute.FileName = "LiverpoolVTFile.exe";
                    //execute.WorkingDirectory = @"C:\Users\jppereza\Downloads\";
                    execute.WorkingDirectory = @"C:\tmp\";
                    Process.Start(execute);

                    //Espereamos que exista el elemento en la carpeta TMP
                    fileDownloaded = 0;
                    do
                    {
                        if (File.Exists(@"C:\tmp\DatosVTLiverpool.txt"))
                        {
                            fileDownloaded = 1;
                            Thread.Sleep(5000);
                            File.Delete(Path.Combine(@sUsuarioComple + @"\Downloads", NameFile));
                        }
                    } while (fileDownloaded == 0);
                    Thread.Sleep(4000);


                    string readText = File.ReadAllText(Path.Combine(@"C:\tmp\DatosVTLiverpool.txt"));
                    dt = ConvertToDataTable(readText, 18, Final, '\t');

                    //Programamos el Bulk
                    sqlP.connect("[dbo].[Automatizacion_Liverpool]", new string[] { "@Accion:LimpiarVentas" });
                    sql.BulkCopy(dt, "Z_VE_Liverpool_Bot");
                    dt.Clear();
                    sqlP.connect("[dbo].[Automatizacion_Liverpool]", new string[] { "@Accion:VentasLiverpool" });
                    sqlP.connect("[dbo].[Automatizacion_Liverpool]", new string[] { "@Accion:LimpiarVentas" });
                    File.Delete(Path.Combine(@"C:\tmp\DatosVTLiverpool.txt"));
                    Thread.Sleep(100);
                    File.Delete(Path.Combine(@"C:\tmp", NameFile));
                    Thread.Sleep(100);

                }
                else
                {
                    Thread.Sleep(7100);
                    nodata = "";
                }
                

            }
        }


        public DataTable ConvertToDataTable(string texto, int numberOfColumns, string Final_fecha, char delimiter = '|')
        {
            DataTable tbl = new DataTable();

            /* AGREGA EL NUMERO DE COLUMNAS AL DataTable */
            for (int col = 0; col < numberOfColumns; col++)
                tbl.Columns.Add(new DataColumn("Column" + (col + 1).ToString()));

            List<string> lines = texto.ToString().Split(new char[] { '\n' }).ToList();
            string[] cols = new string[18];
            //string[] Aux = new string[18];
            int Aux = 0;

            foreach (string line in lines)
            {
                //string[] Datos = new string[18];

                Aux++;
                //var cols = (line + delimiter + Final_fecha).Split(delimiter);
                cols = (line + delimiter + Final_fecha + "\t" + Aux).Split(delimiter);
                DataRow dr = tbl.NewRow();
                
                tbl.Rows.Add(cols);
                
            }
            return tbl;
        }



    }
}

//this is good