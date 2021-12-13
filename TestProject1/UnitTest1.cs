using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.mvideo.ru/");            
        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.CssSelector(".popular-categories__item-image-container")).Click();

            driver.FindElement(By.CssSelector("input[name=\"minPrice\"]")).Clear();
            driver.FindElement(By.CssSelector("input[name=\"maxPrice\"]")).Clear();
            driver.FindElement(By.CssSelector("input[name=\"minPrice\"]")).SendKeys("1549");

            new WebDriverWait(driver, TimeSpan.FromSeconds(50));

            driver.FindElement(By.CssSelector("input[name=\"maxPrice\"]")).Clear();
            driver.FindElement(By.CssSelector("input[name=\"maxPrice\"]")).SendKeys("20500");          
            
            
            new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                .Until(x => driver.FindElements(By.CssSelector(".product-cards-layout__item.ng-star-inserted")).Count != 0);

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'ProductCardVerticalLayout__wrapper-cart')]//*[contains(@class,'ProductCardVerticalPrice__price-current_current-price')]"))
                .Select(webPrice => webPrice.Text.Trim()).ToArray<string>(), s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 1000 && actualPrice <= 10000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 1000 and less or equal than 10000"));
        }

        [Test]
        public void TestTooltipText()
        {

            driver.FindElement(By.CssSelector(".popular-categories__item-image-container")).Click();
            driver.FindElement(By.CssSelector("button[title=\"Добавить в избранное\"]")).Click();

            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("button[_ngcontent-serverapp-c92]"))).Build().Perform();

            /*Console.WriteLine(driver.FindElement(By.CssSelector("button[title=\"Добавить в избранное\"]")).Text);
            Assert.IsTrue(driver.FindElements(By.CssSelector(".tooltip__item.ng-tns-c194-55")).Any(),
                "Tooltip has not appeared.");
            Assert.AreEqual("К списку избранного", driver.FindElement(By.CssSelector(".button.ng-star-inserted[type=\"submit\"]")).Text.Trim(),
                "Tooltip has not appeared.");     */       
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.CssSelector("a[href=\"https://www.mvideo.ru/login\"]")).Click();
            driver.FindElement(By.CssSelector("input[data-sel=\"c-text-field__input c-phone-verification__input\"]")).SendKeys("9371475354");
            driver.FindElement(By.CssSelector(".c-btn__text")).Click();            
            Assert.IsFalse(driver.FindElements(By.XPath("//button[contains(@class, 'SignUp__button-confirm-phone') and not(@disabled)]")).Any(),
                "Phone number confirmation button is enabel when phone number input has no value.");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
