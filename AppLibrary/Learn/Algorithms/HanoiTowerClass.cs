using System.Collections.Generic;

namespace AppLibrary.Learn.Algorithms {
    public class HanoiTowerClass {
        private readonly List< int > _a = new List< int >();
        private readonly List< int > _b = new List< int >();
        private readonly List< int > _c = new List< int >();

        public void Move() {
            _a.Clear();
            _b.Clear();
            _c.Clear();
            for ( var i = 0; i < 25; i++ ) _a.Add( i );

            Move( _a, _b, _c, _a.Count );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void Swap( IList< int > from, ICollection< int > to ) {
            if ( from == null || to == null || from.Count == 0 ) return;
            var temp = from[ from.Count - 1 ];
            from.RemoveAt( from.Count - 1 );
            to.Add( temp );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="n"></param>
        private static void Move( List< int > a, List< int > b, List< int > c, int n ) {
            if ( n == 0 ) return;
            // Переносим все диски, кроме самого большого
            // со стежня A на стержень B
            Move( a, c, b, n - 1 );
            //// Переносим самый большой диск со стержня A на стержень C
            Swap( a, c );
            //// Переносим все диски со стержня B на стержень C 
            //// поверх самого большого диска
            Move( b, a, c, n - 1 );
        }
    }
}