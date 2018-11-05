using System;
using Microsoft.BizTalk.ExplorerOM;

namespace Avista.ESB.Admin
{

      public abstract class BizTalkArtifact : IEquatable<BizTalkArtifact>
      {
            protected BizTalkCatalog bizTalkcatalog;

      /// <summary>
      /// BizTalkArtifact
      /// </summary>
      /// <param name="catalog"></param>
       protected BizTalkArtifact (BizTalkCatalog catalog)
        {
              bizTalkcatalog = catalog;
        }

      /// <summary>
      /// BizTalkCatalog
      /// </summary>
        public BizTalkCatalog Catalog
        {
              get
              {
                    return bizTalkcatalog;
              }
        }
      /// <summary>
      /// Microsoft BizTalk ExplorderOM BtsCatalogexplorer
      /// </summary>
        protected BtsCatalogExplorer BtsCatalogExplorer
        {
              get
              {
                    return bizTalkcatalog.BtsCatalogExplorer;
              }
        }
      /// <summary>
      /// Name
      /// </summary>
        public abstract string Name { get; }

        #region IEquatable<BtsArtifact> Members

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other">other</param>
      /// <returns></returns>
        public bool Equals(BizTalkArtifact other)
        {
            if (ReferenceEquals(null, other)) return false;
            return (Name.Equals(other.Name));
        }

        #endregion

        #region Equality Implementation

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="obj">object</param>
      /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BizTalkArtifact);
        }

         /// <summary>
         /// GetHashCode
         /// </summary>
         /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

       /// <summary>
       /// Operator equal to
       /// </summary>
       /// <param name="left">left</param>
       /// <param name="right">right</param>
       /// <returns></returns>
        public static bool operator ==(BizTalkArtifact left, BizTalkArtifact right)
        {
            if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
            return left.Equals(right);
        }
        /// <summary>
        /// Operator not equal to
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns></returns>
        public static bool operator !=(BizTalkArtifact left, BizTalkArtifact right)
        {
            return !(left == right);
        }

        #endregion
    }
}