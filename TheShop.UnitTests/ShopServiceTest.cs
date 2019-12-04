using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace TheShop.UnitTests
{
    [TestFixture]
    public class ShopServiceTest
    {
        [Test]
        public void TestOrderArticleTwoSuppliersFirstHas()
        {
            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article = new Article()
            {
                ID = 42,
                Name_of_article = "Article from supplier1",
                ArticlePrice = 498
            };
            supplier1Mock.Setup(s => s.ArticleInInventory(42)).Returns(true);
            supplier1Mock.Setup(s => s.GetArticle(42)).Returns(article);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            Article result = shopService.OrderArticle(42, 500);
            Assert.AreEqual(result.ID, 42);
            Assert.AreEqual(result.Name_of_article, "Article from supplier1");
        }

        [Test]
        public void TestOrderArticleTwoSuppliersSecondHas()
        {
            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article = new Article()
            {
                ID = 42,
                Name_of_article = "Article from supplier1",
                ArticlePrice = 505
            };
            supplier1Mock.Setup(s => s.ArticleInInventory(42)).Returns(true);
            supplier1Mock.Setup(s => s.GetArticle(42)).Returns(article);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article2 = new Article()
            {
                ID = 42,
                Name_of_article = "Article from supplier2",
                ArticlePrice = 458
            };
            supplier2Mock.Setup(s => s.ArticleInInventory(42)).Returns(true);
            supplier2Mock.Setup(s => s.GetArticle(42)).Returns(article2);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            Article result = shopService.OrderArticle(42, 500);
            Assert.AreEqual(result.ID, 42);
            Assert.AreEqual(result.Name_of_article, "Article from supplier2");
        }

        [Test]
        public void TestOrderArticleTwoSuppliersNoOneHas()
        {
            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            supplier1Mock.Setup(s => s.ArticleInInventory(42)).Returns(false);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            var ex = Assert.Throws<Exception>(() => shopService.OrderArticle(42, 500));
            Assert.AreEqual(ex.Message, "Could not order article");
        }

        [Test]
        public void TestSellArticle()
        {
            string info = "", debug = "";
            Article written = null;

            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            databaseMock.Setup(d => d.Save(It.IsAny<Article>())).Callback<Article>(a => written = a);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            loggerMock.Setup(l => l.Debug(It.IsAny<string>())).Callback<string>(s => debug = s);
            loggerMock.Setup(l => l.Info(It.IsAny<string>())).Callback<string>(s => info = s);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            Article article = new Article()
            {
                ID = 42,
                Name_of_article = "Article from supplier1",
                ArticlePrice = 505
            };
            shopService.SellArticle(42, article, 58);

            Assert.AreEqual(debug, "Trying to sell article with id=42");
            Assert.AreEqual(info, "Article with id=42 is sold.");
            Assert.IsNotNull(written);
            Assert.AreEqual(written.BuyerUserId, 58);
            Assert.AreEqual(written.IsSold, true);
        }
    }
}
