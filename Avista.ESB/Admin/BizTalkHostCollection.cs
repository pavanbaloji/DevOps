
namespace Avista.ESB.Admin
{
      public class BizTalkHostCollection : BizTalkCollection <BizTalkHost>
      {
            protected BizTalkCatalog bizTalkCatalog;
            public BizTalkHostCollection (BizTalkCatalog catalog)
                  : base( catalog, catalog.BtsCatalogExplorer.Hosts )
            {
            }
      }
}