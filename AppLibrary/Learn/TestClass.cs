using System;
using AppLibrary.Learn.Structures;
using AppLibrary.Learn.Structures.BinarySearchTree;

namespace AppLibrary.Learn {
    /// <summary>
    /// 
    /// </summary>
    internal class TestClass {
        /// <summary>
        /// 
        /// </summary>
        private readonly MyLinkedListClass< int > _list = new MyLinkedListClass< int >();
        //private readonly MyCycleLinkedListClass< int > _list = new MyCycleLinkedListClass< int >();
        //private readonly MyDoubleLinkedListClass< int > _list = new MyDoubleLinkedListClass< int >();
        //private readonly MyDoubleCycleLinkedListClass< int > _list = new MyDoubleCycleLinkedListClass< int >();

        /// <summary>
        /// 
        /// </summary>
        private readonly Action< int > _add;

        /// <summary>
        /// 
        /// </summary>
        private readonly Action< int, bool > _delete;

        /// <summary>
        /// 
        /// </summary>
        private const int Max = 10;

        /// <summary>
        /// 
        /// </summary>
        public TestClass() {
            _add    = _list.Add;
            _delete = _list.Delete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="max"></param>
        /// <param name="visible"></param>
        private void Add( string str, int max, bool visible = false ) {
            _list.Clear();
            //App.MyWindows.TextLine += str;
            for ( var i = 0; i < max; i++ ) {
                _add( i );
                _add( i );
                //if ( visible ) App.MyWindows.TextLine += _list;
            }
            //if ( !visible ) App.MyWindows.TextLine += _list;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Test1() {
            //App.MyWindows.Text = string.Empty;
            //Add( "Тест добавления данных", Max, true );
            ////
            //App.MyWindows.TextLine += "Тест удаления данных сначала";
            //App.MyWindows.TextLine += _list;
            //for ( var i = 0; i < Max; i++ ) {
            //    _delete( i, false );
            //    App.MyWindows.TextLine += _list;
            //}
            ////
            //Add( "Тест удаления данных сконца", Max );
            //for ( var i = Max - 1; i >= 0; i-- ) {
            //    _delete( i, false );
            //    App.MyWindows.TextLine += _list;
            //}
            ////
            //Add( "Тест сброса данных", Max );
            //_list.Clear();
            //App.MyWindows.TextLine += _list;
            ////
            //Add( "Тест удаления произвольных данных", Max );
            //_delete( 1, false );
            //App.MyWindows.TextLine += _list;
            //_delete( 3, false );
            //App.MyWindows.TextLine += _list;
            //_delete( 5, false );
            //App.MyWindows.TextLine += _list;
            //_delete( 7, false );
            //App.MyWindows.TextLine += _list;
            //_delete( 9, false );
            //App.MyWindows.TextLine += _list;
            ////
            //App.MyWindows.TextLine += "Тест удаления всех одинаковых данных";
            //_list.Clear();
            //for ( var i = 0; i < Max; i++ ) {
            //    _add( 101 );
            //    _add( i );
            //    _add( 102 );
            //}
            //App.MyWindows.TextLine += _list;
            //_delete( 101, true );
            //App.MyWindows.TextLine += _list;
            //_delete( 102, true );
            //App.MyWindows.TextLine += _list;
            //
            //
            //
            //
            //Add( "Тест чтения данных в обратную сторону", Max );
            //App.MyWindows.Text += $"DoubleLinkedList {_list.Count:D3} items : ";
            //foreach( var item in _list.BackEnumerator() ) {
            //    App.MyWindows.Text += item + "  ";
            //}
            //App.MyWindows.TextLine += "";
            ////
            //Add( "Тест добавления данных", Max );
            //for ( var i = 0; i < Max; i++ ) {
            //    _list.AddFirst( i + 100 );
            //}
            //App.MyWindows.TextLine += _list;
        }

        public static void Test() {
            //SortClass.BubbleSort( new[] { 2, 5, 7, 0, 13, 4, 8, 11, 3, 17, 1, 15 } );
            //SortClass.ShakerSort( new[] { 2, 5, 7, 0, 13, 4, 8, 11, 3, 17, 1, 15 } );
            //SortClass.CombSort( new[] { 2, 5, 7, 0, 13, 4, 8, 11, 3, 17, 1, 15 } );
            //var  mas = new int[25];
            //var r   = new Random();
            //for ( var i = 0; i < mas.Length; i++ ) {
            //    mas[ i ] = r.Next( -100, 101 );
            //}
            //SortClass.QuickSort( new[] { 2, 17, 5, 3, 7, 0, -1, 8, 3, 11, 3, 17, 1, 3, 15, 6 ,17,17} );

            var tree = new Tree< int >();
            //tree.Add( 5 );
            //tree.Add( 3 );
            //tree.Add( 1 );
            //tree.Add( 2 );
            //tree.Add( 4 );
            //tree.Add( 8 );
            //tree.Add( 7 );
            //tree.Add( 6 );
            //tree.Add( 9 );

            tree.AddRange( new[] { 5, 3, 8, 1, 4, 7, 9, 2, 6 } );

            //foreach ( var item in tree.Preorder() ) {
            //    App.MyWindows.Text += item + ", ";
            //}
            //App.MyWindows.TextLine += "";

            //foreach ( var item in tree.Postorder() ) {
            //    App.MyWindows.Text += item + ", ";
            //}
            //App.MyWindows.TextLine += "";

            //foreach ( var item in tree.Inorder() ) {
            //    App.MyWindows.Text += item + ", ";
            //}
            //App.MyWindows.TextLine += "";
        }
    }
}