using System;
using System.Collections;
using System.Collections.Generic;

namespace Avista.ESB.Admin
{     
      public class BizTalkCollection<T> : IReadOnlyCollectionWrapper<T> where T : BizTalkArtifact
      {
            protected BizTalkCatalog biztalkCatalog;

            /// <summary>
            /// BizTalkCollection
            /// </summary>
            /// <param name="paramCatalog"></param>
            /// <param name="collection"></param>
            public BizTalkCollection (BizTalkCatalog paramCatalog, ICollection collection)
                  : base( collection )
            {
                  biztalkCatalog = paramCatalog;
            }
            /// <summary>
            /// BizTalkCatalog
            /// </summary>
            public BizTalkCatalog Catalog
            {
                  get
                  {
                        return biztalkCatalog;
                  }
            }

            /// <summary>
            /// Type name
            /// </summary>
            /// <param name="name"name></param>
            /// <returns>name</returns>
            public virtual T this[ string name ]
            {
                  get
                  {
                        return GetItem( name );
                  }
            }

            /// <summary>
            /// TryExists
            /// </summary>
            /// <param name="name">name</param>
            /// <returns></returns>
            public virtual bool TryExists (string name)
            {
                  try
                  {
                        var t = GetItem( name );
                        return true;
                  }
                  catch ( KeyNotFoundException /* e */)
                  {
                        return false;
                  }
            }

            #region Support Helpers

            /// <summary>
            /// GetItem
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            internal T GetItem (int index)
            {
                  var position = 0;
                  var enumerator = GetEnumerator();
                  while ( enumerator.MoveNext() )
                  {
                        if ( position++ == index )
                              return enumerator.Current;
                  }

                  throw new IndexOutOfRangeException();
            }
            /// <summary>
            /// GetItem
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            internal T GetItem (string name)
            {
                  var enumerator = GetEnumerator();
                  while ( enumerator.MoveNext() )
                  {
                        if ( String.Compare( enumerator.Current.Name, name, StringComparison.InvariantCultureIgnoreCase ) == 0 )
                              return enumerator.Current;
                  }

                  throw new KeyNotFoundException();
            }
            /// <summary>
            /// FromItem
            /// </summary>
            /// <param name="item">object</param>
            /// <returns>Type</returns>
            internal T FromItem (object item)
            {
                  var FromItemMethod = typeof( T ).GetMethod( "FromItem" );
                  return FromItemMethod.Invoke( null, new object[ ] { Catalog, item } ) as T;
            }

            #endregion

            #region ICollection<T> Overrides
            
            /// <summary>
            /// Contains
            /// </summary>
            /// <param name="item">Item</param>
            /// <returns></returns>
            public override bool Contains (T item)
            {
                  foreach ( var o_item in Collection )
                  {
                        var t_item = FromItem( o_item );
                        if ( t_item.Equals( item ) )
                              return true;
                  }

                  return false;
            }
            /// <summary>
            /// CopyTo
            /// </summary>
            /// <param name="array"></param>
            /// <param name="index"></param>
            public override void CopyTo (T[ ] array, int index)
            {
                  for ( int n = index; (n < array.Length); n++ )
                        array.SetValue( GetItem( n - index ), n );
            }

            #endregion

            #region IEnumerable<T> Overrides

            /// <summary>
            /// IEnumerator
            /// </summary>
            /// <returns></returns>
            public override IEnumerator<T> GetEnumerator ()
            {
                  return new BtsEnumerator<T>( this );
            }

            #endregion
      }

      /// <summary>
      /// BtsEnumerator Class
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public sealed class BtsEnumerator<T> : IEnumeratorWrapper<T> where T : BizTalkArtifact
      {
            private readonly BizTalkCollection<T> bizTalkCollection;

            internal BtsEnumerator (BizTalkCollection<T> collection)
                  : base( collection.Collection.GetEnumerator() )
            {
                  bizTalkCollection = collection;
            }

            #region IEnumerator<T> Overrides

            public override T Current
            {
                  get
                  {
                        return bizTalkCollection.FromItem( Item );
                  }
            }

            #endregion
      }
}

