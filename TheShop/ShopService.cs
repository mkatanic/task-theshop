using System;
using System.Collections.Generic;
using System.Linq;

namespace TheShop
{
	public class ShopService
	{
		private IDatabaseDriver databaseDriver;
		private ILogger logger;

        private ISupplier[] suppliers;
		
		public ShopService(IDatabaseDriver databaseDriver, ILogger logger, ISupplier[] suppliers)
		{
			this.databaseDriver = databaseDriver;
			this.logger = logger;
            this.suppliers = suppliers;
		}

        public static ShopService CreateService()
        {
            ISupplier[] suppliers = new ISupplier[]{
                new Supplier1(),
                new Supplier2(),
                new Supplier3()
            };
            return new ShopService(new DatabaseDriver(), new Logger(), suppliers);
        }

        public Article OrderArticle(int id, int maxExpectedPrice)
        {
            Article tempArticle = null;

            foreach (ISupplier supplier in suppliers)
            {
                bool articleExists = supplier.ArticleInInventory(id);
                if (!articleExists)
                    break;
                tempArticle = supplier.GetArticle(id);
                if (maxExpectedPrice >= tempArticle.ArticlePrice)
                    break;
            }

            if (tempArticle == null)
            {
                throw new Exception("Could not order article");
            }

            return tempArticle;
        }

        public void SellArticle(int id, Article article, int buyerId)
        {
            logger.Debug("Trying to sell article with id=" + id);

            article.IsSold = true;
            article.SoldDate = DateTime.Now;
            article.BuyerUserId = buyerId;

            try
            {
                databaseDriver.Save(article);
                logger.Info("Article with id=" + id + " is sold.");
            }
            catch (ArgumentNullException ex)
            {
                logger.Error("Could not save article with id=" + id);
                throw new Exception("Could not save article with id");
            }
            catch (Exception)
            {
            }
        }

		public void OrderAndSellArticle(int id, int maxExpectedPrice, int buyerId)
		{
            Article article = OrderArticle(id, maxExpectedPrice);
            SellArticle(id, article, buyerId);
		}

		public Article GetById(int id)
		{
			return databaseDriver.GetById(id);
		}
	}
}
