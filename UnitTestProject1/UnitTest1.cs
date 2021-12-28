using System;
using kursach.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAuth()
        {
            var page = new Auth();

            Assert.IsTrue(page.testauth("gr692_pav", "plo030803a"));
            Assert.IsTrue(page.testauth("test1", "qwerty"));
            Assert.IsTrue(page.testauth("hehehelp", "hehehe"));

            Assert.IsFalse(page.testauth("", ""));
            Assert.IsFalse(page.testauth("test1", ""));
            Assert.IsFalse(page.testauth("", "hehehe"));
            Assert.IsFalse(page.testauth("dsvdvd", "dfvfdfv"));
            Assert.IsFalse(page.testauth("plo030803a", "gr692_pav"));
        }

        [TestMethod]
        public void TestReg()
        {
            var page = new Reg();

            Assert.IsTrue(page.testreg("Кашлач", "Никита", "Сергеевич", "nikich4523", "nikich4523", "nikich4523"));

            Assert.IsFalse(page.testreg("", "", "", "", "", ""));
            Assert.IsFalse(page.testreg("", "Никита", "Сергеевич", "nikich4523", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "", "Сергеевич", "nikich4523", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "", "nikich4523", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "nikich4523", "", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "nikich4523", "nikich4523", ""));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "test1", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "nikich4523", "nikich4523", "nikich"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "4523", "nikich4523", "nikich4523"));
            Assert.IsFalse(page.testreg("Кашлач", "Никита", "Сергеевич", "nikich4523", "4523", "4523"));
        }

        [TestMethod]
        public void TestAcc()
        {
            var page = new Account(1);
            string Exp, Res;

            Exp = "Плотникова Алёна Владимировна";
            Res = page.testaccLFM();
            Assert.AreEqual(Exp, Res);

            Exp = "gr692_pav";
            Res = page.testaccLogin();
            Assert.AreEqual(Exp, Res);
        }
    }
}
