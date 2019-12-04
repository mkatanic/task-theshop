using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheShop
{
    interface ISupplier
    {
        bool ArticleInInventory(int id);

        Article GetArticle(int id);
    }
}
