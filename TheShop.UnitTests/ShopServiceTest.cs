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
            const int id = 42;
            const string name = "Article from supplier1";
            const int low_price = 498;
            const int high_price = 500;

            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article = new Article()
            {
                ID = id,
                Name_of_article = name,
                ArticlePrice = low_price
            };
            supplier1Mock.Setup(s => s.ArticleInInventory(id)).Returns(true);
            supplier1Mock.Setup(s => s.GetArticle(id)).Returns(article);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            Article result = shopService.OrderArticle(id, high_price);
            Assert.AreEqual(result.ID, id);
            Assert.AreEqual(result.Name_of_article, name);
        }

        [Test]
        public void TestOrderArticleTwoSuppliersSecondHas()
        {
            const int id = 42;
            const string name1 = "Article from supplier1";
            const string name2 = "Article from supplier2";
            const int low_price = 458;
            const int medium_price = 500;
            const int high_price = 505;

            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article = new Article()
            {
                ID = id,
                Name_of_article = name1,
                ArticlePrice = high_price
            };
            supplier1Mock.Setup(s => s.ArticleInInventory(id)).Returns(true);
            supplier1Mock.Setup(s => s.GetArticle(id)).Returns(article);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);
            Article article2 = new Article()
            {
                ID = id,
                Name_of_article = name2,
                ArticlePrice = low_price
            };
            supplier2Mock.Setup(s => s.ArticleInInventory(id)).Returns(true);
            supplier2Mock.Setup(s => s.GetArticle(id)).Returns(article2);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            Article result = shopService.OrderArticle(id, medium_price);
            Assert.AreEqual(result.ID, id);
            Assert.AreEqual(result.Name_of_article, name2);
        }

        [Test]
        public void TestOrderArticleTwoSuppliersNoOneHas()
        {
            const int id = 42;
            const int price = 500;

            Mock<IDatabaseDriver> databaseMock = new Mock<IDatabaseDriver>(MockBehavior.Strict);
            Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Mock<ISupplier> supplier1Mock = new Mock<ISupplier>(MockBehavior.Strict);
            supplier1Mock.Setup(s => s.ArticleInInventory(id)).Returns(false);
            Mock<ISupplier> supplier2Mock = new Mock<ISupplier>(MockBehavior.Strict);

            ShopService shopService = new ShopService(databaseMock.Object, loggerMock.Object, new ISupplier[] { supplier1Mock.Object, supplier2Mock.Object });

            var ex = Assert.Throws<Exception>(() => shopService.OrderArticle(id, price));
            Assert.AreEqual(ex.Message, "Could not order article");
        }

        [Test]
        public void TestSellArticle()
        {
            const int id = 42;
            const string name = "Article from supplier1";
            const int price = 500;
            const int user_id = 58;

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
                ID = id,
                Name_of_article = name,
                ArticlePrice = price
            };
            shopService.SellArticle(id, article, user_id);

            Assert.AreEqual(debug, "Trying to sell article with id=" + id);
            Assert.AreEqual(info, "Article with id=" + id + " is sold.");
            Assert.IsNotNull(written);
            Assert.AreEqual(written.BuyerUserId, user_id);
            Assert.AreEqual(written.IsSold, true);
        }
    }
}
